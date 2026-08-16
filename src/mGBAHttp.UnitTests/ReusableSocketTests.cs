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
        public void ConnectionRefused_givesUp() =>
            Assert.IsTrue(ReusableSocket.ShouldGiveUp(Refused(), requestFullySent: false, responseStarted: false));

        [TestMethod]
        public void RequestNeverFullySent_retries()
        {
            Assert.IsFalse(ReusableSocket.ShouldGiveUp(Dropped(), requestFullySent: false, responseStarted: false));
            Assert.IsFalse(ReusableSocket.ShouldGiveUp(new TimeoutException(), requestFullySent: false, responseStarted: false));
        }

        [TestMethod]
        public void SocketDiedBeforeAnyReply_retries() =>
            Assert.IsFalse(ReusableSocket.ShouldGiveUp(Dropped(), requestFullySent: true, responseStarted: false));

        [TestMethod]
        public void SocketDiedAfterReplyStarted_givesUp() =>
            Assert.IsTrue(ReusableSocket.ShouldGiveUp(Dropped(), requestFullySent: true, responseStarted: true));

        [TestMethod]
        public void TimeoutAfterSending_givesUp()
        {
            Assert.IsTrue(ReusableSocket.ShouldGiveUp(new TimeoutException(), requestFullySent: true, responseStarted: false));
            Assert.IsTrue(ReusableSocket.ShouldGiveUp(new TimeoutException(), requestFullySent: true, responseStarted: true));
        }
    }
}
