using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text.Json;

namespace mGBAHttp.IntegrationTests
{
    /// <summary>
    /// Integration tests for the CoreAdapterApi endpoints.
    /// Note: These tests require a mGBA instance with the Lua script running against the button test ROM
    /// </summary>
    [TestClass]
    public sealed class CoreAdapterApiTests : IDisposable
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public CoreAdapterApiTests()
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }

        // Running the tests parallel is difficult because of this test
        [TestMethod]
        [Ignore]
        public async Task ResetEndpoint_SendsRequestSuccessfully()
        {
            // Act
            var response = await _client.PostAsync("/coreadapter/reset", null);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("", responseContent);

            Thread.Sleep(1000); // give space for the ROM to finish resetting for the next tests
        }

        [TestMethod]
        public async Task MemoryEndpoint_ReturnsMemoryDomains()
        {
            // Act
            var response = await _client.GetAsync("/coreadapter/memory");
            var responseContent = await response.Content.ReadAsStringAsync();
            var domains = responseContent.Trim('[', ']')
                .Split(',')
                .Select(d => d.Trim().Trim('"'))
                .ToArray();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var valuesLength = responseContent.Split(',').Length;

            Assert.IsNotNull(domains);
            Assert.AreEqual(10, domains.Length);
        }

        public void Dispose()
        {
            _client.Dispose();
            _factory.Dispose();
        }
    }
}
