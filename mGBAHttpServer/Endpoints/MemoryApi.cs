using mGBAHttpServer.Models;
using mGBAHttpServer.Services;

namespace mGBAHttpServer.Endpoints
{
    public static class MemoryApi
    {
        public static RouteGroupBuilder MapMemoryEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/memory");
            group.WithTags("Memory");

            group.MapGet("/base", async (SocketService socket, string memoryDomain) =>
            {
                return await socket.SendMessageAsync(new MessageModel("memorydomain.base", memoryDomain.ToString()));
            });

            group.MapGet("/bound", async (SocketService socket, string memoryDomain) =>
            {
                return await socket.SendMessageAsync(new MessageModel("memorydomain.bound", memoryDomain.ToString()));
            });

            group.MapGet("/name", async (SocketService socket, string memoryDomain) =>
            {
                return await socket.SendMessageAsync(new MessageModel("memorydomain.name", memoryDomain.ToString()));
            });

            group.MapGet("/read16", async (SocketService socket, string memoryDomain, string address) =>
            {
                return await socket.SendMessageAsync(new MessageModel("memorydomain.read16", memoryDomain.ToString(), address));
            });

            group.MapGet("/read32", async (SocketService socket, string memoryDomain, string address) =>
            {
                return await socket.SendMessageAsync(new MessageModel("memorydomain.read32", memoryDomain.ToString(), address));
            });

            group.MapGet("/read8", async (SocketService socket, string memoryDomain, string address) =>
            {
                return await socket.SendMessageAsync(new MessageModel("memorydomain.read8", memoryDomain.ToString(), address));
            });

            group.MapGet("/size", async (SocketService socket, string memoryDomain) =>
            {
                return await socket.SendMessageAsync(new MessageModel("memorydomain.size", memoryDomain.ToString()));
            });

            return group;
        }
    }
}
