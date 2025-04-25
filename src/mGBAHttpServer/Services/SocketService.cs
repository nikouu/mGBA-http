using mGBAHttpServer.Models;
using Microsoft.Extensions.Options;
using Microsoft.IO;
using System.Buffers;
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
        private readonly TimeSpan _initialRetryDelay = TimeSpan.FromMilliseconds(500);
        private static readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager = new();
        private readonly ILogger<SocketService> _logger;

        public SocketService(IOptions<SocketOptions> socketOptions, ILogger<SocketService> logger)
        {
            var ipAddress = IPAddress.Parse(socketOptions.Value.IpAddress);
            _tcpEndPoint = new(ipAddress, socketOptions.Value.Port);
            _tcpSocket = new(_tcpEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
            {
                SendTimeout = socketOptions.Value.WriteTimeout,
                ReceiveTimeout = socketOptions.Value.ReadTimeout,
                NoDelay = true
            };
            _logger = logger;
        }

        public async Task<string> SendMessageAsync(MessageModel message)
        {
            string response = string.Empty;
            Exception lastException = null;

            for (int attempt = 0; attempt < _maxRetries; attempt++)
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

                    return response.Replace("<|SUCCESS|>", "");
                }
                catch (Exception ex) when (
                    (ex is SocketException socketEx &&
                        (socketEx.NativeErrorCode.Equals(10053) || socketEx.NativeErrorCode.Equals(10054))))
                {
                    lastException = ex;

                    if (attempt < _maxRetries - 1)
                    {
                        _logger.LogWarning($"Failed to connect to mGBA with attempt: {attempt} with message: [{message}]. Retrying... If this message doesn't appear again, the message succeeded.");
                        // Reset connection state
                        _tcpSocket.Disconnect(reuseSocket: true);
                        _isConnected = false;
                        var retryDelay = _initialRetryDelay * (attempt + 1);
                        await Task.Delay(retryDelay);
                    }
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    throw;
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }
            }

            throw lastException ?? new TimeoutException($"Failed to send message after {_maxRetries} attempts: {message}");
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
