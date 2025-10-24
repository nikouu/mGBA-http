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
                o.Summary = "Get the address of the base of this memory domain.";
                o.Description = "Get the address of the base of this memory domain.";
                return o;
            });

            group.MapGet("/bound", async (ObjectPool<ReusableSocket> socketPool, string memoryDomain) =>
            {
                var messageModel = new MessageModel("memoryDomain.bound", memoryDomain.ToString()).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Get the address of the end bound of this memory domain.";
                o.Description = "Get the address of the end bound of this memory domain. Note that this address is not in the domain itself, and is the address of the first byte past it.";
                return o;
            });

            group.MapGet("/name", async (ObjectPool<ReusableSocket> socketPool, string memoryDomain) =>
            {
                var messageModel = new MessageModel("memoryDomain.name", memoryDomain.ToString()).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Get a short, human-readable name for this memory domain.";
                o.Description = "Get a short, human-readable name for this memory domain.";
                return o;
            });

            group.MapGet("/read16", async (ObjectPool<ReusableSocket> socketPool, string memoryDomain, string address) =>
            {
                var messageModel = new MessageModel("memoryDomain.read16", memoryDomain.ToString(), address).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Read a 16-bit value from the given offset.";
                o.Description = "Read a 16-bit value from the given offset.";
                o.Parameters[1].Description = "Address in hex, e.g. 0x03000000";
                return o;
            });

            group.MapGet("/read32", async (ObjectPool<ReusableSocket> socketPool, string memoryDomain, string address) =>
            {
                var messageModel = new MessageModel("memoryDomain.read32", memoryDomain.ToString(), address).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Read a 32-bit value from the given offset.";
                o.Description = "Read a 32-bit value from the given offset.";
                o.Parameters[1].Description = "Address in hex, e.g. 0x03000000";
                return o;
            });

            group.MapGet("/read8", async (ObjectPool<ReusableSocket> socketPool, string memoryDomain, string address) =>
            {
                var messageModel = new MessageModel("memoryDomain.read8", memoryDomain.ToString(), address).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Read an 8-bit value from the given offset.";
                o.Description = "Read an 8-bit value from the given offset.";
                o.Parameters[1].Description = "Address in hex, e.g. 0x03000000";
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
                o.Description = "Read byte range from the given offset and returns an array of unsigned integers";
                o.Parameters[1].Description = "Address in hex, e.g. 0x03000000";
                return o;
            });

            group.MapGet("/size", async (ObjectPool<ReusableSocket> socketPool, string memoryDomain) =>
            {
                var messageModel = new MessageModel("memoryDomain.size", memoryDomain.ToString()).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Get the size of this memory domain in bytes.";
                o.Description = "Get the size of this memory domain in bytes.";
                return o;
            });

            group.MapPost("/write16", async (ObjectPool<ReusableSocket> socketPool, string memoryDomain, string address, int value) =>
            {
                var messageModel = new MessageModel("memoryDomain.write16", memoryDomain.ToString(), address, value.ToString()).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Write a 16-bit value from the given offset.";
                o.Description = "Write a 16-bit value from the given offset.";
                o.Parameters[1].Description = "Address in hex, e.g. 0x03000000";
                return o;
            });

            group.MapPost("/write32", async (ObjectPool<ReusableSocket> socketPool, string memoryDomain, string address, int value) =>
            {
                var messageModel = new MessageModel("memoryDomain.write32", memoryDomain.ToString(), address, value.ToString()).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Write a 32-bit value from the given offset.";
                o.Description = "Write a 32-bit value from the given offset.";
                o.Parameters[1].Description = "Address in hex, e.g. 0x03000000";
                return o;
            });

            group.MapPost("/write8", async (ObjectPool<ReusableSocket> socketPool, string memoryDomain, string address, int value) =>
            {
                var messageModel = new MessageModel("memoryDomain.write8", memoryDomain.ToString(), address, value.ToString()).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Write an 8-bit value from the given offset.";
                o.Description = "Write an 8-bit value from the given offset.";
                o.Parameters[1].Description = "Address in hex, e.g. 0x03000000";
                return o;
            });

            return group;
        }
    }
}

