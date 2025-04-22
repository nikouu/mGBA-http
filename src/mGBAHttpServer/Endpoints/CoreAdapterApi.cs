using mGBAHttpServer.Models;
using mGBAHttpServer.Services;

namespace mGBAHttpServer.Endpoints
{
    public static class CoreAdapterApi
    {
        public static RouteGroupBuilder MapCoreAdapterEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/coreadapter");
            group.WithTags("CoreAdapter");

            group.MapPost("/reset", async (SocketService socket) =>
            {
                await socket.SendMessageAsync(new MessageModel("coreAdapter.reset"));
            }).WithOpenApi(o =>
            {
                o.Summary = "Reset the emulation.";
                o.Description = "Reset the emulation and calls the reset callback.";
                return o;
            });

            group.MapPost("/memory", async (SocketService socket) =>
            {
                var memoryDomains = await socket.SendMessageAsync(new MessageModel("coreAdapter.memory"));
                return memoryDomains.Split(',', StringSplitOptions.TrimEntries);
            }).WithOpenApi(o =>
            {
                o.Summary = "Get the platform specific set of MemoryDomains.";
                o.Description = "Get the platform specific set of MemoryDomains as an array of strings.";
                return o;
            });

            return group;
        }
    }
}
