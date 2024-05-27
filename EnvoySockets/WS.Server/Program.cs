using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;
using System.Text;
namespace WS.Server
{
    public class Program
    {
        static readonly Tunnel tunnel = new Tunnel();
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            var app = builder.Build();

            // <snippet_UseWebSockets>
            var webSocketOptions = new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromMinutes(2)
            };

            app.UseWebSockets(webSocketOptions);

            app.MapGet("/", () => "Hello World!");

            app.Map("/ws", async (HttpContext context) =>
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    tunnel.TunnelSocket = await context.WebSockets.AcceptWebSocketAsync();
                    Console.Out.WriteLine("Client connected to tunnel");

                    await Listen();

                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                }

            });

            app.Map("/sendmessage", static async (HttpContext context) =>
            {

                var message = context.Request.BodyReader.ReadAsync();
                
                var messageText = System.Text.UTF8Encoding.UTF8.GetString(message.Result.Buffer);
                await SendToClient(System.Text.UTF8Encoding.UTF8.GetBytes(messageText));
            });

            

            app.Run();
        }


        private static async Task SendToClient(byte[] buffer)
        {
            await tunnel.TunnelSocket.SendAsync(
                new ArraySegment<byte>(buffer, 0, buffer.Length),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }
        private static async Task Listen()
        {
            //var buffer = new byte[1024 * 4];
            //var buffer = tunnel.Buffer;
            var receiveResult = await tunnel.TunnelSocket.ReceiveAsync(
                new ArraySegment<byte>(tunnel.Buffer), CancellationToken.None);

            var messageText = System.Text.UTF8Encoding.UTF8.GetString(tunnel.Buffer,0,receiveResult.Count);
            while (!receiveResult.CloseStatus.HasValue)
            {
                //await webSocket.SendAsync(
                //    new ArraySegment<byte>(buffer, 0, receiveResult.Count),
                //    receiveResult.MessageType,
                //    receiveResult.EndOfMessage,
                //    CancellationToken.None);

                //receiveResult = await webSocket.ReceiveAsync(
                //    new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await tunnel.TunnelSocket.CloseAsync(
                receiveResult.CloseStatus.Value,
                receiveResult.CloseStatusDescription,
                CancellationToken.None);
        }

        //private static async Task Echo(WebSocket webSocket)
        //{
        //    //var buffer = new byte[1024 * 4];
        //    var buffer = tunnel.Buffer;
        //    var receiveResult = await webSocket.ReceiveAsync(
        //        new ArraySegment<byte>(buffer), CancellationToken.None);

        //    while (!receiveResult.CloseStatus.HasValue)
        //    {
        //        await webSocket.SendAsync(
        //            new ArraySegment<byte>(buffer, 0, receiveResult.Count),
        //            receiveResult.MessageType,
        //            receiveResult.EndOfMessage,
        //            CancellationToken.None);

        //        receiveResult = await webSocket.ReceiveAsync(
        //            new ArraySegment<byte>(buffer), CancellationToken.None);
        //    }

        //    await webSocket.CloseAsync(
        //        receiveResult.CloseStatus.Value,
        //        receiveResult.CloseStatusDescription,
        //        CancellationToken.None);
        //}
    }
}
