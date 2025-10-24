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
        [DataRow(KeysEnum.A, DisplayName = "Add Key A")]
        [DataRow(KeysEnum.B, DisplayName = "Add Key B")]
        [DataRow(KeysEnum.Start, DisplayName = "Add Key Start")]
        [DataRow(KeysEnum.Select, DisplayName = "Add Key Select")]
        [DataRow(KeysEnum.Right, DisplayName = "Add Key Right")]
        [DataRow(KeysEnum.Left, DisplayName = "Add Key Left")]
        [DataRow(KeysEnum.Up, DisplayName = "Add Key Up")]
        [DataRow(KeysEnum.Down, DisplayName = "Add Key Down")]
        [DataRow(KeysEnum.R, DisplayName = "Add Key R")]
        [DataRow(KeysEnum.L, DisplayName = "Add Key L")]
        public async Task AddKey_SendsRequestSuccessfully(KeysEnum key)
        {
            // Act
            var response = await _client.PostAsync($"/core/addkey?key={key}", null);
            await _client.PostAsync($"/core/clearkey?key={key}", null);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task AddKeys_SendsRequestSuccessfully()
        {
            // Arrange - 12 is Start (8) and Select (4)
            const int keyBitmask = 12;

            // Act
            var response = await _client.PostAsync($"/core/addkeys?keyBitmask={keyBitmask}", null);
            await _client.PostAsync($"/core/clearkeys?keyBitmask={keyBitmask}", null);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [DataTestMethod]
        [DataRow(KeysEnum.A, DisplayName = "Clear Key A")]
        [DataRow(KeysEnum.B, DisplayName = "Clear Key B")]
        [DataRow(KeysEnum.Start, DisplayName = "Clear Key Start")]
        [DataRow(KeysEnum.Select, DisplayName = "Clear Key Select")]
        [DataRow(KeysEnum.Right, DisplayName = "Clear Key Right")]
        [DataRow(KeysEnum.Left, DisplayName = "Clear Key Left")]
        [DataRow(KeysEnum.Up, DisplayName = "Clear Key Up")]
        [DataRow(KeysEnum.Down, DisplayName = "Clear Key Down")]
        [DataRow(KeysEnum.R, DisplayName = "Clear Key R")]
        [DataRow(KeysEnum.L, DisplayName = "Clear Key L")]
        public async Task ClearKey_SendsRequestSuccessfully(KeysEnum key)
        {
            // Arrange
            await _client.PostAsync($"/core/addkey?key={key}", null);

            // Act
            var response = await _client.PostAsync($"/core/clearkey?key={key}", null);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task ClearKeys_SendsRequestSuccessfully()
        {
            // Arrange - 12 is Start (8) and Select (4)
            const int keyBitmask = 12;
            await _client.PostAsync($"/core/addkeys?keyBitmask={keyBitmask}", null);

            // Act
            var response = await _client.PostAsync($"/core/clearkeys?keyBitmask={keyBitmask}", null);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task GetKeys_ReturnsKeyState()
        {
            // Arrange
            const int keyBitmask = 12;
            await _client.PostAsync($"/core/addkeys?keyBitmask={keyBitmask}", null);

            // Act
            var response = await _client.GetAsync("/core/getkeys");
            await _client.PostAsync($"/core/clearkeys?keyBitmask={keyBitmask}", null);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.AreEqual(keyBitmask, int.Parse(content));
        }

        [DataTestMethod]
        [DataRow(KeysEnum.A, DisplayName = "Get Key A")]
        [DataRow(KeysEnum.B, DisplayName = "Get Key B")]
        public async Task GetKey_ReturnsKeyState(KeysEnum key)
        {
            // Act
            var response = await _client.GetAsync($"/core/getkey?key={key}");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.AreEqual(0, int.Parse(content));
        }

        [TestMethod]
        public async Task SetKeys_SendsRequestSuccessfully()
        {
            // Arrange - 3 is A (1) and B (2)
            const int keysBitmask = 3;
            await _client.PostAsync($"/core/addkey?key={KeysEnum.Select}", null);


            // Act
            var response = await _client.PostAsync($"/core/setkeys?keys={keysBitmask}", null);


            // Assert
            var currentlySetKeysResponse = await _client.GetAsync("/core/getkeys");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var content = await currentlySetKeysResponse.Content.ReadAsStringAsync();

            Assert.AreEqual(keysBitmask, int.Parse(content));

            await _client.PostAsync($"/core/clearkeys?keyBitmask={keysBitmask}", null);
        }

        [TestMethod]
        public async Task Step_SendsRequestSuccessfully()
        {
            // Act
            var response = await _client.PostAsync("/core/step", null);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task CurrentFrame_ReturnsFrameNumber()
        {
            // Act
            var response = await _client.GetAsync("/core/currentFrame");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(int.Parse(content) >= 0);
        }

        [TestMethod]
        public async Task FrameCycles_ReturnsCyclesPerFrame()
        {
            // Act
            var response = await _client.GetAsync("/core/framecycles");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(int.Parse(content) > 0);
        }

        [TestMethod]
        public async Task Frequency_ReturnsCyclesPerSecond()
        {
            // Act
            var response = await _client.GetAsync("/core/frequency");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(int.Parse(content) > 0);
        }

        [TestMethod]
        [Ignore] // Seems to have a problem, might be with the ROM being used
        public async Task GetGameCode_ReturnsGameCode()
        {
            // Act
            var response = await _client.GetAsync("/core/getgamecode");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.IsFalse(string.IsNullOrEmpty(content));
        }

        [TestMethod]
        public async Task GetGameTitle_ReturnsGameTitle()
        {
            // Act
            var response = await _client.GetAsync("/core/getgametitle");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.AreEqual("(BIOS)", content);
        }

        [TestMethod]
        public async Task Platform_ReturnsPlatformType()
        {
            // Act
            var response = await _client.GetAsync("/core/platform");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.AreEqual(0, int.Parse(content));
        }

        [TestMethod]
        public async Task Checksum_ReturnsROMChecksum()
        {
            // Act
            var response = await _client.GetAsync("/core/checksum");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            long checksum = long.Parse(content);
            Assert.IsTrue(checksum >= 0);
        }

        [TestMethod]
        [Ignore] // This doesn't work for the button test ROM
        public async Task RomSize_ReturnsROMSize()
        {
            // Act
            var response = await _client.GetAsync("/core/romsize");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            int size = int.Parse(content);
            Assert.IsTrue(size > 0);
        }

        [TestMethod]
        public async Task AutoloadSave_SendsRequestSuccessfully()
        {
            // Act
            var response = await _client.PostAsync("/core/autoloadsave", null);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task LoadSaveFile_SendsRequestSuccessfully()
        {
            // Act
            var response = await _client.PostAsync($"/core/loadsavefile?path={TestSavePath}&temporary=true", null);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task Screenshot_SendsRequestSuccessfully()
        {
            // Act
            var response = await _client.PostAsync($"/core/screenshot?path={TestScreenshotPath}", null);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
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

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

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

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

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

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task LoadStateSlot_SendsRequestSuccessfully()
        {
            // Arrange
            await _client.PostAsync("/core/savestateslot?slot=1&flags=31", null);

            // Act
            var response = await _client.PostAsync("/core/loadstateslot?slot=1&flags=29", null);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        [Ignore] // Still an unstable API in mGBA-http
        public async Task SaveStateBuffer_ReturnsStateData()
        {
            // Act
            var response = await _client.PostAsync("/core/savestatebuffer?flags=31", null);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var stateData = JsonSerializer.Deserialize<int[]>(content);
            Assert.IsNotNull(stateData);
            Assert.IsTrue(stateData.Length > 0);
        }

        [TestMethod]
        public async Task Read8_ReturnsValue()
        {
            // Arrange
            var address = "0x10000000";

            // Act
            var response = await _client.GetAsync($"/core/read8?address={address}");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            int value = int.Parse(content);
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public async Task Read16_ReturnsValue()
        {
            // Arrange
            var address = "0x10000000";

            // Act
            var response = await _client.GetAsync($"/core/read16?address={address}");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            int value = int.Parse(content);
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public async Task Read32_ReturnsValue()
        {
            // Arrange
            var address = "0x10000000";

            // Act
            var response = await _client.GetAsync($"/core/read32?address={address}");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            long value = long.Parse(content);
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public async Task ReadRange_ReturnsValues()
        {
            // Arrange
            var address = "0x10000000";
            var length = 16;

            // Act
            var response = await _client.GetAsync($"/core/readrange?address={address}&length={length}");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var valuesLength = content.Split(',').Length;
            Assert.IsNotNull(content);
            Assert.AreEqual(length, valuesLength);
        }

        [TestMethod]
        public async Task ReadRange_Large_ReturnsValues()
        {
            // Arrange
            var address = "0x10000000";
            int length = 16000;

            // Act
            var response = await _client.GetAsync($"/core/readrange?address={address}&length={length}");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var valuesLength = content.Split(',').Length;
            Assert.IsNotNull(content);
            Assert.AreEqual(length, valuesLength);
        }

        [TestMethod]
        [Ignore] //Flaky, need to find an area in the raw bus to safely write
        public async Task Write8_SendsRequestSuccessfully()
        {
            // Arrange
            var address = "0x10000000";
            const int value = 42;

            // Act
            var response = await _client.PostAsync($"/core/write8?address={address}&value={value}", null);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

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

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

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

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

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

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            int value = int.Parse(content);
            Assert.IsNotNull(value);
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

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            // Verify write
            var readResponse = await _client.GetAsync($"/core/readregister?regName={regName}");
            var content = await readResponse.Content.ReadAsStringAsync();
            Assert.AreEqual(value, int.Parse(content));
        }

        public void Dispose()
        {
            _client.Dispose();
            _factory.Dispose();
        }
    }
}
