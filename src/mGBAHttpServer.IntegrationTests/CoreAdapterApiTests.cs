using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text.Json;

namespace mGBAHttpServer.IntegrationTests
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
        public async Task ResetEndpoint_SendsRequestSuccessfully()
        {
            // Act
            var response = await _client.PostAsync("/coreadapter/reset", null);
            
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task MemoryEndpoint_ReturnsMemoryDomains()
        {
            // Act
            var response = await _client.GetAsync("/coreadapter/memory");
            
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            
            var content = await response.Content.ReadAsStringAsync();
            var memoryDomains = JsonSerializer.Deserialize<string[]>(content);
            
            Assert.IsNotNull(memoryDomains);
            Assert.AreEqual(10, memoryDomains.Length);
        }

        public void Dispose()
        {
            _client.Dispose();
            _factory.Dispose();
        }
    }
}
