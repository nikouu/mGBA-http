using Microsoft.AspNetCore.Mvc.Testing;
using mGBAHttpServer.Models;
using System.Net;

namespace mGBAHttpServer.IntegrationTests
{
    /// <summary>
    /// Integration tests for the ButtonApi endpoints.
    /// 
    /// Note: These tests require a mGBA instance with the Lua script running
    /// </summary>
    [TestClass]
    public sealed class ButtonApiTests : IDisposable
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public ButtonApiTests()
        {
            _factory = new WebApplicationFactory<Program>();

            _client = _factory.CreateClient();
        }

        [DataTestMethod]
        [DataRow(KeysEnum.A, DisplayName = "Key A")]
        [DataRow(KeysEnum.B, DisplayName = "Key B")]
        [DataRow(KeysEnum.Select, DisplayName = "Key Select")]
        [DataRow(KeysEnum.Start, DisplayName = "Key Start")]
        [DataRow(KeysEnum.Right, DisplayName = "Key Right")]
        [DataRow(KeysEnum.Left, DisplayName = "Key Left")]
        [DataRow(KeysEnum.Up, DisplayName = "Key Up")]
        [DataRow(KeysEnum.Down, DisplayName = "Key Down")]
        [DataRow(KeysEnum.R, DisplayName = "Key R")]
        [DataRow(KeysEnum.L, DisplayName = "Key L")]
        public async Task TapEndpoint_SendsRequestSuccessfully(KeysEnum key)
        {            
            // Act
            var response = await _client.PostAsync($"/mgba-http/button/tap?key={key}", null);

            Thread.Sleep(100);
            
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [DataTestMethod]
        [DataRow(new[] { KeysEnum.A }, DisplayName = "Key A")]
        [DataRow(new[] { KeysEnum.B }, DisplayName = "Key B")]
        [DataRow(new[] { KeysEnum.A, KeysEnum.B }, DisplayName = "Keys A+B")]
        [DataRow(new[] { KeysEnum.Up, KeysEnum.A }, DisplayName = "Keys Up+A")]
        [DataRow(new[] { KeysEnum.Left, KeysEnum.Right }, DisplayName = "Keys Left+Right")]
        [DataRow(new[] { KeysEnum.Up, KeysEnum.Down, KeysEnum.Left, KeysEnum.Right }, DisplayName = "Keys Up+Down+Left+Right")]
        public async Task TapManyEndpoint_SendsRequestSuccessfully(KeysEnum[] keys)
        {
            // Act
            var queryString = string.Join("&", keys.Select(k => $"keys={k}"));
            var response = await _client.PostAsync($"/mgba-http/button/tapmany?{queryString}", null);

            Thread.Sleep(100);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [DataTestMethod]
        [DataRow(KeysEnum.A, 5, DisplayName = "Key A - 5 frames")]
        [DataRow(KeysEnum.B, 10, DisplayName = "Key B - 10 frames")]
        [DataRow(KeysEnum.Select, 15, DisplayName = "Key Select - 15 frames")]
        [DataRow(KeysEnum.Start, 20, DisplayName = "Key Start - 20 frames")]
        [DataRow(KeysEnum.Right, 5, DisplayName = "Key Right - 5 frames")]
        public async Task HoldEndpoint_SendsRequestSuccessfully(KeysEnum key, int duration)
        {
            // Act
            var response = await _client.PostAsync($"/mgba-http/button/hold?key={key}&duration={duration}", null);

            Thread.Sleep(100);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [DataTestMethod]
        [DataRow(new[] { KeysEnum.A }, 50, DisplayName = "Key A - 5 frames")]
        [DataRow(new[] { KeysEnum.A, KeysEnum.B }, 10, DisplayName = "Keys A+B - 10 frames")]
        [DataRow(new[] { KeysEnum.Up, KeysEnum.B }, 15, DisplayName = "Keys Up+B - 15 frames")]
        [DataRow(new[] { KeysEnum.Left, KeysEnum.Right }, 50, DisplayName = "Keys Left+Right - 5 frames")]
        [DataRow(new[] { KeysEnum.L, KeysEnum.Down, KeysEnum.A }, 10, DisplayName = "Keys Up+Down+A - 10 frames")]
        public async Task HoldManyEndpoint_SendsRequestSuccessfully(KeysEnum[] keys, int duration)
        {
            // Act
            var queryString = string.Join("&", keys.Select(k => $"keys={k}")) + $"&duration={duration}";
            var response = await _client.PostAsync($"/mgba-http/button/holdmany?{queryString}", null);

            Thread.Sleep(100);

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
