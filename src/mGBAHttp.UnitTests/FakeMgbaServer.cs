using System.Net;
using System.Net.Sockets;
using System.Text;

namespace mGBAHttp.UnitTests
{
    internal sealed class FakeMgbaServer : IAsyncDisposable
    {
        private const string Termination = "<|END|>";

        private readonly TcpListener _listener;
        private readonly Func<string, string?> _handler;
        private readonly CancellationTokenSource _cts = new();
        private readonly List<string> _received = [];
        private readonly Task _acceptLoop;
        private int _connections;

        private FakeMgbaServer(Func<string, string?> handler)
        {
            _handler = handler;
            _listener = new TcpListener(IPAddress.Loopback, 0);
            _listener.Start();
            _acceptLoop = AcceptAsync();
        }

        public static FakeMgbaServer Start(Func<string, string?> handler) => new(handler);

        public static FakeMgbaServer Replying(string reply) => new(_ => reply);

        public int Port => ((IPEndPoint)_listener.LocalEndpoint).Port;

        public int Connections => Volatile.Read(ref _connections);

        public TimeSpan ReplyDelay { get; set; }

        public int ReplyChunkSize { get; set; }

        public IReadOnlyList<string> Received
        {
            get
            {
                lock (_received)
                {
                    return _received.ToArray();
                }
            }
        }

        private async Task AcceptAsync()
        {
            while (!_cts.IsCancellationRequested)
            {
                Socket client;
                try
                {
                    client = await _listener.AcceptSocketAsync(_cts.Token);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (SocketException)
                {
                    return;
                }

                Interlocked.Increment(ref _connections);
                _ = ServeAsync(client);
            }
        }

        private async Task ServeAsync(Socket client)
        {
            using (client)
            {
                var buffer = new byte[4096];
                var pending = new StringBuilder();

                while (!_cts.IsCancellationRequested)
                {
                    int read;
                    try
                    {
                        read = await client.ReceiveAsync(buffer, SocketFlags.None, _cts.Token);
                    }
                    catch
                    {
                        return;
                    }

                    if (read == 0)
                    {
                        return;
                    }

                    pending.Append(Encoding.UTF8.GetString(buffer, 0, read));

                    while (Split(pending, out var message))
                    {
                        lock (_received)
                        {
                            _received.Add(message);
                        }

                        var reply = _handler(message);
                        if (reply is null)
                        {
                            return;
                        }

                        if (ReplyDelay > TimeSpan.Zero)
                        {
                            await Task.Delay(ReplyDelay, _cts.Token);
                        }

                        try
                        {
                            await SendAsync(client, reply + Termination);
                        }
                        catch
                        {
                            return;
                        }
                    }
                }
            }
        }

        private static bool Split(StringBuilder pending, out string message)
        {
            var text = pending.ToString();
            var index = text.IndexOf(Termination, StringComparison.Ordinal);

            if (index < 0)
            {
                message = string.Empty;
                return false;
            }

            message = text[..index];
            pending.Clear();
            pending.Append(text[(index + Termination.Length)..]);
            return true;
        }

        private async Task SendAsync(Socket client, string payload)
        {
            var bytes = Encoding.UTF8.GetBytes(payload);
            var chunk = ReplyChunkSize > 0 ? ReplyChunkSize : bytes.Length;

            for (var sent = 0; sent < bytes.Length;)
            {
                var size = Math.Min(chunk, bytes.Length - sent);
                sent += await client.SendAsync(bytes.AsMemory(sent, size), SocketFlags.None, _cts.Token);

                if (ReplyChunkSize > 0 && sent < bytes.Length)
                {
                    await Task.Delay(10, _cts.Token);
                }
            }
        }

        public async ValueTask DisposeAsync()
        {
            await _cts.CancelAsync();
            _listener.Stop();

            try
            {
                await _acceptLoop;
            }
            catch
            {
            }

            _cts.Dispose();
        }
    }
}
