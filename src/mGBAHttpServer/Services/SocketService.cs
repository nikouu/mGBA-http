using mGBAHttpServer.Models;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace mGBAHttpServer.Services
{
    public class SocketService : IDisposable
    {
        private bool _isConnected = false;
        private readonly IPEndPoint _tcpEndPoint;
        private readonly Socket _tcpSocket;
        private const int _maxRetries = 3;

        public SocketService()
        {
            var ipAddress = IPAddress.Parse("127.0.0.1");
            _tcpEndPoint = new(ipAddress, 8888);
            _tcpSocket = new(_tcpEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        public async Task<string> SendMessageAsync(MessageModel message)
        {
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
                    // Maybe mGBA closes the socket but doesn't inform the other side?
                    // Like how client.Shutdown(SocketShutdown.Both) in C# would
                    _tcpSocket.Disconnect(true);
                    _isConnected = false;
                }
                catch (Exception)
                {
                    throw;
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
