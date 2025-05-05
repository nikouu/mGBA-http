using Microsoft.Extensions.ObjectPool;
using Microsoft.IO;
using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace mGBAHttpServer.Domain
{
    public class ReusableSocket : IResettable, IDisposable
    {
        private Socket _socket;
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

            while (attempts < _maxRetries)
            {
                try
                {
                    attempts++;
                    var response = await SendAsync(message);
                    return response.Replace("<|SUCCESS|>", "");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[{attempts}][-]{ex.Message}");
                    if (attempts >= _maxRetries)
                    {
                        throw;
                    }

                    if (ex is SocketException sockEx &&
                        (sockEx.SocketErrorCode
                        is SocketError.ConnectionReset
                        or SocketError.NotConnected
                        or SocketError.IsConnected
                        or SocketError.ConnectionAborted
                        or SocketError.Disconnecting))
                    {
                        RecreateSocket();
                    }

                    await Task.Delay(delay);
                    delay = Math.Min(delay * 3, _maxDelay);
                }
            }

            throw new Exception("How did we get here?");
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

        private async Task<string> SendAsync(string message)
        {
            //Console.WriteLine(_socket.LocalEndPoint);
            if (!_socket.Connected)
            {
                await _socket.ConnectAsync(_ipEndpoint);
                Console.WriteLine(_socket.LocalEndPoint);
            }

            var messageBytes = Encoding.UTF8.GetBytes(message + _terminationString);
            await _socket.SendAsync(messageBytes, SocketFlags.None);

            return await ReadAsync();
        }

        private async Task<string> ReadAsync()
        {
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
