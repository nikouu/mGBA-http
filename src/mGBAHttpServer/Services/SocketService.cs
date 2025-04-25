using mGBAHttpServer.Models;
using Microsoft.Extensions.Options;
using Microsoft.IO;
using System.Buffers;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace mGBAHttpServer.Services
{
    public class SocketService : IDisposable
    {
        private readonly IPEndPoint _tcpEndPoint;
        private Socket _tcpSocket;
        private const int _maxRetries = 3;
        private readonly TimeSpan _initialRetryDelay = TimeSpan.FromMilliseconds(500);
        private static readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager = new();
        private readonly ILogger<SocketService> _logger;
        private readonly SocketOptions _socketOptions;

        public SocketService(IOptions<SocketOptions> socketOptions, ILogger<SocketService> logger)
        {
            _socketOptions = socketOptions.Value;
            var ipAddress = IPAddress.Parse(_socketOptions.IpAddress);
            _tcpEndPoint = new(ipAddress, _socketOptions.Port);
            _tcpSocket = CreateSocket();
            _logger = logger;
        }

        private Socket CreateSocket()
        {
            return new Socket(_tcpEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
            {
                SendTimeout = _socketOptions.WriteTimeout,
                ReceiveTimeout = _socketOptions.ReadTimeout
            };
        }

        public async Task<string> SendMessageAsync(MessageModel message)
        {
            var startTime = Stopwatch.GetTimestamp();

            string response = string.Empty;
            Exception lastException = null;

            for (int attempt = 0; attempt < _maxRetries; attempt++)
            {
                var buffer = ArrayPool<byte>.Shared.Rent(1024);
                try
                {
                    if (!_tcpSocket.Connected)
                    {                        
                        // Cleanup old socket
                        if (_tcpSocket != null)
                        {
                            try
                            {
                                if (_tcpSocket.Connected)
                                {
                                    _tcpSocket.Shutdown(SocketShutdown.Both);
                                    _tcpSocket.Close();
                                }
                                _tcpSocket.Dispose();
                            }
                            catch
                            {
                                // Ignore cleanup errors
                            }
                        }

                        // Create fresh socket
                        _tcpSocket = CreateSocket();
                        
                        await _tcpSocket.ConnectAsync(_tcpEndPoint);
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
                    Console.WriteLine(response);
                    return response.Replace("<|SUCCESS|>", "");
                }
                catch (Exception ex) when (
                    ex is SocketException socketEx &&
                    (socketEx.NativeErrorCode.Equals(10022) ||
                     socketEx.NativeErrorCode.Equals(10053) ||
                     socketEx.NativeErrorCode.Equals(10054) ||
                     socketEx.NativeErrorCode.Equals(10061)))
                {
                    lastException = ex;

                    if (attempt < _maxRetries - 1)
                    {
                        _logger.LogWarning($"Failed to connect to mGBA with attempt: {attempt} with message: [{message}]. Retrying... If this message doesn't appear again, the message succeeded.");

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
                    var diff = Stopwatch.GetElapsedTime(startTime);
                    Console.WriteLine(diff);
                }
            }
            
            throw lastException ?? new TimeoutException($"Failed to send message after {_maxRetries} attempts: {message}");
        }

        public void Dispose()
        {
            if (_tcpSocket != null)
            {
                try
                {
                    if (_tcpSocket.Connected)
                    {
                        _tcpSocket.Shutdown(SocketShutdown.Both);
                        _tcpSocket.Close();
                    }
                }
                catch
                {
                    // Ignore disposal errors
                }
                finally
                {
                    _tcpSocket.Dispose();
                }
            }
            GC.SuppressFinalize(this);
        }
    }
}
