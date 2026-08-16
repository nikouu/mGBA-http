using mGBAHttp.Domain;
using mGBAHttp.Models;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace mGBAHttp.UnitTests
{
    [TestClass]
    public sealed class ReusableSocketProtocolTests
    {
        private static int ClosedPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        private static ReusableSocket Connect(FakeMgbaServer server, int readTimeout = 2000) =>
            new(
                new SocketOptions { IpAddress = "127.0.0.1", Port = server.Port, ReadTimeout = readTimeout, WriteTimeout = 2000 },
                maxAttempts: 3,
                initialDelay: 1,
                maxDelay: 2);

        [TestMethod]
        public async Task SendMessage_returnsServerReply()
        {
            await using var server = FakeMgbaServer.Replying("POKEMON FIRE");
            using var socket = Connect(server);

            Assert.AreEqual("POKEMON FIRE", await socket.SendMessageAsync("core.getGameTitle,,,"));
        }

        [TestMethod]
        public async Task SendMessage_appendsTerminatorAndSendsMessage()
        {
            await using var server = FakeMgbaServer.Replying("ok");
            using var socket = Connect(server);

            await socket.SendMessageAsync("core.addKey,5,,");

            CollectionAssert.AreEqual(new[] { "core.addKey,5,," }, server.Received.ToArray());
        }

        [TestMethod]
        public async Task SendMessage_stripsSuccessMarker()
        {
            await using var server = FakeMgbaServer.Replying("<|SUCCESS|>");
            using var socket = Connect(server);

            Assert.AreEqual(string.Empty, await socket.SendMessageAsync("core.clearKeys,,,"));
        }

        [TestMethod]
        public async Task SendMessage_errorMarkerThrowsMgbaException()
        {
            await using var server = FakeMgbaServer.Replying("<|ERROR|>");
            using var socket = Connect(server);

            await Assert.ThrowsExactlyAsync<MgbaException>(() => socket.SendMessageAsync("core.addKey,5,,"));
        }

        [TestMethod]
        public async Task SendMessage_readsReplyLargerThanInitialBuffer()
        {
            var reply = new string('x', 5000);
            await using var server = FakeMgbaServer.Replying(reply);
            using var socket = Connect(server);

            Assert.AreEqual(reply, await socket.SendMessageAsync("memoryDomain.readRange,wram,0,5000"));
        }

        [TestMethod]
        public async Task SendMessage_readsReplySplitAcrossChunks()
        {
            await using var server = FakeMgbaServer.Replying("POKEMON FIRE");
            server.ReplyChunkSize = 3;
            using var socket = Connect(server);

            Assert.AreEqual("POKEMON FIRE", await socket.SendMessageAsync("core.getGameTitle,,,"));
        }

        [TestMethod]
        public async Task SendMessage_whenServerStalls_throwsTimeout()
        {
            await using var server = FakeMgbaServer.Replying("too late");
            server.ReplyDelay = TimeSpan.FromMilliseconds(500);
            using var socket = Connect(server, readTimeout: 100);

            await Assert.ThrowsExactlyAsync<TimeoutException>(() => socket.SendMessageAsync("core.getGameTitle,,,"));
        }

        [TestMethod]
        public async Task SendMessage_whenServerDropsFirstConnection_retriesAndSucceeds()
        {
            var attempts = 0;
            await using var server = FakeMgbaServer.Start(_ => Interlocked.Increment(ref attempts) == 1 ? null : "recovered");
            using var socket = Connect(server);

            Assert.AreEqual("recovered", await socket.SendMessageAsync("core.getGameTitle,,,"));
            Assert.AreEqual(2, attempts);
        }

        [TestMethod]
        public async Task SendMessage_whenNothingIsListening_doesNotRetry()
        {
            const int retryDelay = 5000;
            using var socket = new ReusableSocket(
                new SocketOptions { IpAddress = "127.0.0.1", Port = ClosedPort(), ReadTimeout = 8000, WriteTimeout = 8000 },
                maxAttempts: 3,
                initialDelay: retryDelay,
                maxDelay: retryDelay);

            var elapsed = Stopwatch.StartNew();
            await Assert.ThrowsExactlyAsync<SocketException>(() => socket.SendMessageAsync("core.getGameTitle,,,"));
            elapsed.Stop();

            Assert.IsTrue(
                elapsed.ElapsedMilliseconds < retryDelay,
                $"Expected no retry delay, took {elapsed.ElapsedMilliseconds}ms");
        }

        [TestMethod]
        public async Task SendMessage_reusesTheSameConnection()
        {
            await using var server = FakeMgbaServer.Replying("ok");
            using var socket = Connect(server);

            await socket.SendMessageAsync("core.getGameTitle,,,");
            await socket.SendMessageAsync("core.getGameTitle,,,");

            Assert.AreEqual(2, server.Received.Count);
            Assert.AreEqual(1, server.Connections);
        }
    }
}
