using mGBAHttpServer.Models;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace mGBAHttpServer.Services
{
    public class SocketService : IDisposable
    {
        // TODO: this isn't a correct way to check if mGBA has closed the connection 
        // Reconnecting after an Scripting > File > Reset seems to be difficult. Maybe mGBA doesn't
        // successfully drop the socket?
        private bool _isConnected = false;
        private readonly IPEndPoint _tcpEndPoint;
        private readonly Socket _tcpSocket;
        private const int _maxRetries = 3;
        private int _retryAttempts = 0;

        public SocketService()
        {
            var ipAddress = IPAddress.Parse("127.0.0.1");
            _tcpEndPoint = new(ipAddress, 8888);
            _tcpSocket = new(_tcpEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        public async Task<string> SendMessageAsync(MessageModel message)
        {
            if (_retryAttempts > 3)
            {
                throw new Exception();
            }

            var response = "";

            for (int i = 0; i < _maxRetries; i++)
            {
                try
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
                    response = Encoding.UTF8.GetString(buffer, 0, received);
                    break;
                }
                catch (SocketException ex) when (ex.NativeErrorCode.Equals(10053))
                {
                    _tcpSocket.Disconnect(true);
                    _isConnected = false;                    
                }
            }

            return response.Replace("<|ACK|>", "");
        }

        public void Dispose()
        {
            _tcpSocket?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
