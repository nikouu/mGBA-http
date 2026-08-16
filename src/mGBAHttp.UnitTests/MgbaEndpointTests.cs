using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace mGBAHttp.UnitTests
{
    [TestClass]
    public sealed class MgbaEndpointTests
    {
        private static WebApplicationFactory<Program> AppUsing(FakeMgbaServer server, int readTimeout = 2000) =>
            new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.UseSetting("mgba-http:Socket:Port", server.Port.ToString());
                builder.UseSetting("mgba-http:Socket:ReadTimeout", readTimeout.ToString());
            });

        [TestMethod]
        public async Task Get_returnsValueFromMgba()
        {
            await using var server = FakeMgbaServer.Replying("POKEMON FIRE");
            using var app = AppUsing(server);
            using var client = app.CreateClient();

            var response = await client.GetAsync("/core/getgametitle");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("POKEMON FIRE", await response.Content.ReadAsStringAsync());
            CollectionAssert.AreEqual(new[] { "core.getGameTitle,,," }, server.Received.ToArray());
        }

        [TestMethod]
        public async Task Post_sendsParametersToMgba()
        {
            await using var server = FakeMgbaServer.Replying("<|SUCCESS|>");
            using var app = AppUsing(server);
            using var client = app.CreateClient();

            var response = await client.PostAsync("/core/addkey?key=5", null);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            CollectionAssert.AreEqual(new[] { "core.addKey,5,," }, server.Received.ToArray());
        }

        [TestMethod]
        public async Task WhenMgbaReportsError_returns502()
        {
            await using var server = FakeMgbaServer.Replying("<|ERROR|>");
            using var app = AppUsing(server);
            using var client = app.CreateClient();

            var response = await client.GetAsync("/core/getgametitle");

            Assert.AreEqual(HttpStatusCode.BadGateway, response.StatusCode);
        }

        [TestMethod]
        public async Task WhenMgbaStalls_returns504()
        {
            await using var server = FakeMgbaServer.Replying("too late");
            server.ReplyDelay = TimeSpan.FromMilliseconds(500);
            using var app = AppUsing(server, readTimeout: 100);
            using var client = app.CreateClient();

            var response = await client.GetAsync("/core/getgametitle");

            Assert.AreEqual(HttpStatusCode.GatewayTimeout, response.StatusCode);
        }
    }
}
