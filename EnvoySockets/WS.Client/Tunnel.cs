using System;
using System.Net.WebSockets;
using System.Text;

namespace WS.Client
{
    public class Tunnel
    {
        public static async Task ConnectWebSocketAsync(string uri)
        {
            using ClientWebSocket clientWebSocket = new ClientWebSocket();
            try
            {
                // Connect to the WebSocket server
                await clientWebSocket.ConnectAsync(new Uri(uri), CancellationToken.None);
                Console.WriteLine("Connected to the WebSocket server.");

                // Send a message to the WebSocket server
                string message = "Hello, WebSocket!";
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                await clientWebSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true,
                    CancellationToken.None);
                Console.WriteLine("Message sent: " + message);

                // Receive a message from the WebSocket server
                byte[] buffer = new byte[1024];
                WebSocketReceiveResult result =
                    await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine("Message received: " + receivedMessage);

                // Close the WebSocket connection
                await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                Console.WriteLine("WebSocket connection closed.");
            }
            catch (WebSocketException ex)
            {
                Console.WriteLine("WebSocket error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }


        public static async Task Listen(ClientWebSocket clientWebSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result =
                await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            try
            {
                string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);

                var newMessage = $"received {receivedMessage}";

                var responseBuffer = Encoding.UTF8.GetBytes(newMessage);

                await clientWebSocket.SendAsync(
                    new ArraySegment<byte>(responseBuffer, 0, responseBuffer.Length),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);

                await Listen(clientWebSocket);
            }
            catch (WebSocketException ex)
            {
                Console.WriteLine("WebSocket error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        public static async Task RunListener(string uri)
        {
            using ClientWebSocket clientWebSocket = new ClientWebSocket();
            await clientWebSocket.ConnectAsync(new Uri(uri), CancellationToken.None);
            Console.WriteLine("Connected to the WebSocket server.");
        }
    }
}
