using mGBAHttp.Domain;
using mGBAHttp.Models;
using Microsoft.Extensions.ObjectPool;

namespace mGBAHttp.Endpoints
{
    public static class MemoryDomainApi
    {
        public static RouteGroupBuilder MapMemoryDomainEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/memorydomain");
            group.WithTags("MemoryDomain");

            group.MapGet("/base", async (ObjectPool<ReusableSocket> socketPool, string memoryDomain) =>
            {
                var messageModel = new MessageModel("memoryDomain.base", memoryDomain.ToString()).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Get the base address of the memory domain.";
                o.Description = "Get the base address of the specified memory domain.";
                o.Parameters[0].Description = "Memory domain name (e.g., 'wram', 'cart0', 'bios')";
                o.Responses["200"].Description = "Address of the base of this memory domain";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("50331648");
                return o;
            });

            group.MapGet("/bound", async (ObjectPool<ReusableSocket> socketPool, string memoryDomain) =>
            {
                var messageModel = new MessageModel("memoryDomain.bound", memoryDomain.ToString()).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Get the bound (end address) of the memory domain.";
                o.Description = "Get the bound (end address) of the specified memory domain as a decimal string.";
                o.Parameters[0].Description = "Memory domain name (e.g., 'wram', 'cart0', 'bios')";
                o.Responses["200"].Description = "Bound address as decimal string";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("50594303");
                return o;
            });

            group.MapGet("/name", async (ObjectPool<ReusableSocket> socketPool, string memoryDomain) =>
            {
                var messageModel = new MessageModel("memoryDomain.name", memoryDomain.ToString()).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Get the name of the memory domain.";
                o.Description = "Get the name of the specified memory domain.";
                o.Parameters[0].Description = "Memory domain name (e.g., 'wram', 'cart0', 'bios')";
                o.Responses["200"].Description = "Memory domain name";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("wram");
                return o;
            });

            group.MapGet("/read16", async (ObjectPool<ReusableSocket> socketPool, string memoryDomain, string address) =>
            {
                var messageModel = new MessageModel("memoryDomain.read16", memoryDomain.ToString(), address).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Read a 16-bit value from the given offset.";
                o.Description = "Read a 16-bit value from the given offset in the specified memory domain.";
                o.Parameters[0].Description = "Memory domain name (e.g., 'wram', 'cart0', 'bios')";
                o.Parameters[1].Description = "Address as hex string with 0x prefix (e.g., '0x03000000')";
                o.Responses["200"].Description = "16-bit value as decimal string";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("65535");
                return o;
            });

            group.MapGet("/read32", async (ObjectPool<ReusableSocket> socketPool, string memoryDomain, string address) =>
            {
                var messageModel = new MessageModel("memoryDomain.read32", memoryDomain.ToString(), address).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Read a 32-bit value from the given offset.";
                o.Description = "Read a 32-bit value from the given offset in the specified memory domain.";
                o.Parameters[0].Description = "Memory domain name (e.g., 'wram', 'cart0', 'bios')";
                o.Parameters[1].Description = "Address as hex string with 0x prefix (e.g., '0x03000000')";
                o.Responses["200"].Description = "32-bit value as decimal string";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("4294967295");
                return o;
            });

            group.MapGet("/read8", async (ObjectPool<ReusableSocket> socketPool, string memoryDomain, string address) =>
            {
                var messageModel = new MessageModel("memoryDomain.read8", memoryDomain.ToString(), address).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Read an 8-bit value from the given offset.";
                o.Description = "Read an 8-bit value from the given offset in the specified memory domain.";
                o.Parameters[0].Description = "Memory domain name (e.g., 'wram', 'cart0', 'bios')";
                o.Parameters[1].Description = "Address as hex string with 0x prefix (e.g., '0x03000000')";
                o.Responses["200"].Description = "8-bit value as decimal string";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("255");
                return o;
            });

            group.MapGet("/readrange", async (ObjectPool<ReusableSocket> socketPool, string memoryDomain, string address, string length) =>
            {
                var messageModel = new MessageModel("memoryDomain.readRange", memoryDomain.ToString(), address, length).ToString();
                var result = await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
                return result;
            }).WithOpenApi(o =>
            {
                o.Summary = "Read byte range from the given offset.";
                o.Description = "Read byte range from the given offset and returns a hex array string in the format: [d3,00,ea,66,...]";
                o.Parameters[0].Description = "Memory domain name (e.g., 'wram', 'cart0', 'bios')";
                o.Parameters[1].Description = "Address as hex string (e.g., '0x03000000')";
                o.Parameters[2].Description = "Number of bytes to read";
                o.Responses["200"].Description = "Hex array string format: [d3,00,ea,66,...]";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("[d3,00,00,ea,66,00,00,ea]");
                return o;
            });

            group.MapGet("/size", async (ObjectPool<ReusableSocket> socketPool, string memoryDomain) =>
            {
                var messageModel = new MessageModel("memoryDomain.size", memoryDomain.ToString()).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Get the size of the memory domain.";
                o.Description = "Get the size of the specified memory domain in bytes as a decimal string.";
                o.Parameters[0].Description = "Memory domain name (e.g., 'wram', 'cart0', 'bios')";
                o.Responses["200"].Description = "Size in bytes as decimal string";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("262144");
                return o;
            });

            group.MapPost("/write16", async (ObjectPool<ReusableSocket> socketPool, string memoryDomain, string address, int value) =>
            {
                var messageModel = new MessageModel("memoryDomain.write16", memoryDomain.ToString(), address, value.ToString()).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Write a 16-bit value to the given offset.";
                o.Description = "Write a 16-bit value to the given offset in the specified memory domain.";
                o.Parameters[0].Description = "Memory domain name (e.g., 'wram', 'cart0', 'bios')";
                o.Parameters[1].Description = "Address as hex string with 0x prefix (e.g., '0x03000000')";
                o.Parameters[2].Description = "16-bit value to write (0-65535)";
                o.Responses["200"].Description = "Empty success response";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("");
                return o;
            });

            group.MapPost("/write32", async (ObjectPool<ReusableSocket> socketPool, string memoryDomain, string address, int value) =>
            {
                var messageModel = new MessageModel("memoryDomain.write32", memoryDomain.ToString(), address, value.ToString()).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Write a 32-bit value to the given offset.";
                o.Description = "Write a 32-bit value to the given offset in the specified memory domain.";
                o.Parameters[0].Description = "Memory domain name (e.g., 'wram', 'cart0', 'bios')";
                o.Parameters[1].Description = "Address as hex string with 0x prefix (e.g., '0x03000000')";
                o.Parameters[2].Description = "32-bit value to write (0-4294967295)";
                o.Responses["200"].Description = "Empty success response";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("");
                return o;
            });

            group.MapPost("/write8", async (ObjectPool<ReusableSocket> socketPool, string memoryDomain, string address, int value) =>
            {
                var messageModel = new MessageModel("memoryDomain.write8", memoryDomain.ToString(), address, value.ToString()).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Write an 8-bit value to the given offset.";
                o.Description = "Write an 8-bit value to the given offset in the specified memory domain.";
                o.Parameters[0].Description = "Memory domain name (e.g., 'wram', 'cart0', 'bios')";
                o.Parameters[1].Description = "Address as hex string with 0x prefix (e.g., '0x03000000')";
                o.Parameters[2].Description = "8-bit value to write (0-255)";
                o.Responses["200"].Description = "Empty success response";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("");
                return o;
            });

            return group;
        }
    }
}

