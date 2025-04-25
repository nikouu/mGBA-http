using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text.Json;

namespace mGBAHttpServer.IntegrationTests
{
    /// <summary>
    /// Integration tests for the MemoryDomainApi endpoints.
    /// 
    /// Note: These tests require a mGBA instance with the Lua script running
    /// </summary>
    [TestClass]
    public sealed class MemoryDomainApiTests : IDisposable
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private const string BiosDomain = "bios";
        private const string WRam = "wram";
        private const int ReadAddress = 0x00000000;
        private const int WriteAddress = 0x02000000;

        public MemoryDomainApiTests()
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }

        [TestMethod]
        public async Task BaseEndpoint_ReturnBaseAddress()
        {
            // Act
            var response = await _client.GetAsync($"/memorydomain/base?memoryDomain={BiosDomain}");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.AreEqual(0, int.Parse(content));
        }

        [TestMethod]
        public async Task BoundEndpoint_ReturnBoundAddress()
        {
            // Act
            var response = await _client.GetAsync($"/memorydomain/bound?memoryDomain={BiosDomain}");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.AreEqual(16384, int.Parse(content));
        }

        [TestMethod]
        public async Task NameEndpoint_ReturnDomainName()
        {
            // Act
            var response = await _client.GetAsync($"/memorydomain/name?memoryDomain={BiosDomain}");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.AreEqual("BIOS", content);
        }

        [TestMethod]
        public async Task SizeEndpoint_ReturnDomainSize()
        {
            // Act
            var response = await _client.GetAsync($"/memorydomain/size?memoryDomain={BiosDomain}");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.AreEqual(16384, int.Parse(content));
        }

        [TestMethod]
        public async Task Read8Endpoint_Reads8()
        {
            // Act
            var response = await _client.GetAsync($"/memorydomain/read8?memoryDomain={BiosDomain}&address={ReadAddress}");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.AreEqual(211, int.Parse(content));
        }

        [TestMethod]
        public async Task Read16Endpoint_Read16()
        {
            // Act
            var response = await _client.GetAsync($"/memorydomain/read16?memoryDomain={BiosDomain}&address={ReadAddress}");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.AreEqual(211, int.Parse(content));
        }

        [TestMethod]
        public async Task Read32Endpoint_Read32()
        {
            // Act
            var response = await _client.GetAsync($"/memorydomain/read32?memoryDomain={BiosDomain}&address={ReadAddress}");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.AreEqual(3925868755, long.Parse(content));
        }

        [TestMethod]
        public async Task ReadRangeEndpoint_ReadRange()
        {
            // Arrange
            const string length = "16"; // Read 16 bytes
            int[] expected = [211, 0, 0, 234, 102, 0, 0, 234, 12, 0, 0, 234, 254, 255, 255, 234];

            // Act
            var response = await _client.GetAsync($"/memorydomain/readrange?memoryDomain={BiosDomain}&address={ReadAddress}&length={length}");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var values = JsonSerializer.Deserialize<int[]>(content);

            CollectionAssert.AreEqual(expected, values);
        }

        [TestMethod]
        public async Task Write8Endpoint_Write8()
        {
            // Arrange
            const int value = 42;

            // Act
            var response = await _client.PostAsync($"/memorydomain/write8?memoryDomain={WRam}&address={WriteAddress}&value={value}", null);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            // Verify the write by reading back the value
            var readResponse = await _client.GetAsync($"/memorydomain/read8?memoryDomain={WRam}&address={WriteAddress}");
            var content = await readResponse.Content.ReadAsStringAsync();

            Assert.AreEqual(value, int.Parse(content));
        }

        [TestMethod]
        public async Task Write16Endpoint_Write16()
        {
            // Arrange
            const int value = 12345;

            // Act
            var response = await _client.PostAsync($"/memorydomain/write16?memoryDomain={WRam}&address={WriteAddress + 16}&value={value}", null);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            // Verify the write by reading back the value
            var readResponse = await _client.GetAsync($"/memorydomain/read16?memoryDomain={WRam}&address={WriteAddress + 16}");
            var content = await readResponse.Content.ReadAsStringAsync();

            Assert.AreEqual(value, int.Parse(content));
        }

        [TestMethod]
        public async Task Write32Endpoint_Write32()
        {
            // Arrange
            const int value = 0x12345678;

            // Act
            var response = await _client.PostAsync($"/memorydomain/write32?memoryDomain={WRam}&address={WriteAddress + 32}&value={value}", null);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            // Verify the write by reading back the value
            var readResponse = await _client.GetAsync($"/memorydomain/read32?memoryDomain={WRam}&address={WriteAddress + 32}");
            var content = await readResponse.Content.ReadAsStringAsync();

            Assert.AreEqual(value, long.Parse(content));
        }

        public void Dispose()
        {
            _client.Dispose();
            _factory.Dispose();
        }
    }
}

