using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text.Json;

namespace mGBAHttp.IntegrationTests
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
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);            
            Assert.AreEqual(0, int.Parse(responseContent));
        }

        [TestMethod]
        public async Task BoundEndpoint_ReturnBoundAddress()
        {
            // Act
            var response = await _client.GetAsync($"/memorydomain/bound?memoryDomain={BiosDomain}");
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(16384, int.Parse(responseContent));
        }

        [TestMethod]
        public async Task NameEndpoint_ReturnDomainName()
        {
            // Act
            var response = await _client.GetAsync($"/memorydomain/name?memoryDomain={BiosDomain}");
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("BIOS", responseContent);
        }

        [TestMethod]
        public async Task SizeEndpoint_ReturnDomainSize()
        {
            // Act
            var response = await _client.GetAsync($"/memorydomain/size?memoryDomain={BiosDomain}");
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(16384, int.Parse(responseContent));
        }

        [TestMethod]
        public async Task Read8Endpoint_Reads8()
        {
            // Act
            var response = await _client.GetAsync($"/memorydomain/read8?memoryDomain={BiosDomain}&address={ReadAddress}");
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(211, int.Parse(responseContent));
        }

        [TestMethod]
        public async Task Read16Endpoint_Read16()
        {
            // Act
            var response = await _client.GetAsync($"/memorydomain/read16?memoryDomain={BiosDomain}&address={ReadAddress}");
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(211, int.Parse(responseContent));
        }

        [TestMethod]
        public async Task Read32Endpoint_Read32()
        {
            // Act
            var response = await _client.GetAsync($"/memorydomain/read32?memoryDomain={BiosDomain}&address={ReadAddress}");
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(3925868755, long.Parse(responseContent));
        }

        [TestMethod]
        public async Task ReadRangeEndpoint_ReadRange()
        {
            // Arrange
            const string length = "16"; // Read 16 bytes
            int[] expected = [0xd3, 0x00, 0x00, 0xea, 0x66, 0x00, 0x00, 0xea, 0x0c, 0x00, 0x00, 0xea, 0xfe, 0xff, 0xff, 0xea];

            // Act
            var response = await _client.GetAsync($"/memorydomain/readrange?memoryDomain={BiosDomain}&address={ReadAddress}&length={length}");
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            
            var values = responseContent.Trim('[', ']').Split(',').Select(x => Convert.ToInt32(x.Trim(), 16)).ToArray();
            CollectionAssert.AreEqual(expected, values);
        }

        [TestMethod]
        public async Task ReadRangeEndpoint_ReadRange_Large()
        {
            // Arrange
            const int length = 16384;

            // Act
            var response = await _client.GetAsync($"/memorydomain/readrange?memoryDomain={BiosDomain}&address={0x0}&length={length}");
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var valuesLength = responseContent.Split(',').Length;
            Assert.IsNotNull(responseContent);
            Assert.AreEqual(length, valuesLength);
        }

        [TestMethod]
        public async Task Write8Endpoint_Write8()
        {
            // Arrange
            const int value = 42;

            // Act
            var response = await _client.PostAsync($"/memorydomain/write8?memoryDomain={WRam}&address={WriteAddress}&value={value}", null);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("", responseContent);

            // Verify the write by reading back the value
            var readResponse = await _client.GetAsync($"/memorydomain/read8?memoryDomain={WRam}&address={WriteAddress}");
            var readResponseContent = await readResponse.Content.ReadAsStringAsync();

            Assert.AreEqual(value, int.Parse(readResponseContent));
        }

        [TestMethod]
        public async Task Write16Endpoint_Write16()
        {
            // Arrange
            const int value = 12345;

            // Act
            var response = await _client.PostAsync($"/memorydomain/write16?memoryDomain={WRam}&address={WriteAddress + 16}&value={value}", null);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("", responseContent);

            // Verify the write by reading back the value
            var readResponse = await _client.GetAsync($"/memorydomain/read16?memoryDomain={WRam}&address={WriteAddress + 16}");
            var readResponseContent = await readResponse.Content.ReadAsStringAsync();

            Assert.AreEqual(value, int.Parse(readResponseContent));
        }

        [TestMethod]
        public async Task Write32Endpoint_Write32()
        {
            // Arrange
            const int value = 0x12345678;

            // Act
            var response = await _client.PostAsync($"/memorydomain/write32?memoryDomain={WRam}&address={WriteAddress + 32}&value={value}", null);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("", responseContent);

            // Verify the write by reading back the value
            var readResponse = await _client.GetAsync($"/memorydomain/read32?memoryDomain={WRam}&address={WriteAddress + 32}");
            var readResponseContent = await readResponse.Content.ReadAsStringAsync();

            Assert.AreEqual(value, long.Parse(readResponseContent));
        }

        [TestMethod]
        public async Task Write32Endpoint_MaxSignedIntPlusOne_WritesCorrectValue()
        {
            // Arrange
            const uint value = 2147483648U; // Max signed int32 + 1 (0x80000000)
            const int offset = 64;

            // Act
            var writeResponse = await _client.PostAsync($"/memorydomain/write32?memoryDomain={WRam}&address={WriteAddress + offset}&value={value}", null);

            var readResponse = await _client.GetAsync($"/memorydomain/read32?memoryDomain={WRam}&address={WriteAddress + offset}");
            var readContent = await readResponse.Content.ReadAsStringAsync();
            var readValue = uint.Parse(readContent);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, writeResponse.StatusCode);
            Assert.AreEqual(value, readValue);
        }

        [TestMethod]
        public async Task Write32Endpoint_MaxUnsignedInt_WritesCorrectValue()
        {
            // Arrange
            const uint value = 4294967295U; // Max unsigned int32 (0xFFFFFFFF)
            const int offset = 68;

            // Act
            var writeResponse = await _client.PostAsync($"/memorydomain/write32?memoryDomain={WRam}&address={WriteAddress + offset}&value={value}", null);
            
            var readResponse = await _client.GetAsync($"/memorydomain/read32?memoryDomain={WRam}&address={WriteAddress + offset}");
            var readContent = await readResponse.Content.ReadAsStringAsync();
            var readValue = uint.Parse(readContent);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, writeResponse.StatusCode);
            Assert.AreEqual(value, readValue);
        }

        [TestMethod]
        public async Task Write32Endpoint_MaxSignedInt_WritesCorrectValue()
        {
            // Arrange
            const uint value = 2147483647U; // Max signed int32 (0x7FFFFFFF)
            const int offset = 72;

            // Act
            var writeResponse = await _client.PostAsync($"/memorydomain/write32?memoryDomain={WRam}&address={WriteAddress + offset}&value={value}", null);
            
            var readResponse = await _client.GetAsync($"/memorydomain/read32?memoryDomain={WRam}&address={WriteAddress + offset}");
            var readContent = await readResponse.Content.ReadAsStringAsync();
            var readValue = uint.Parse(readContent);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, writeResponse.StatusCode);
            Assert.AreEqual(value, readValue);
        }

        [TestMethod]
        public async Task Write16Endpoint_MaxSignedInt16PlusOne_WritesCorrectValue()
        {
            // Arrange
            const ushort value = 32768; // Max signed int16 + 1 (0x8000)
            const int offset = 76;

            // Act
            var writeResponse = await _client.PostAsync($"/memorydomain/write16?memoryDomain={WRam}&address={WriteAddress + offset}&value={value}", null);

            var readResponse = await _client.GetAsync($"/memorydomain/read16?memoryDomain={WRam}&address={WriteAddress + offset}");
            var readContent = await readResponse.Content.ReadAsStringAsync();
            var readValue = ushort.Parse(readContent);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, writeResponse.StatusCode);
            Assert.AreEqual(value, readValue);
        }

        [TestMethod]
        public async Task Write16Endpoint_MaxUnsignedInt16_WritesCorrectValue()
        {
            // Arrange
            const ushort value = 65535; // Max unsigned int16 (0xFFFF)
            const int offset = 78;

            // Act
            var writeResponse = await _client.PostAsync($"/memorydomain/write16?memoryDomain={WRam}&address={WriteAddress + offset}&value={value}", null);
            
            var readResponse = await _client.GetAsync($"/memorydomain/read16?memoryDomain={WRam}&address={WriteAddress + offset}");
            var readContent = await readResponse.Content.ReadAsStringAsync();
            var readValue = ushort.Parse(readContent);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, writeResponse.StatusCode);
            Assert.AreEqual(value, readValue);
        }

        [TestMethod]
        public async Task Read32Endpoint_ReturnsUnsignedValue()
        {
            // Act
            var response = await _client.GetAsync($"/memorydomain/read32?memoryDomain={BiosDomain}&address={ReadAddress}");
            var responseContent = await response.Content.ReadAsStringAsync();
            var value = uint.Parse(responseContent);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(3925868755U, value);
            Assert.IsTrue(value > int.MaxValue);
        }

        [TestMethod]
        [DataRow(0U, DisplayName = "Write 0")]
        [DataRow(1U, DisplayName = "Write 1")]
        [DataRow(255U, DisplayName = "Write 255")]
        [DataRow(256U, DisplayName = "Write 256")]
        [DataRow(65535U, DisplayName = "Write 65535")]
        [DataRow(65536U, DisplayName = "Write 65536")]
        [DataRow(2147483647U, DisplayName = "Write Max Signed Int32")]
        [DataRow(2147483648U, DisplayName = "Write Max Signed Int32 + 1")]
        [DataRow(3000000000U, DisplayName = "Write 3 Billion")]
        [DataRow(4294967295U, DisplayName = "Write Max Unsigned Int32")]
        public async Task Write32Endpoint_VariousUnsignedValues_WritesCorrectValue(uint value)
        {
            // Arrange
            const int offset = 80;

            // Act
            var writeResponse = await _client.PostAsync($"/memorydomain/write32?memoryDomain={WRam}&address={WriteAddress + offset}&value={value}", null);

            var readResponse = await _client.GetAsync($"/memorydomain/read32?memoryDomain={WRam}&address={WriteAddress + offset}");
            var readContent = await readResponse.Content.ReadAsStringAsync();
            var readValue = uint.Parse(readContent);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, writeResponse.StatusCode);
            Assert.AreEqual(value, readValue, $"Expected to write and read back {value} (0x{value:X8})");
        }
        public void Dispose()
        {
            _client.Dispose();
            _factory.Dispose();
        }
    }
}

