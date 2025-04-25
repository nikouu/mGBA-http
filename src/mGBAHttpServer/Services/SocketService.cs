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
            var socket = new Socket(_tcpEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
            {
                SendTimeout = _socketOptions.WriteTimeout,
                ReceiveTimeout = _socketOptions.ReadTimeout,
                NoDelay = true // Disable Nagle's algorithm
            };

            // Enable keep-alive to detect broken connections
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveTime, 5);  // 5 seconds before first keepalive
            socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveInterval, 1); // 1 second between keepalives
            socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveRetryCount, 3); // 3 retry attempts

            return socket;
        }

        public async Task<string> SendMessageAsync(MessageModel message)
        {
            var startTime = Stopwatch.GetTimestamp();
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
                        _logger.LogDebug("Socket connected successfully");
                    }

                    // Use CancellationToken to prevent hanging
                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5)); // 5 second timeout

                    var messageBytes = Encoding.UTF8.GetBytes(message.ToString());
                    var bytesSent = await _tcpSocket.SendAsync(messageBytes, SocketFlags.None);
                    _logger.LogDebug("Sent {BytesSent} bytes for message: {Message}", bytesSent, message);

                    using var memoryStream = _recyclableMemoryStreamManager.GetStream();
                    int totalBytesRead = 0;
                    int bytesRead;
                    bool receivedAnyData = false;

                    try
                    {
                        do
                        {
                            bytesRead = await _tcpSocket.ReceiveAsync(buffer, SocketFlags.None)
                                .WaitAsync(cts.Token); // Add timeout to receive operation
                            
                            _logger.LogDebug("Received {BytesRead} bytes", bytesRead);
                            
                            if (bytesRead > 0)
                            {
                                receivedAnyData = true;
                                await memoryStream.WriteAsync(buffer.AsMemory(0, bytesRead));
                                totalBytesRead += bytesRead;
                            }
                            else if (bytesRead == 0 && !receivedAnyData)
                            {
                                _logger.LogWarning("Server closed connection before sending response for message: {Message}", message);
                                throw new SocketException((int)SocketError.ConnectionReset);
                            }
                        } while (bytesRead > 0 && _tcpSocket.Available > 0);
                    }
                    catch (OperationCanceledException)
                    {
                        _logger.LogWarning("Receive operation timed out after 5 seconds for message: {Message}", message);
                        throw new SocketException((int)SocketError.TimedOut);
                    }

                    var response = Encoding.UTF8.GetString(memoryStream.GetReadOnlySequence());
                    
                    if (string.IsNullOrEmpty(response))
                    {
                        _logger.LogInformation(
                            "Empty response received for message {MessageType}. Bytes read: {TotalBytes}", 
                            message.Type, 
                            totalBytesRead
                        );
                    }
                    else
                    {
                        _logger.LogDebug("Response received: {Response}", response);
                    }

                    var diff = Stopwatch.GetElapsedTime(startTime);
                    _logger.LogDebug("Request took: {Duration}", diff);

                    return response.Replace("<|SUCCESS|>", "");
                }
                catch (Exception ex) when (
                    ex is SocketException socketEx &&
                    (socketEx.NativeErrorCode.Equals(10022) || // Invalid argument
                     socketEx.NativeErrorCode.Equals(10053) || // Connection aborted
                     socketEx.NativeErrorCode.Equals(10054) || // Connection reset
                     socketEx.NativeErrorCode.Equals(10060) || // Connection timed out
                     socketEx.NativeErrorCode.Equals(10061)))  // Connection refused
                {
                    lastException = ex;

                    if (attempt < _maxRetries - 1)
                    {
                        _logger.LogWarning(
                            "Socket error {ErrorCode} on attempt {Attempt} for message type {MessageType}. Retrying...", 
                            ((SocketException)ex).NativeErrorCode,
                            attempt + 1,
                            message.Type
                        );

                        // Force socket cleanup on connection errors
                        _tcpSocket.Dispose();
                        _tcpSocket = CreateSocket();

                        var retryDelay = _initialRetryDelay * (attempt + 1);
                        await Task.Delay(retryDelay);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled error for message type {MessageType}", message.Type);
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
