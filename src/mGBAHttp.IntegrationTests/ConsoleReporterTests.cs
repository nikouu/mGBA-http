using mGBAHttp.Logging;
using Microsoft.Extensions.Options;

namespace mGBAHttp.IntegrationTests
{
    [TestClass]
    public sealed class ConsoleReporterTests
    {
        private static string Capture(bool detailed, Action<ConsoleReporter> report)
        {
            var reporter = new ConsoleReporter(Options.Create(new ConsoleOptions { Detailed = detailed }));
            var writer = new StringWriter();
            var original = Console.Out;

            Console.SetOut(writer);
            try
            {
                report(reporter);
            }
            finally
            {
                Console.SetOut(original);
            }

            return writer.ToString();
        }

        [TestMethod]
        public void RequestIn_writesMethodAndPath()
        {
            var output = Capture(false, r => r.RequestIn("POST", "/core/addkey?key=5", "abc"));

            StringAssert.Contains(output, "[INF]");
            StringAssert.Contains(output, "Incoming POST request to: /core/addkey?key=5");
        }

        [TestMethod]
        [DataRow(200, "[INF]")]
        [DataRow(204, "[INF]")]
        [DataRow(400, "[ERR]")]
        [DataRow(502, "[ERR]")]
        public void RequestOut_levelFollowsStatus(int status, string expectedLevel)
        {
            var output = Capture(false, r => r.RequestOut("/core/addkey", status, "", 1, "abc"));

            StringAssert.Contains(output, expectedLevel);
            StringAssert.Contains(output, $"HTTP {status}");
        }

        [TestMethod]
        public void RequestOut_writesResponseText()
        {
            var output = Capture(false, r => r.RequestOut("/core/getgametitle", 200, "(BIOS)", 1, "abc"));

            StringAssert.Contains(output, "Outgoing response from /core/getgametitle: HTTP 200 (BIOS)");
        }

        [TestMethod]
        public void RequestOut_emptyResponseEndsAtStatus()
        {
            var output = Capture(false, r => r.RequestOut("/mgba-http/button/tap", 200, "", 1, "abc"));

            StringAssert.EndsWith(output.TrimEnd(), "HTTP 200");
        }

        [TestMethod]
        public void RequestOut_withoutDetailed_omitsCorrelationId()
        {
            var output = Capture(false, r => r.RequestOut("/core/addkey", 200, "", 12, "abc"));

            Assert.IsFalse(output.Contains("correlationId"));
            Assert.IsFalse(output.Contains("elapsed"));
        }

        [TestMethod]
        public void RequestOut_withDetailed_writesCorrelationIdStatusAndElapsed()
        {
            var output = Capture(true, r => r.RequestOut("/core/addkey", 400, "", 12, "abc"));

            StringAssert.Contains(output, "correlationId=abc");
            StringAssert.Contains(output, "status=400");
            StringAssert.Contains(output, "elapsed=12ms");
        }

        [TestMethod]
        public void RequestIn_withDetailed_writesCorrelationId()
        {
            var output = Capture(true, r => r.RequestIn("POST", "/core/addkey", "abc"));

            StringAssert.Contains(output, "correlationId=abc");
        }

        [TestMethod]
        public void RequestFailed_writesErrorWithExceptionMessage()
        {
            var output = Capture(false, r => r.RequestFailed("/core/addkey", new InvalidOperationException("boom"), 3, "abc"));

            StringAssert.Contains(output, "[ERR]");
            StringAssert.Contains(output, "boom");
        }

        [TestMethod]
        public void Header_writesUrlAndDocLinks()
        {
            var output = Capture(false, r => r.Header("http://localhost:5000"));

            StringAssert.Contains(output, "Listening on http://localhost:5000");
            StringAssert.Contains(output, "/scalar");
            StringAssert.Contains(output, "/openapi/v1.json");
        }
    }
}
