using mGBAHttp.Models;
using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace mGBAHttp.Domain
{
    public class ReusableSocket : IDisposable
    {
        private Socket _socket;
        private bool _responseStarted;
        private readonly IPEndPoint _ipEndpoint;
        private readonly int _readTimeout;
        private readonly int _writeTimeout;
        private readonly int _maxAttempts;
        private readonly int _retryDelay;
        private const string _terminationString = "<|END|>";
        private static readonly byte[] _terminationBytes = Encoding.UTF8.GetBytes(_terminationString);

        public ReusableSocket(SocketOptions options)
            : this(options, maxAttempts: 3, retryDelay: 200)
        {
        }

        internal ReusableSocket(SocketOptions options, int maxAttempts, int retryDelay)
        {
            var address = IPAddress.Parse(options.IpAddress);
            _ipEndpoint = new IPEndPoint(address, options.Port);
            _readTimeout = options.ReadTimeout;
            _writeTimeout = options.WriteTimeout;
            _maxAttempts = maxAttempts;
            _retryDelay = retryDelay;
            _socket = new Socket(_ipEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        public async Task<string> SendMessageAsync(string message)
        {
            var attempts = 0;

            while (true)
            {
                attempts++;
                var requestSent = false;
                _responseStarted = false;

                try
                {
                    await ConnectAndSendAsync(message);
                    requestSent = true;

                    var response = await ReadAsync();

                    if (response.Contains("<|ERROR|>"))
                    {
                        throw new MgbaException("Error executing command. See mGBA scripting window for details.");
                    }

                    return response.Replace("<|SUCCESS|>", "");
                }
                catch (MgbaException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    // The socket may hold a half-written request or half-read response, so
                    // it must not be reused as-is by the next attempt or the pool.
                    RecreateSocket();

                    if (attempts >= _maxAttempts || ShouldGiveUp(ex, requestSent, _responseStarted, attempts))
                    {
                        throw;
                    }

                    await Task.Delay(_retryDelay);
                }
            }
        }

        private void RecreateSocket()
        {
            try
            {
                _socket?.Dispose();
            }
            catch { }

            _socket = new Socket(_ipEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        // Give up when mGBA may already have run the command, or when a resend cannot help.
        internal static bool ShouldGiveUp(Exception exception, bool requestFullySent, bool responseStarted, int attempts) =>
            exception switch
            {
                // Usually nothing is listening, but a full listen backlog also refuses, so allow one retry.
                SocketException { SocketErrorCode: SocketError.ConnectionRefused } => attempts > 1,
                // The socket died after a reply started, so the command may have run and cannot be safely repeated.
                SocketException => requestFullySent && responseStarted,
                // Timeouts and anything else: once it was sent, the command may have run.
                _ => requestFullySent,
            };

        private async Task ConnectAndSendAsync(string message)
        {
            using var cts = new CancellationTokenSource(_writeTimeout);
            try
            {
                if (!_socket.Connected)
                {
                    await _socket.ConnectAsync(_ipEndpoint, cts.Token);
                }

                var messageBytes = Encoding.UTF8.GetBytes(message + _terminationString);
                var totalSent = 0;
                while (totalSent < messageBytes.Length)
                {
                    totalSent += await _socket.SendAsync(messageBytes.AsMemory(totalSent), SocketFlags.None, cts.Token);
                }
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException($"Could not reach mGBA within {_writeTimeout}ms. Is mGBA running with the Lua script loaded?");
            }
        }

        private async Task<string> ReadAsync()
        {
            using var cts = new CancellationTokenSource(_readTimeout);
            var buffer = ArrayPool<byte>.Shared.Rent(1024);
            var totalBytes = 0;

            try
            {
                while (true)
                {
                    if (totalBytes == buffer.Length)
                    {
                        var largerBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length * 2);
                        buffer.AsSpan(0, totalBytes).CopyTo(largerBuffer);
                        ArrayPool<byte>.Shared.Return(buffer);
                        buffer = largerBuffer;
                    }

                    int bytesRead;
                    try
                    {
                        bytesRead = await _socket.ReceiveAsync(buffer.AsMemory(totalBytes), SocketFlags.None, cts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        throw new TimeoutException($"No response from mGBA within {_readTimeout}ms. Is mGBA running with the Lua script loaded?");
                    }

                    if (bytesRead == 0)
                    {
                        throw new SocketException((int)SocketError.Disconnecting);
                    }

                    _responseStarted = true;
                    totalBytes += bytesRead;

                    int markerIndex = buffer.AsSpan(0, totalBytes).IndexOf(_terminationBytes);
                    if (markerIndex >= 0)
                    {
                        return Encoding.UTF8.GetString(buffer.AsSpan(0, markerIndex));
                    }
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        public void Dispose()
        {
            _socket?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
