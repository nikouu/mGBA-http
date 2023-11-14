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
            });            

            return group;
        }
    }
}
