using mGBAHttpServer.Models;
using mGBAHttpServer.Services;

namespace mGBAHttpServer.Endpoints
{
    public static class ButtonApi
    {
        public static RouteGroupBuilder MapButtonEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/button");
            group.WithTags("Button");

            //waiting for .net8 i think
            //group.WithSummary("summary");
            //group.WithDescription("Description");

            group.MapPost("/", async (SocketService socket, KeysEnum key) =>
            {
                await socket.SendMessage(new MessageModel("custom.button", key.ToString()));
            });            

            return group;
        }
    }
}
