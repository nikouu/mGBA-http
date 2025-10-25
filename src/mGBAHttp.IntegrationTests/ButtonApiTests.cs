using mGBAHttp.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace mGBAHttp.IntegrationTests
{
    /// <summary>
    /// Integration tests for the ButtonApi endpoints. 
    /// Note: These tests require a mGBA instance with the Lua script running against the button test ROM
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
        [DataRow(ButtonEnum.A, DisplayName = "Add Key A")]
        [DataRow(ButtonEnum.B, DisplayName = "Add Key B")]
        [DataRow(ButtonEnum.Start, DisplayName = "Add Key Start")]
        [DataRow(ButtonEnum.Select, DisplayName = "Add Key Select")]
        [DataRow(ButtonEnum.Right, DisplayName = "Add Key Right")]
        [DataRow(ButtonEnum.Left, DisplayName = "Add Key Left")]
        [DataRow(ButtonEnum.Up, DisplayName = "Add Key Up")]
        [DataRow(ButtonEnum.Down, DisplayName = "Add Key Down")]
        [DataRow(ButtonEnum.R, DisplayName = "Add Key R")]
        [DataRow(ButtonEnum.L, DisplayName = "Add Key L")]
        public async Task Add_SendsRequestSuccessfully(ButtonEnum button)
        {
            // Act
            var response = await _client.PostAsync($"/mgba-http/button/add?button={button}", null);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Reset state
            await _client.PostAsync($"/mgba-http/button/clear?button={button}", null);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("", responseContent);
        }

        [DataTestMethod]
        [DataRow(ButtonEnum.A, DisplayName = "Clear Key A")]
        [DataRow(ButtonEnum.B, DisplayName = "Clear Key B")]
        [DataRow(ButtonEnum.Start, DisplayName = "Clear Key Start")]
        [DataRow(ButtonEnum.Select, DisplayName = "Clear Key Select")]
        [DataRow(ButtonEnum.Right, DisplayName = "Clear Key Right")]
        [DataRow(ButtonEnum.Left, DisplayName = "Clear Key Left")]
        [DataRow(ButtonEnum.Up, DisplayName = "Clear Key Up")]
        [DataRow(ButtonEnum.Down, DisplayName = "Clear Key Down")]
        [DataRow(ButtonEnum.R, DisplayName = "Clear Key R")]
        [DataRow(ButtonEnum.L, DisplayName = "Clear Key L")]
        public async Task Clear_SendsRequestSuccessfully(ButtonEnum button)
        {
            // Arrange
            await _client.PostAsync($"/mgba-http/button/add?button={button}", null);

            // Act
            var response = await _client.PostAsync($"/mgba-http/button/clear?button={button}", null);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("", responseContent);
        }

        [DataTestMethod]
        [DataRow(ButtonEnum.A, DisplayName = "Get Key A")]
        [DataRow(ButtonEnum.B, DisplayName = "Get Key B")]
        public async Task Get_ReturnsButtonState(ButtonEnum button)
        {
            // Act
            var response = await _client.GetAsync($"/mgba-http/button/get?button={button}");
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(0, int.Parse(responseContent));
        }

        [DataTestMethod]
        [DataRow(ButtonEnum.A, DisplayName = "Key A")]
        [DataRow(ButtonEnum.B, DisplayName = "Key B")]
        [DataRow(ButtonEnum.Select, DisplayName = "Key Select")]
        [DataRow(ButtonEnum.Start, DisplayName = "Key Start")]
        [DataRow(ButtonEnum.Right, DisplayName = "Key Right")]
        [DataRow(ButtonEnum.Left, DisplayName = "Key Left")]
        [DataRow(ButtonEnum.Up, DisplayName = "Key Up")]
        [DataRow(ButtonEnum.Down, DisplayName = "Key Down")]
        [DataRow(ButtonEnum.R, DisplayName = "Key R")]
        [DataRow(ButtonEnum.L, DisplayName = "Key L")]
        public async Task TapEndpoint_SendsRequestSuccessfully(ButtonEnum button)
        {
            // Act
            var response = await _client.PostAsync($"/mgba-http/button/tap?button={button}", null);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("", responseContent);
        }

        [DataTestMethod]
        [DataRow(new[] { ButtonEnum.A }, DisplayName = "Key A")]
        [DataRow(new[] { ButtonEnum.B }, DisplayName = "Key B")]
        [DataRow(new[] { ButtonEnum.A, ButtonEnum.B }, DisplayName = "Keys A+B")]
        [DataRow(new[] { ButtonEnum.Up, ButtonEnum.A }, DisplayName = "Keys Up+A")]
        [DataRow(new[] { ButtonEnum.Left, ButtonEnum.Right }, DisplayName = "Keys Left+Right")]
        [DataRow(new[] { ButtonEnum.Up, ButtonEnum.Down, ButtonEnum.Left, ButtonEnum.Right }, DisplayName = "Keys Up+Down+Left+Right")]
        public async Task TapManyEndpoint_SendsRequestSuccessfully(ButtonEnum[] buttons)
        {
            // Act
            var queryString = string.Join("&", buttons.Select(k => $"keys={k}"));
            var response = await _client.PostAsync($"/mgba-http/button/tapmany?{queryString}", null);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("", responseContent);
        }

        [DataTestMethod]
        [DataRow(ButtonEnum.A, 50, DisplayName = "Key A - 50 frames")]
        [DataRow(ButtonEnum.B, 10, DisplayName = "Key B - 10 frames")]
        [DataRow(ButtonEnum.Select, 15, DisplayName = "Key Select - 15 frames")]
        [DataRow(ButtonEnum.Start, 20, DisplayName = "Key Start - 20 frames")]
        [DataRow(ButtonEnum.Right, 50, DisplayName = "Key Right - 50 frames")]
        public async Task HoldEndpoint_SendsRequestSuccessfully(ButtonEnum button, int duration)
        {
            // Act
            var response = await _client.PostAsync($"/mgba-http/button/hold?button={button}&duration={duration}", null);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("", responseContent);
        }

        [DataTestMethod]
        [DataRow(new[] { ButtonEnum.A }, 50, DisplayName = "Key A - 50 frames")]
        [DataRow(new[] { ButtonEnum.A, ButtonEnum.B }, 10, DisplayName = "Keys A+B - 10 frames")]
        [DataRow(new[] { ButtonEnum.Up, ButtonEnum.B }, 15, DisplayName = "Keys Up+B - 15 frames")]
        [DataRow(new[] { ButtonEnum.Left, ButtonEnum.Right }, 50, DisplayName = "Keys Left+Right - 50 frames")]
        [DataRow(new[] { ButtonEnum.L, ButtonEnum.Down, ButtonEnum.A }, 10, DisplayName = "Keys Up+Down+A - 10 frames")]
        public async Task HoldManyEndpoint_SendsRequestSuccessfully(ButtonEnum[] buttons, int duration)
        {
            // Act
            var queryString = string.Join("&", buttons.Select(k => $"keys={k}")) + $"&duration={duration}";
            var response = await _client.PostAsync($"/mgba-http/button/holdmany?{queryString}", null);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("", responseContent);
        }

        public void Dispose()
        {
            _client.Dispose();
            _factory.Dispose();
        }
    }
}
