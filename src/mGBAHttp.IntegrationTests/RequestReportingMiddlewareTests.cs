using mGBAHttp.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace mGBAHttp.IntegrationTests
{
    [TestClass]
    public sealed class RequestReportingMiddlewareTests
    {
        private static DefaultHttpContext Context(string path)
        {
            var context = new DefaultHttpContext();
            context.Request.Method = "POST";
            context.Request.Path = path;
            context.Response.Body = new MemoryStream();
            return context;
        }

        private static RequestReportingMiddleware Middleware(RequestDelegate next) =>
            new(next, new ConsoleReporter(Options.Create(new ConsoleOptions { Detailed = false })));

        private static async Task<string> Capture(Func<Task> run)
        {
            var writer = new StringWriter();
            var original = Console.Out;

            Console.SetOut(writer);
            try
            {
                await run();
            }
            finally
            {
                Console.SetOut(original);
            }

            return writer.ToString();
        }

        private static async Task<string> ReadBody(HttpContext context)
        {
            context.Response.Body.Position = 0;
            return await new StreamReader(context.Response.Body).ReadToEndAsync();
        }

        [TestMethod]
        [DataRow("/scalar/")]
        [DataRow("/openapi/v1.json")]
        public async Task DocsRequests_areNotReported(string path)
        {
            var context = Context(path);
            var output = await Capture(() => Middleware(_ => Task.CompletedTask).InvokeAsync(context));

            Assert.AreEqual(string.Empty, output);
        }

        [TestMethod]
        public async Task Request_reportsIncomingAndOutgoing()
        {
            var context = Context("/core/getgametitle");
            var output = await Capture(() => Middleware(c => c.Response.WriteAsync("(BIOS)")).InvokeAsync(context));

            StringAssert.Contains(output, "Incoming POST request to: /core/getgametitle");
            StringAssert.Contains(output, "Outgoing response from /core/getgametitle: HTTP 200 (BIOS)");
        }

        [TestMethod]
        public async Task Response_reachesClientUnchanged()
        {
            var context = Context("/core/getgametitle");
            await Capture(() => Middleware(c => c.Response.WriteAsync("(BIOS)")).InvokeAsync(context));

            Assert.AreEqual("(BIOS)", await ReadBody(context));
        }

        [TestMethod]
        public async Task FailureStatus_isReportedAsError()
        {
            var context = Context("/core/addkey");
            var output = await Capture(() => Middleware(c =>
            {
                c.Response.StatusCode = 400;
                return c.Response.WriteAsync("bad param");
            }).InvokeAsync(context));

            StringAssert.Contains(output, "[ERR]");
            StringAssert.Contains(output, "HTTP 400 bad param");
        }

        [TestMethod]
        public async Task EscapedException_isReportedAndRethrown()
        {
            var context = Context("/core/addkey");
            var middleware = Middleware(_ => throw new InvalidOperationException("boom"));

            var output = await Capture(async () =>
                await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => middleware.InvokeAsync(context)));

            StringAssert.Contains(output, "[ERR]");
            StringAssert.Contains(output, "boom");
        }

        [TestMethod]
        public async Task LongResponse_isTruncated()
        {
            var context = Context("/core/savestatebuffer");
            var body = new string('x', 600);
            var output = await Capture(() => Middleware(c => c.Response.WriteAsync(body)).InvokeAsync(context));

            StringAssert.Contains(output, "(600 chars total)");
            Assert.AreEqual(body, await ReadBody(context));
        }
    }
}
