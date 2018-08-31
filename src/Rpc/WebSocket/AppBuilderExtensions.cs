using System;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Builder;

namespace LeanCloud.Engine.Rpc
{
    public static class AppBuilderExtensions
    {
        public static IApplicationBuilder UseRpcServer(this IApplicationBuilder app, Cloud cloud, RpcServer server, string websocketRoutePath = "/ws")
        {
            WebSocketRpcServer websocketServer = new WebSocketRpcServer(server);

            var hostingUrlWithSchema = cloud.IsProduction ? cloud.GetHostingUrl() : "http://localhost:3000";
            var uri = new Uri(hostingUrlWithSchema);

            var hostWithPort = uri.Port > 0 ? $"{uri.Host}:{uri.Port}" : $"{uri.Host}";

            websocketServer.WebSocketRoutePath = websocketRoutePath;
            server.ServerUrl = $"{uri.Scheme}://{hostWithPort}";

            var wsSchema = uri.Scheme.ToLower().Equals("http") ? "ws" : "wss";
            server.ConnectingUrl = $"{wsSchema}://{hostWithPort}{websocketRoutePath}";

            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024
            };

            app.UseWebSockets(webSocketOptions);

            websocketServer.ToggleLog = true;

            app.Use(async (context, next) =>
            {
                if (context.Request.Path != websocketServer.WebSocketRoutePath)
                {
                    await next.Invoke();
                    return;
                }

                if (!context.WebSockets.IsWebSocketRequest)
                {
                    context.Response.StatusCode = 400;
                    return;
                }

                WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();

                var connection = await websocketServer.OnWebSocketConnected(context, webSocket);
                if (connection != null)
                {
                    await websocketServer.OnWebSocketInvoked(context, connection);
                }
                else
                {
                    context.Response.StatusCode = 404;
                }
            });
            return app;
        }
    }
}
