using mGBAHttpServer.Models;
using mGBAHttpServer.Services;

namespace mGBAHttpServer.Endpoints
{
    public static class MemoryDomainApi
    {
        public static RouteGroupBuilder MapMemoryDomainEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/memorydomain");
            group.WithTags("MemoryDomain");

            group.MapGet("/base", async (SocketService socket, string memoryDomain) =>
            {
                return await socket.SendMessageAsync(new MessageModel("memoryDomain.base", memoryDomain.ToString()));
            });

            group.MapGet("/bound", async (SocketService socket, string memoryDomain) =>
            {
                return await socket.SendMessageAsync(new MessageModel("memoryDomain.bound", memoryDomain.ToString()));
            });

            group.MapGet("/name", async (SocketService socket, string memoryDomain) =>
            {
                return await socket.SendMessageAsync(new MessageModel("memoryDomain.name", memoryDomain.ToString()));
            });

            group.MapGet("/read16", async (SocketService socket, string memoryDomain, string address) =>
            {
                return await socket.SendMessageAsync(new MessageModel("memoryDomain.read16", memoryDomain.ToString(), address));
            });

            group.MapGet("/read32", async (SocketService socket, string memoryDomain, string address) =>
            {
                return await socket.SendMessageAsync(new MessageModel("memoryDomain.read32", memoryDomain.ToString(), address));
            });

            group.MapGet("/read8", async (SocketService socket, string memoryDomain, string address) =>
            {
                return await socket.SendMessageAsync(new MessageModel("memoryDomain.read8", memoryDomain.ToString(), address));
            });

            group.MapGet("/readrange", async (SocketService socket, string memoryDomain, string address, string length) =>
            {
                return await socket.SendMessageAsync(new MessageModel("memoryDomain.readRange", memoryDomain.ToString(), address, length));
            });

            group.MapGet("/size", async (SocketService socket, string memoryDomain) =>
            {
                return await socket.SendMessageAsync(new MessageModel("memoryDomain.size", memoryDomain.ToString()));
            });

            group.MapPost("/write16", async (SocketService socket, string memoryDomain, string address, string value) =>
            {
                return await socket.SendMessageAsync(new MessageModel("memoryDomain.write16", memoryDomain.ToString(), address, value));
            });

            group.MapPost("/write32", async (SocketService socket, string memoryDomain, string address, string value) =>
            {
                return await socket.SendMessageAsync(new MessageModel("memoryDomain.write32", memoryDomain.ToString(), address, value));
            });

            group.MapPost("/write8", async (SocketService socket, string memoryDomain, string address, string value) =>
            {
                return await socket.SendMessageAsync(new MessageModel("memoryDomain.write8", memoryDomain.ToString(), address, value));
            });

            return group;
        }
    }
}
