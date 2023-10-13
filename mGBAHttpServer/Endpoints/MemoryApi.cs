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

            group.MapGet("/gameboyadvance/base", async (SocketService socket, GBAMemoryDomainsEnum memoryDomain) =>
            {
                return await socket.SendMessageAsync(new MessageModel("memorydomain.gameboyadvance.base", memoryDomain.ToString()));
            });

            group.MapGet("/gameboyadvance/bound", async (SocketService socket, GBAMemoryDomainsEnum memoryDomain) =>
            {
                return await socket.SendMessageAsync(new MessageModel("memorydomain.gameboyadvance.bound", memoryDomain.ToString()));
            });

            group.MapGet("/gameboyadvance/name", async (SocketService socket, GBAMemoryDomainsEnum memoryDomain) =>
            {
                return await socket.SendMessageAsync(new MessageModel("memorydomain.gameboyadvance.name", memoryDomain.ToString()));
            });

            group.MapGet("/gameboyadvance/read16", async (SocketService socket, GBAMemoryDomainsEnum memoryDomain, string address) =>
            {
                return await socket.SendMessageAsync(new MessageModel("memorydomain.gameboyadvance.read16", memoryDomain.ToString(), address));
            });

            group.MapGet("/gameboyadvance/read32", async (SocketService socket, GBAMemoryDomainsEnum memoryDomain, string address) =>
            {
                return await socket.SendMessageAsync(new MessageModel("memorydomain.gameboyadvance.read32", memoryDomain.ToString(), address));
            });

            group.MapGet("/gameboyadvance/read8", async (SocketService socket, GBAMemoryDomainsEnum memoryDomain, string address) =>
            {
                return await socket.SendMessageAsync(new MessageModel("memorydomain.gameboyadvance.read8", memoryDomain.ToString(), address));
            });

            group.MapGet("/gameboyadvance/size", async (SocketService socket, GBAMemoryDomainsEnum memoryDomain) =>
            {
                return await socket.SendMessageAsync(new MessageModel("memorydomain.gameboyadvance.size", memoryDomain.ToString()));
            });


            return group;
        }
    }
}
