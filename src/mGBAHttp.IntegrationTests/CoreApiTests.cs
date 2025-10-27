using mGBAHttp.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text.Json;

namespace mGBAHttp.IntegrationTests
{
    /// <summary>
    /// Integration tests for the CoreApi endpoints.
    /// Note: These tests require a mGBA instance with the Lua script running against the button test ROM
    /// </summary>
    [TestClass]
    public sealed class CoreApiTests : IDisposable
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private static readonly string RomDirectory = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\..\")); // root of the repo
        private readonly string TestRomPath = Path.Combine(RomDirectory, "BtnTest.gba");
        private readonly string TestSavePath = Path.Combine(RomDirectory, "BtnTest.sav");
        private readonly string TestScreenshotPath = Path.Combine(RomDirectory, "screenshot.png");
        private readonly string TestStatePath = Path.Combine(RomDirectory, "savestate.ss0");

        public CoreApiTests()
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }

        [TestMethod]
        [Ignore] // Unsure fully how this works to test it. Loading seems to crash mGBA or do nothing. Best to use /mgba-http/extension/loadfile
        public async Task LoadFile_SendsRequestSuccessfully()
        {
            // Act
            var response = await _client.PostAsync($"/core/loadfile?path={TestRomPath}", null);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [DataTestMethod]
        [DataRow(0, DisplayName = "Add Key A")]
        [DataRow(1, DisplayName = "Add Key B")]
        [DataRow(2, DisplayName = "Add Key Select")]
        [DataRow(3, DisplayName = "Add Key Start")]
        [DataRow(4, DisplayName = "Add Key Right")]
        [DataRow(5, DisplayName = "Add Key Left")]
        [DataRow(6, DisplayName = "Add Key Up")]
        [DataRow(7, DisplayName = "Add Key Down")]
        [DataRow(8, DisplayName = "Add Key R")]
        [DataRow(9, DisplayName = "Add Key L")]
        public async Task AddKey_SendsRequestSuccessfully(int key)
        {
            // Act
            var response = await _client.PostAsync($"/core/addkey?key={key}", null);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Reset state
            await _client.PostAsync($"/core/clearkey?key={key}", null);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("", responseContent);
        }

        [TestMethod]
        public async Task AddKeys_SendsRequestSuccessfully()
        {
            // Arrange - 12 is Start (8) and Select (4)
            const int keyBitmask = 12;

            // Act
            var response = await _client.PostAsync($"/core/addkeys?keyBitmask={keyBitmask}", null);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Reset state
            await _client.PostAsync($"/core/clearkeys?keyBitmask={keyBitmask}", null);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("", responseContent);
        }

        [DataTestMethod]
        [DataRow(0, DisplayName = "Clear Key A")]
        [DataRow(1, DisplayName = "Clear Key B")]
        [DataRow(2, DisplayName = "Clear Key Select")]
        [DataRow(3, DisplayName = "Clear Key Start")]
        [DataRow(4, DisplayName = "Clear Key Right")]
        [DataRow(5, DisplayName = "Clear Key Left")]
        [DataRow(6, DisplayName = "Clear Key Up")]
        [DataRow(7, DisplayName = "Clear Key Down")]
        [DataRow(8, DisplayName = "Clear Key R")]
        [DataRow(9, DisplayName = "Clear Key L")]
        public async Task ClearKey_SendsRequestSuccessfully(int key)
        {
            // Arrange
            await _client.PostAsync($"/core/addkey?key={key}", null);

            // Act
            var response = await _client.PostAsync($"/core/clearkey?key={key}", null);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("", responseContent);
        }

        [TestMethod]
        public async Task ClearKeys_SendsRequestSuccessfully()
        {
            // Arrange - 12 is Start (8) and Select (4)
            const int keyBitmask = 12;
            await _client.PostAsync($"/core/addkeys?keyBitmask={keyBitmask}", null);

            // Act
            var response = await _client.PostAsync($"/core/clearkeys?keyBitmask={keyBitmask}", null);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("", responseContent);
        }

        [TestMethod]
        public async Task GetKeys_ReturnsKeyState()
        {
            // Arrange
            const int keyBitmask = 12;
            await _client.PostAsync($"/core/addkeys?keyBitmask={keyBitmask}", null);

            // Act
            var response = await _client.GetAsync("/core/getkeys");
            var responseContent = await response.Content.ReadAsStringAsync();

            // Reset state
            await _client.PostAsync($"/core/clearkeys?keyBitmask={keyBitmask}", null);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(keyBitmask, int.Parse(responseContent));
        }

        [DataTestMethod]
        [DataRow(0, DisplayName = "Get Key A")]
        [DataRow(1, DisplayName = "Get Key B")]
        public async Task GetKey_ReturnsKeyState(int key)
        {
            // Act
            var response = await _client.GetAsync($"/core/getkey?key={key}");
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(0, int.Parse(responseContent));
        }

        [TestMethod]
        public async Task SetKeys_SendsRequestSuccessfully()
        {
            // Arrange - 3 is A (1) and B (2)
            var keysBitmask = 3;
            await _client.PostAsync($"/core/addkey?key=2", null);

            // Act
            var response = await _client.PostAsync($"/core/setkeys?keys={keysBitmask}", null);
            var responseContent = await response.Content.ReadAsStringAsync();
            var currentlySetKeysResponse = await _client.GetStringAsync("/core/getkeys");

            // Reset state
            await _client.PostAsync($"/core/clearkeys?keyBitmask={keysBitmask}", null);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("", responseContent);
            Assert.AreEqual(keysBitmask, int.Parse(currentlySetKeysResponse));
        }

        [TestMethod]
        public async Task Step_SendsRequestSuccessfully()
        {
            // Act
            var response = await _client.PostAsync("/core/step", null);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("", responseContent);
        }

        [TestMethod]
        public async Task CurrentFrame_ReturnsFrameNumber()
        {
            // Act
            var response = await _client.GetAsync("/core/currentFrame");
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(int.Parse(responseContent) >= 0);
        }

        [TestMethod]
        public async Task FrameCycles_ReturnsCyclesPerFrame()
        {
            // Act
            var response = await _client.GetAsync("/core/framecycles");
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(int.Parse(responseContent) > 0);
        }

        [TestMethod]
        public async Task Frequency_ReturnsCyclesPerSecond()
        {
            // Act
            var response = await _client.GetAsync("/core/frequency");
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(int.Parse(responseContent) > 0);
        }

        [TestMethod]
        [Ignore] // Seems to have a problem, might be with the ROM being used
        public async Task GetGameCode_ReturnsGameCode()
        {
            // Act
            var response = await _client.GetAsync("/core/getgamecode");
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsFalse(string.IsNullOrEmpty(responseContent));
        }

        [TestMethod]
        public async Task GetGameTitle_ReturnsGameTitle()
        {
            // Act
            var response = await _client.GetAsync("/core/getgametitle");
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("(BIOS)", responseContent);
        }

        [TestMethod]
        public async Task Platform_ReturnsPlatformType()
        {
            // Act
            var response = await _client.GetAsync("/core/platform");
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(0, int.Parse(responseContent));
        }

        [TestMethod]
        public async Task Checksum_ReturnsROMChecksum()
        {
            // Act
            var response = await _client.GetAsync("/core/checksum");
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(long.Parse(responseContent) >= 0);
        }

        [TestMethod]
        [Ignore] // This doesn't work for the button test ROM
        public async Task RomSize_ReturnsROMSize()
        {
            // Act
            var response = await _client.GetAsync("/core/romsize");
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(int.Parse(responseContent) > 0);
        }

        [TestMethod]
        public async Task AutoloadSave_SendsRequestSuccessfully()
        {
            // Act
            var response = await _client.PostAsync("/core/autoloadsave", null);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("true", responseContent);
        }

        [TestMethod]
        public async Task LoadSaveFile_SendsRequestSuccessfully()
        {
            // Act
            var response = await _client.PostAsync($"/core/loadsavefile?path={TestSavePath}&temporary=true", null);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("true", responseContent);
        }

        [TestMethod]
        public async Task Screenshot_SendsRequestSuccessfully()
        {
            // Act
            var response = await _client.PostAsync($"/core/screenshot?path={TestScreenshotPath}", null);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("", responseContent);

            var file = new FileInfo(TestScreenshotPath);
            Assert.IsTrue(file.Exists);
            Assert.IsTrue(file.Length > 0);

            File.Delete(TestScreenshotPath);
        }

        [TestMethod]
        public async Task SaveStateFile_SendsRequestSuccessfully()
        {
            // Act
            var response = await _client.PostAsync($"/core/savestatefile?path={TestStatePath}&flags=31", null);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("true", responseContent);

            var file = new FileInfo(TestStatePath);
            Assert.IsTrue(file.Exists);
            Assert.IsTrue(file.Length > 0);

            File.Delete(TestStatePath);
        }

        [TestMethod]
        public async Task LoadStateFile_SendsRequestSuccessfully()
        {
            // Arrange
            await _client.PostAsync($"/core/savestatefile?path={TestStatePath}&flags=31", null);

            // Act
            var response = await _client.PostAsync($"/core/loadstatefile?path={TestStatePath}&flags=29", null);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("true", responseContent);

            // Clean up
            string directory = Path.GetDirectoryName(TestStatePath);
            foreach (string file in Directory.GetFiles(directory, "*.ss*"))
            {
                File.Delete(file);
            }
        }

        [TestMethod]
        public async Task SaveStateSlot_SendsRequestSuccessfully()
        {
            // Act
            var response = await _client.PostAsync("/core/savestateslot?slot=1&flags=31", null);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("true", responseContent);
        }

        [TestMethod]
        public async Task LoadStateSlot_SendsRequestSuccessfully()
        {
            // Arrange
            await _client.PostAsync("/core/savestateslot?slot=1&flags=31", null);

            // Act
            var response = await _client.PostAsync("/core/loadstateslot?slot=1&flags=29", null);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("true", responseContent);
        }

        [TestMethod]
        public async Task Read8_ReturnsValue()
        {
            // Arrange
            var address = "0x1000";

            // Act
            var response = await _client.GetAsync($"/core/read8?address={address}");
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(int.Parse(responseContent) >= 0);
        }

        [TestMethod]
        public async Task Read16_ReturnsValue()
        {
            // Arrange
            var address = "0x1000";

            // Act
            var response = await _client.GetAsync($"/core/read16?address={address}");
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(int.Parse(responseContent) >= 0);
        }

        [TestMethod]
        public async Task Read32_ReturnsValue()
        {
            // Arrange
            var address = "0x1000";

            // Act
            var response = await _client.GetAsync($"/core/read32?address={address}");
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(long.Parse(responseContent) >= 0);
        }

        [TestMethod]
        public async Task ReadRange_ReturnsValues()
        {
            // Arrange
            var address = "0x1000";
            var length = 16;

            // Act
            var response = await _client.GetAsync($"/core/readrange?address={address}&length={length}");
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            
            var valuesLength = responseContent.Split(',').Length;
            Assert.IsNotNull(responseContent);
            Assert.AreEqual(length, valuesLength);
        }

        [TestMethod]
        public async Task ReadRange_Large_ReturnsValues()
        {
            // Arrange
            var address = "0x1000";
            int length = 16000;

            // Act
            var response = await _client.GetAsync($"/core/readrange?address={address}&length={length}");
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            
            var valuesLength = responseContent.Split(',').Length;
            Assert.IsNotNull(responseContent);
            Assert.AreEqual(length, valuesLength);
        }

        [TestMethod]
        [Ignore] //Flaky, need to find an area in the raw bus to safely write
        public async Task Write8_SendsRequestSuccessfully()
        {
            // Arrange
            var address = "0x1000";
            const int value = 42;

            // Act
            var response = await _client.PostAsync($"/core/write8?address={address}&value={value}", null);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("", responseContent);

            // Verify write
            var readResponse = await _client.GetAsync($"/core/read8?address={address}");
            var content = await readResponse.Content.ReadAsStringAsync();
            Assert.AreEqual(value, int.Parse(content));
        }

        [TestMethod]
        [Ignore] //Flaky, need to find an area in the raw bus to safely write
        public async Task Write16_SendsRequestSuccessfully()
        {
            // Arrange
            var address = "0x10000002";
            const int value = 12345;

            // Act
            var response = await _client.PostAsync($"/core/write16?address={address}&value={value}", null);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("", responseContent);

            // Verify write
            var readResponse = await _client.GetAsync($"/core/read16?address={address}");
            var content = await readResponse.Content.ReadAsStringAsync();
            Assert.AreEqual(value, int.Parse(content));
        }

        [TestMethod]
        [Ignore] //Flaky, need to find an area in the raw bus to safely write
        public async Task Write32_SendsRequestSuccessfully()
        {
            // Arrange
            var address = "0x10000004";
            const int value = 0x12345678;

            // Act
            var response = await _client.PostAsync($"/core/write32?address={address}&value={value}", null);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("", responseContent);

            // Verify write
            var readResponse = await _client.GetAsync($"/core/read32?address={address}");
            var content = await readResponse.Content.ReadAsStringAsync();
            Assert.AreEqual(value, long.Parse(content));
        }

        [TestMethod]
        public async Task ReadRegister_ReturnsRegisterValue()
        {
            // Arrange
            const string regName = "r0";

            // Act
            var response = await _client.GetAsync($"/core/readregister?regName={regName}");
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsFalse(string.IsNullOrEmpty(responseContent));
            Assert.IsTrue(int.Parse(responseContent) >= 0);
        }

        [TestMethod]
        [Ignore] // Difficult for now due to register values changing rapidly. Need to find the right register to use
        public async Task WriteRegister_SendsRequestSuccessfully()
        {
            // Arrange
            const string regName = "r1";
            const int value = 0x1234;

            // Act
            var response = await _client.PostAsync($"/core/writeregister?regName={regName}&value={value}", null);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("", responseContent);

            // Verify write
            var readResponse = await _client.GetAsync($"/core/readregister?regName={regName}");
            var content = await readResponse.Content.ReadAsStringAsync();
            Assert.AreEqual(value, int.Parse(content));
        }

        [TestMethod]
        public async Task SaveStateBuffer_ReturnsStateData()
        {
            // Act
            var response = await _client.PostAsync("/core/savestatebuffer?flags=31", null);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var hexValues = responseContent.Split(',');
            Assert.IsTrue(hexValues.Length > 0);

            foreach (var hex in hexValues)
            {
                Assert.IsTrue(int.TryParse(hex.Trim(), System.Globalization.NumberStyles.HexNumber, null, out _));
            }
        }

        [TestMethod]
        public async Task LoadStateBuffer_SendsRequestSuccessfully()
        {
            // Arrange
            var saveResponse = await _client.PostAsync("/core/savestatebuffer?flags=31", null);
            var savedState = await saveResponse.Content.ReadAsStringAsync();

            // Act
            var content = new StringContent($"[{savedState}]", System.Text.Encoding.UTF8, "text/plain");
            var response = await _client.PostAsync("/core/loadstatebuffer?flags=29", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("true", responseContent);
        }

        public void Dispose()
        {
            _client.Dispose();
            _factory.Dispose();
        }
    }
}
