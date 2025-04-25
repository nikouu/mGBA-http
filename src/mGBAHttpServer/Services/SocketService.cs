using mGBAHttpServer.Models;
using Microsoft.Extensions.Options;
using Microsoft.IO;
using System.Buffers;
using System.Drawing;
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
        private static readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager = new();

        public SocketService(IOptions<SocketOptions> socketOptions)
        {
            var ipAddress = IPAddress.Parse(socketOptions.Value.IpAddress);
            _tcpEndPoint = new(ipAddress, socketOptions.Value.Port);
            _tcpSocket = new(_tcpEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
            {
                SendTimeout = socketOptions.Value.WriteTimeout,
                ReceiveTimeout = socketOptions.Value.ReadTimeout,
                NoDelay = true
            };
        }

        public async Task<string> SendMessageAsync(MessageModel message)
        {
            var response = "";
            for (int i = 0; i < _maxRetries; i++)
            {
                var buffer = ArrayPool<byte>.Shared.Rent(1024);
                try
                {
                    if (!_isConnected)
                    {
                        await _tcpSocket.ConnectAsync(_tcpEndPoint);
                        _isConnected = true;
                    }

                    var messageBytes = Encoding.UTF8.GetBytes(message.ToString());
                    await _tcpSocket.SendAsync(messageBytes, SocketFlags.None);

                    using var memoryStream = _recyclableMemoryStreamManager.GetStream();                    
                    int totalBytesRead = 0;
                    int bytesRead;

                    do
                    {
                        bytesRead = await _tcpSocket.ReceiveAsync(buffer, SocketFlags.None);
                        if (bytesRead > 0)
                        {
                            await memoryStream.WriteAsync(buffer.AsMemory(0, bytesRead));
                            totalBytesRead += bytesRead;
                        }
                    } while (bytesRead > 0 && _tcpSocket.Available > 0);

                    response = Encoding.UTF8.GetString(memoryStream.GetReadOnlySequence());
                    
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
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }
            }

            return response.Replace("<|ACK|>", "");
        }

        public void Dispose()
        {
            if (_tcpSocket.Connected)
            {
                _tcpSocket.Close();
                //_tcpSocket.Shutdown(SocketShutdown.Both);
            }
            _tcpSocket?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
