using mGBAHttp.Domain;
using mGBAHttp.Models;
using mGBAHttp.OpenApi;
using Microsoft.Extensions.ObjectPool;

namespace mGBAHttp.Endpoints
{
    public static class MemoryDomainApi
    {
        public static RouteGroupBuilder MapMemoryDomainEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/memorydomain");
            group.WithTags("MemoryDomain");

            group.MapGet("/base", Base).WithMetadata(new ResponseExample("50331648"));
            group.MapGet("/bound", Bound).WithMetadata(new ResponseExample("50594303"));
            group.MapGet("/name", Name).WithMetadata(new ResponseExample("wram"));
            group.MapGet("/read16", Read16).WithMetadata(new ResponseExample("65535"));
            group.MapGet("/read32", Read32).WithMetadata(new ResponseExample("4294967295"));
            group.MapGet("/read8", Read8).WithMetadata(new ResponseExample("255"));
            group.MapGet("/readrange", ReadRange).WithMetadata(new ResponseExample("d3,00,00,ea,66,00,00,ea"));
            group.MapGet("/size", Size).WithMetadata(new ResponseExample("262144"));
            group.MapPost("/write16", Write16);
            group.MapPost("/write32", Write32);
            group.MapPost("/write8", Write8);

            return group;
        }

        /// <summary>Get the base address of the memory domain.</summary>
        /// <remarks>Get the base address of the specified memory domain.</remarks>
        /// <param name="memoryDomain">Memory domain name (e.g., 'wram', 'cart0', 'bios', etc).</param>
        /// <response code="200">Address of the base of this memory domain.</response>
        public static async Task<string> Base(ObjectPool<ReusableSocket> socketPool, string memoryDomain)
        {
            var messageModel = new MessageModel("memoryDomain.base", memoryDomain.ToString()).ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Get the bound of the memory domain.</summary>
        /// <remarks>Get the address of the end bound of this memory domain. Note that this address is not in the domain itself, and is the address of the first byte past it.</remarks>
        /// <param name="memoryDomain">Memory domain name (e.g., 'wram', 'cart0', 'bios', etc).</param>
        /// <response code="200">Bound address as a string.</response>
        public static async Task<string> Bound(ObjectPool<ReusableSocket> socketPool, string memoryDomain)
        {
            var messageModel = new MessageModel("memoryDomain.bound", memoryDomain.ToString()).ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Get the name of the memory domain.</summary>
        /// <remarks>Get a short, human-readable name for this memory domain.</remarks>
        /// <param name="memoryDomain">Memory domain name (e.g., 'wram', 'cart0', 'bios', etc).</param>
        /// <response code="200">Memory domain name.</response>
        public static async Task<string> Name(ObjectPool<ReusableSocket> socketPool, string memoryDomain)
        {
            var messageModel = new MessageModel("memoryDomain.name", memoryDomain.ToString()).ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Read a 16-bit value from the given offset.</summary>
        /// <remarks>Read a 16-bit value from the given offset in the specified memory domain.</remarks>
        /// <param name="memoryDomain">Memory domain name (e.g., 'wram', 'cart0', 'bios', etc).</param>
        /// <param name="address">Address as hex string with 0x prefix (e.g., '0x0300')</param>
        /// <response code="200">16-bit value as a string.</response>
        public static async Task<string> Read16(ObjectPool<ReusableSocket> socketPool, string memoryDomain, string address)
        {
            var messageModel = new MessageModel("memoryDomain.read16", memoryDomain.ToString(), address).ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Read a 32-bit value from the given offset.</summary>
        /// <remarks>Read a 32-bit value from the given offset in the specified memory domain.</remarks>
        /// <param name="memoryDomain">Memory domain name (e.g., 'wram', 'cart0', 'bios', etc).</param>
        /// <param name="address">Address as hex string with 0x prefix (e.g., '0x0300')</param>
        /// <response code="200">32-bit value as a string.</response>
        public static async Task<string> Read32(ObjectPool<ReusableSocket> socketPool, string memoryDomain, string address)
        {
            var messageModel = new MessageModel("memoryDomain.read32", memoryDomain.ToString(), address).ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Read an 8-bit value from the given offset.</summary>
        /// <remarks>Read an 8-bit value from the given offset in the specified memory domain.</remarks>
        /// <param name="memoryDomain">Memory domain name (e.g., 'wram', 'cart0', 'bios', etc).</param>
        /// <param name="address">Address as hex string with 0x prefix (e.g., '0x0300')</param>
        /// <response code="200">8-bit value as a string.</response>
        public static async Task<string> Read8(ObjectPool<ReusableSocket> socketPool, string memoryDomain, string address)
        {
            var messageModel = new MessageModel("memoryDomain.read8", memoryDomain.ToString(), address).ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Read byte range from the given offset.</summary>
        /// <remarks>Read byte range from the given offset.</remarks>
        /// <param name="memoryDomain">Memory domain name (e.g., 'wram', 'cart0', 'bios', etc).</param>
        /// <param name="address">Address as hex string (e.g., '0x0300').</param>
        /// <param name="length">Number of bytes to read.</param>
        /// <response code="200">Comma separated hex values.</response>
        public static async Task<string> ReadRange(ObjectPool<ReusableSocket> socketPool, string memoryDomain, string address, string length)
        {
            var messageModel = new MessageModel("memoryDomain.readRange", memoryDomain.ToString(), address, length).ToString();
            var result = await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            return result;
        }

        /// <summary>Get the size of the memory domain.</summary>
        /// <remarks>Get the size of this memory domain in bytes.</remarks>
        /// <param name="memoryDomain">Memory domain name (e.g., 'wram', 'cart0', 'bios', etc).</param>
        /// <response code="200">Size in bytes as a string.</response>
        public static async Task<string> Size(ObjectPool<ReusableSocket> socketPool, string memoryDomain)
        {
            var messageModel = new MessageModel("memoryDomain.size", memoryDomain.ToString()).ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Write a 16-bit value from the given offset.</summary>
        /// <remarks>Write a 16-bit value from the given offset in the specified memory domain.</remarks>
        /// <param name="memoryDomain">Memory domain name (e.g., 'wram', 'cart0', 'bios', etc).</param>
        /// <param name="address">Address as hex string with 0x prefix (e.g., '0x0300').</param>
        /// <param name="value">16-bit unsigned integer value to write (0-65535). Not hex.</param>
        /// <response code="200">Empty success response.</response>
        public static async Task<string> Write16(ObjectPool<ReusableSocket> socketPool, string memoryDomain, string address, uint value)
        {
            var messageModel = new MessageModel("memoryDomain.write16", memoryDomain.ToString(), address, value.ToString()).ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Write a 32-bit value from the given offset.</summary>
        /// <remarks>Write a 32-bit value from the given offset in the specified memory domain.</remarks>
        /// <param name="memoryDomain">Memory domain name (e.g., 'wram', 'cart0', 'bios', etc).</param>
        /// <param name="address">Address as hex string with 0x prefix (e.g., '0x0300').</param>
        /// <param name="value">32-bit unsigned integer value to write (0-4294967295). Not hex.</param>
        /// <response code="200">Empty success response.</response>
        public static async Task<string> Write32(ObjectPool<ReusableSocket> socketPool, string memoryDomain, string address, uint value)
        {
            var messageModel = new MessageModel("memoryDomain.write32", memoryDomain.ToString(), address, value.ToString()).ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Write an 8-bit value from the given offset.</summary>
        /// <remarks>Write an 8-bit value from the given offset in the specified memory domain.</remarks>
        /// <param name="memoryDomain">Memory domain name (e.g., 'wram', 'cart0', 'bios', etc).</param>
        /// <param name="address">Address as hex string with 0x prefix (e.g., '0x0300').</param>
        /// <param name="value">8-bit integer value to write (0-255). Not hex.</param>
        /// <response code="200">Empty success response.</response>
        public static async Task<string> Write8(ObjectPool<ReusableSocket> socketPool, string memoryDomain, string address, int value)
        {
            var messageModel = new MessageModel("memoryDomain.write8", memoryDomain.ToString(), address, value.ToString()).ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }
    }
}
