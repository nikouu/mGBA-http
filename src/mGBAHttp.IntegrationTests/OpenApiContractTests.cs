using Microsoft.AspNetCore.Mvc.Testing;
using System.Text.Json;
namespace mGBAHttp.IntegrationTests;

[TestClass]
public class OpenApiContractTests : VerifyBase
{
    [TestMethod]
    public async Task OpenApiDocument_MatchesBaseline()
    {
        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var json = await client.GetStringAsync("/openapi/v1.json");

        using var document = JsonDocument.Parse(json);
        var pretty = JsonSerializer.Serialize(document.RootElement, new JsonSerializerOptions { WriteIndented = true });

        await Verify(pretty);
    }
}
