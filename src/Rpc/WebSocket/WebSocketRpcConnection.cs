using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LeanCloud.Engine.Rpc
{
    public class WebSocketRpcConnection : RpcConnection
    {
        public WebSocket WebSocket { get; set; }

        public WebSocketRpcConnection()
        {

        }

        public WebSocketRpcConnection(WebSocket webSocket)
        {
            WebSocket = webSocket;
        }

        public override async Task SendAsync(string message)
        {
            if(this.Server is WebSocketRpcServer webSocketRpcServer)
            {
                if(webSocketRpcServer.ToggleLog) webSocketRpcServer.Log(message);
            }
            if (WebSocket.State != WebSocketState.Open) return;
            var arr = Encoding.UTF8.GetBytes(message);

            var buffer = new ArraySegment<byte>(
                    array: arr,
                    offset: 0,
                    count: arr.Length);

            try
            {
                await WebSocket.SendAsync(
                    buffer: buffer,
                    messageType: WebSocketMessageType.Text,
                    endOfMessage: true,
                    cancellationToken: CancellationToken.None);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
