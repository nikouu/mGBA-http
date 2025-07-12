using Microsoft.AspNetCore.Mvc.Testing;

namespace mGBAHttpServer.IntegrationTests
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
            await _client.PostAsync($"/mgba-http/extension/loadfile?path={TestRomPath2}", null);
            var response = await _client.GetStringAsync("/core/getgametitle");

            // Assert
            Assert.AreEqual("POKEMON FIRE", response);

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
