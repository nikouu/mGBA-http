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

            // TODO: come back and potentially rename this path and endpoint to be more descriptive such as /custom/button/press
            group.MapPost("/", async (SocketService socket, KeysEnum key) =>
            {
                await socket.SendMessageAsync(new MessageModel("custom.button", key.ToString()));
            });            

            return group;
        }
    }
}
