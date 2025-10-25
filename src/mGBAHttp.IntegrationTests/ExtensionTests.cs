using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace mGBAHttp.IntegrationTests
{
    /// <summary>
    /// Integration tests for the Extension endpoints.
    /// Note: These tests require a mGBA instance with the Lua script running against the button test ROM
    /// </summary>
    [TestClass]
    public class ExtensionTests : IDisposable
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private static readonly string RomDirectory = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\..\")); // root of the repo
        private readonly string TestRomPath = Path.Combine(RomDirectory, "BtnTest.gba");
        private readonly string TestRomPath2 = Path.Combine(RomDirectory, "POKEMON FIRE.gba");

        public ExtensionTests()
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }

        [TestMethod]
        public async Task LoadFile_ROMLoadsSuccessfully()
        {
            // Act
            var response = await _client.PostAsync($"/mgba-http/extension/loadfile?path={TestRomPath2}", null);
            var responseContent = await response.Content.ReadAsStringAsync();
            var titleResponse = await _client.GetAsync("/core/getgametitle");
            var titleResponseContent = await titleResponse.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("true", responseContent);
            Assert.AreEqual("POKEMON FIRE", titleResponseContent);

            // Cleanup
            await _client.PostAsync($"/mgba-http/extension/loadfile?path={TestRomPath}", null);
        }

        public void Dispose()
        {
            _client.Dispose();
            _factory.Dispose();
        }
    }
}
