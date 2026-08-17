using Microsoft.AspNetCore.Http;
using System.Net.Sockets;

namespace mGBAHttp.UnitTests
{
    [TestClass]
    public sealed class MgbaExceptionHandlerTests
    {
        private static async Task<(bool handled, int status, string body)> Handle(Exception exception)
        {
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            var handled = await new MgbaExceptionHandler().TryHandleAsync(context, exception, default);

            context.Response.Body.Position = 0;
            var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
            return (handled, context.Response.StatusCode, body);
        }

        [TestMethod]
        public async Task MgbaException_Maps502AndPassesMessage()
        {
            var (handled, status, body) = await Handle(new MgbaException("scripting error"));
            Assert.IsTrue(handled);
            Assert.AreEqual(502, status);
            Assert.AreEqual("scripting error", body);
        }

        [TestMethod]
        public async Task Timeout_Maps504()
        {
            var (handled, status, _) = await Handle(new TimeoutException());
            Assert.IsTrue(handled);
            Assert.AreEqual(504, status);
        }

        [TestMethod]
        public async Task SocketException_Maps502()
        {
            var (handled, status, _) = await Handle(new SocketException());
            Assert.IsTrue(handled);
            Assert.AreEqual(502, status);
        }

        [TestMethod]
        public async Task BadHttpRequest_Maps400AndPassesMessage()
        {
            var (handled, status, body) = await Handle(new BadHttpRequestException("bad param", 400));
            Assert.IsTrue(handled);
            Assert.AreEqual(400, status);
            Assert.AreEqual("bad param", body);
        }

        [TestMethod]
        public async Task UnknownException_Maps500AndPassesMessage()
        {
            var (handled, status, body) = await Handle(new InvalidOperationException("boom"));
            Assert.IsTrue(handled);
            Assert.AreEqual(500, status);
            Assert.AreEqual("boom", body);
        }
    }
}
