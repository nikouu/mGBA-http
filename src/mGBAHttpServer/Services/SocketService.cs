using mGBAHttpServer.Models;
using Microsoft.Extensions.Options;
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
        private readonly TimeSpan _initialRetryDelay = TimeSpan.FromSeconds(1);

        public SocketService(IOptions<SocketOptions> socketOptions)
        {
            var ipAddress = IPAddress.Parse(socketOptions.Value.IpAddress);
            _tcpEndPoint = new(ipAddress, socketOptions.Value.Port);
            _tcpSocket = new(_tcpEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
            {
                SendTimeout = socketOptions.Value.WriteTimeout,
                ReceiveTimeout = socketOptions.Value.ReadTimeout
            };
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
                    // fixes problem if the other side has been uncleanly terminated
                    _tcpSocket.Disconnect(reuseSocket: true);
                    _isConnected = false;
                    var retryDelay = _initialRetryDelay * (i + 1);
                    await Task.Delay(retryDelay);
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
            if (_tcpSocket.Connected)
            {
                _tcpSocket.Shutdown(SocketShutdown.Both);
            }
            _tcpSocket?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
