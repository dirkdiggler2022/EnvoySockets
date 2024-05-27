using System.Net.WebSockets;

namespace WS.Server
{
    public class Tunnel
    {
        public readonly byte[] _buffer = new byte[1024 * 4];
        public byte[] Buffer { get { return _buffer; } }
        public WebSocket TunnelSocket { get; set; }
    }
}
