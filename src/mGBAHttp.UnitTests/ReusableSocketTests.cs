using mGBAHttp.Domain;
using System.Net.Sockets;

namespace mGBAHttp.UnitTests
{
    [TestClass]
    public sealed class ReusableSocketTests
    {
        private static SocketException Refused() => new((int)SocketError.ConnectionRefused);

        private static SocketException Dropped() => new((int)SocketError.ConnectionReset);

        [TestMethod]
        public void ConnectionRefused_retriesOnce() =>
            Assert.IsFalse(ReusableSocket.ShouldGiveUp(Refused(), requestFullySent: false, responseStarted: false, attempts: 1));

        [TestMethod]
        public void ConnectionRefused_givesUpAfterTheRetry() =>
            Assert.IsTrue(ReusableSocket.ShouldGiveUp(Refused(), requestFullySent: false, responseStarted: false, attempts: 2));

        [TestMethod]
        public void RequestNeverFullySent_retries()
        {
            Assert.IsFalse(ReusableSocket.ShouldGiveUp(Dropped(), requestFullySent: false, responseStarted: false, attempts: 1));
            Assert.IsFalse(ReusableSocket.ShouldGiveUp(new TimeoutException(), requestFullySent: false, responseStarted: false, attempts: 1));
        }

        [TestMethod]
        public void SocketDiedBeforeAnyReply_retries() =>
            Assert.IsFalse(ReusableSocket.ShouldGiveUp(Dropped(), requestFullySent: true, responseStarted: false, attempts: 1));

        [TestMethod]
        public void SocketDiedAfterReplyStarted_givesUp() =>
            Assert.IsTrue(ReusableSocket.ShouldGiveUp(Dropped(), requestFullySent: true, responseStarted: true, attempts: 1));

        [TestMethod]
        public void TimeoutAfterSending_givesUp()
        {
            Assert.IsTrue(ReusableSocket.ShouldGiveUp(new TimeoutException(), requestFullySent: true, responseStarted: false, attempts: 1));
            Assert.IsTrue(ReusableSocket.ShouldGiveUp(new TimeoutException(), requestFullySent: true, responseStarted: true, attempts: 1));
        }
    }
}
