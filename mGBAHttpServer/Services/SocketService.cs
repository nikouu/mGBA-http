using mGBAHttpServer.Models;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace mGBAHttpServer.Services
{
    public class SocketService : IDisposable
    {
        // TODO: this isn't a correct way to check if mGBA has closed the connection 
        private bool _isConnected = false;
        private readonly IPEndPoint _tcpEndPoint;
        private readonly Socket _tcpSocket;

        public SocketService() {
            var ipAddress = IPAddress.Parse("127.0.0.1");
            _tcpEndPoint = new(ipAddress, 8888);
            _tcpSocket = new(_tcpEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        public async Task SendMessage(MessageModel message)
        {
            if (!_isConnected)
            {
                await _tcpSocket.ConnectAsync(_tcpEndPoint);
                _isConnected = true;
            }

            var messageBytes = Encoding.UTF8.GetBytes(message.ToString());
            _ = await _tcpSocket.SendAsync(messageBytes, SocketFlags.None);

            var buffer = new byte[1_024];
            var received = await _tcpSocket.ReceiveAsync(buffer, SocketFlags.None);
            var response = Encoding.UTF8.GetString(buffer, 0, received);
        }

        public void Dispose()
        {
            _tcpSocket?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
