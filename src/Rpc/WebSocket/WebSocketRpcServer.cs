using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace LeanCloud.Engine.Rpc
{
    public class WebSocketRpcServer : RpcConnectionLayer
    {
        public string WebSocketRoutePath { get; set; } = "/rpc";
        public bool ToggleLog { get; set; }

        public WebSocketRpcServer(RpcServer businessServer) : base(businessServer)
        {
            this.BusinessServer.Connections = new List<RpcConnection>();
        }

        public override Task<RpcConnection> OnConnected(RpcConnection connection)
        {
            return this.BusinessServer.OnConnected(connection);
        }

        public void Log(string message)
        {
            Console.WriteLine($"{DateTime.Now.ToString("hh:mm:ss")}: {message}");
        }

        public async Task<WebSocketRpcConnection> OnWebSocketConnected(HttpContext context, WebSocket webSocket)
        {
            var connection = new WebSocketRpcConnection(webSocket);
            await this.OnConnected(connection);
            connection.Client.Courier = connection.SendAsync;
            connection.Server = this;
            this.BusinessServer.Connections.Add(connection);
            return connection;
        }

        public async Task OnWebSocketInvoked(HttpContext context, WebSocketRpcConnection connection)
        {
            try
            {
                while (connection.WebSocket.State == WebSocketState.Open)
                {
                    ArraySegment<Byte> buffer = new ArraySegment<byte>(new Byte[1024 * 4]);
                    WebSocketReceiveResult result = null;
                    try
                    {
                        if (!connection.WebSocket.CloseStatus.HasValue)
                        {
                            using (var ms = new MemoryStream())
                            {
                                do
                                {
                                    result = await connection.WebSocket.ReceiveAsync(buffer, CancellationToken.None).ConfigureAwait(false);
                                    ms.Write(buffer.Array, buffer.Offset, result.Count);
                                }

                                while (!result.EndOfMessage);

                                ms.Seek(0, SeekOrigin.Begin);

                                using (var reader = new StreamReader(ms, Encoding.UTF8))
                                {
                                    string message = await reader.ReadToEndAsync().ConfigureAwait(false);
                                    if (ToggleLog)
                                    {
                                        var request = context.Request;
                                        string log = $"{request.Method} {request.Scheme}://{request.Host}{request.Path} {message}";
                                        Log(log);
                                    }

                                    await OnReceived(connection, message);
                                }
                            }
                        }
                        else
                        {
                            switch (connection.WebSocket.State)
                            {
                                case WebSocketState.Aborted:
                                case WebSocketState.Closed:
                                    await OnDisconnected(connection);
                                    break;
                                case WebSocketState.CloseReceived:
                                    break;
                                case WebSocketState.CloseSent:
                                    break;
                            }
                        }
                    }

                    catch (WebSocketException websocketEx)
                    {
                        if (websocketEx.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
                        {
                            connection.WebSocket.Abort();
                        }
                    }
                }
                await this.OnDisconnected(connection);

            }
            catch (Exception ex)
            {
                if (connection.WebSocket.CloseStatus.HasValue)
                {
                    await this.OnDisconnected(connection);
                }
                if (ex.Source == "Microsoft.AspNetCore.WebSockets.Protocol" && ex.Message == "Unexpected end of stream")
                {
                    await this.OnDisconnected(connection);
                }

                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Message);
            }
        }

        public override Task OnReceived(RpcConnection sender, string message)
        {
            return this.BusinessServer.OnReceived(sender, message);
        }

        public override Task OnDisconnected(RpcConnection connection)
        {
            return this.BusinessServer.OnDisconnected(connection);
        }
    }
}
