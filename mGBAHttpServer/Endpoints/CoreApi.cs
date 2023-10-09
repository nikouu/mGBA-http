using mGBAHttpServer.Models;
using mGBAHttpServer.Services;

namespace mGBAHttpServer.Endpoints
{
    public static class CoreApi
    {
        public static RouteGroupBuilder MapCoreEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/core");
            group.WithTags("Core");

            //waiting for .net8 i think
            //group.WithSummary("summary");
            //group.WithDescription("Description");

            group.MapGet("/checksum", async (SocketService socket) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.checksum"));
            });

            group.MapGet("/currentFrame", async (SocketService socket) =>
            {
                var g = await socket.SendMessageAsync(new MessageModel("core.currentframe"));
                return g;
            });

            return group;
        }
    }
}
