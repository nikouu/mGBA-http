using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace mGBAHttpServer.IntegrationTests
{
    /// <summary>
    /// Integration tests for the ConsoleApi endpoints.
    /// Note: These tests require a mGBA instance with the Lua script running against the button test ROM 
    /// </summary>
    [TestClass]
    public sealed class ConsoleApiTests : IDisposable
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public ConsoleApiTests()
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }

        [DataTestMethod]
        [DataRow("Test error message", DisplayName = "Error message")]
        [DataRow("Error with special chars: !@#$%^", DisplayName = "Error with special characters")]
        public async Task ErrorEndpoint_SendsRequestSuccessfully(string message)
        {
            // Act
            var response = await _client.PostAsync($"/console/error?message={Uri.EscapeDataString(message)}", null);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [DataTestMethod]
        [DataRow("Test log message", DisplayName = "Log message")]
        [DataRow("Log with special chars: !@#$%^", DisplayName = "Log with special characters")]
        public async Task LogEndpoint_SendsRequestSuccessfully(string message)
        {
            // Act
            var response = await _client.PostAsync($"/console/log?message={Uri.EscapeDataString(message)}", null);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [DataTestMethod]
        [DataRow("Test warning message", DisplayName = "Warning message")]
        [DataRow("Warning with special chars: !@#$%^", DisplayName = "Warning with special characters")]
        public async Task WarnEndpoint_SendsRequestSuccessfully(string message)
        {
            // Act
            var response = await _client.PostAsync($"/console/warn?message={Uri.EscapeDataString(message)}", null);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        public void Dispose()
        {
            _client.Dispose();
            _factory.Dispose();
        }
    }
}
