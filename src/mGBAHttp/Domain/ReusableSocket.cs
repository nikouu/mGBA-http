using Microsoft.Extensions.ObjectPool;
using Microsoft.IO;
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
        private const int _maxRetries = 3;
        private const int _initialDelay = 400;
        private const int _maxDelay = 2000;
        private static readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager = new();
        private const string _terminationString = "<|END|>";
        private static readonly byte[] _terminationBytes = Encoding.UTF8.GetBytes(_terminationString);

        public ReusableSocket(string ipAddress, int port)
        {

            var address = IPAddress.Parse(ipAddress);
            _ipEndpoint = new IPEndPoint(address, port);
            _socket = new Socket(_ipEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect()
        {
            if (!_socket.Connected)
            {
                _socket.Connect(_ipEndpoint);
            }
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
            if (!_socket.Connected)
            {
                await _socket.ConnectAsync(_ipEndpoint);
            }

            var messageBytes = Encoding.UTF8.GetBytes(message + _terminationString);
            var totalSent = 0;
            while (totalSent < messageBytes.Length)
            {
                totalSent += await _socket.SendAsync(messageBytes.AsMemory(totalSent), SocketFlags.None);
            }
        }

        private async Task<string> ReadAsync()
        {
            _responseStarted = false;
            var buffer = ArrayPool<byte>.Shared.Rent(1024);
            try
            {
                using var memoryStream = _recyclableMemoryStreamManager.GetStream();
                int totalBytes = 0;

                while (true)
                {
                    var bytesRead = await _socket.ReceiveAsync(buffer, SocketFlags.None);
                    if (bytesRead == 0)
                    {
                        throw new SocketException((int)SocketError.Disconnecting);
                    }

                    _responseStarted = true;
                    await memoryStream.WriteAsync(buffer.AsMemory(0, bytesRead));
                    totalBytes += bytesRead;

                    // Check for termination marker in the accumulated buffer
                    var mem = memoryStream.GetBuffer().AsSpan(0, totalBytes);
                    int markerIndex = mem.IndexOf(_terminationBytes);
                    if (markerIndex >= 0)
                    {
                        // Found marker, extract message up to marker
                        var messageBytes = mem.Slice(0, markerIndex);
                        var response = Encoding.UTF8.GetString(messageBytes);
                        return response;
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
