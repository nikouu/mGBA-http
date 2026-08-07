using mGBAHttp.Domain;

namespace mGBAHttp.IntegrationTests
{
    [TestClass]
    public sealed class ReusableSocketTests
    {
        // fullySent, responseStarted, isSocketException, expectedRetry
        [DataTestMethod]
        [DataRow(false, false, true, true)]    // never fully sent -> safe
        [DataRow(false, false, false, true)]   // never fully sent -> safe
        [DataRow(true, false, true, true)]     // dead socket, no reply -> safe
        [DataRow(true, true, true, false)]     // reply started then died -> may have run
        [DataRow(true, false, false, false)]   // read timeout -> may have run
        [DataRow(true, true, false, false)]    // reply started then errored -> may have run
        public void ShouldRetry_onlyWhenCommandCannotHaveExecuted(
            bool fullySent, bool responseStarted, bool isSocketException, bool expected) =>
            Assert.AreEqual(expected, ReusableSocket.ShouldRetry(fullySent, responseStarted, isSocketException));
    }
}
