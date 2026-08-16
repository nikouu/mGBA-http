using mGBAHttp.Models;
using Microsoft.Extensions.ObjectPool;
using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace mGBAHttp.Domain
{
    public class ReusableSocket : IResettable, IDisposable
    {
        private Socket _socket;
        private bool _responseStarted;
        private readonly IPEndPoint _ipEndpoint;
        private readonly int _readTimeout;
        private readonly int _writeTimeout;
        private readonly int _maxRetries;
        private readonly int _initialDelay;
        private readonly int _maxDelay;
        private const string _terminationString = "<|END|>";
        private static readonly byte[] _terminationBytes = Encoding.UTF8.GetBytes(_terminationString);

        public ReusableSocket(SocketOptions options)
            : this(options, maxRetries: 3, initialDelay: 400, maxDelay: 2000)
        {
        }

        internal ReusableSocket(SocketOptions options, int maxRetries, int initialDelay, int maxDelay)
        {
            var address = IPAddress.Parse(options.IpAddress);
            _ipEndpoint = new IPEndPoint(address, options.Port);
            _readTimeout = options.ReadTimeout;
            _writeTimeout = options.WriteTimeout;
            _maxRetries = maxRetries;
            _initialDelay = initialDelay;
            _maxDelay = maxDelay;
            _socket = new Socket(_ipEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        public async Task<string> SendMessageAsync(string message)
        {
            var attempts = 0;
            var delay = _initialDelay;

            while (true)
            {
                attempts++;
                var requestSent = false;

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

                    if (!ShouldRetry(requestSent, _responseStarted, ex is SocketException)
                        || attempts >= _maxRetries)
                    {
                        throw;
                    }

                    await Task.Delay(delay);
                    delay = Math.Min(delay * 3, _maxDelay);
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

        // Resend only when mGBA cannot have run the command: it never fully sent,
        // or the socket died before any reply arrived.
        internal static bool ShouldRetry(bool requestFullySent, bool responseStarted, bool isSocketException) =>
            !requestFullySent || (isSocketException && !responseStarted);

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
            _responseStarted = false;
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

        public bool TryReset()
        {
            return true;
        }

        public void Dispose()
        {
            _socket?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
