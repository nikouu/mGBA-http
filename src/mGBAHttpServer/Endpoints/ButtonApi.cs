using mGBAHttpServer.Models;
using mGBAHttpServer.Services;

namespace mGBAHttpServer.Endpoints
{
    public static class ButtonApi
    {
        public static RouteGroupBuilder MapButtonEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/mgba-http/button");
            group.WithTags("Button");

            group.MapPost("/", async (SocketService socket, KeysEnum key) =>
            {
                await socket.SendMessageAsync(new MessageModel("mgba-http.button", key.ToString()));
            }).WithOpenApi(o =>
            {
                o.Summary = "Sends button presses.";
                o.Description = "A custom convenience API that implements a key press and release. This is as opposed to the key based core API that sends only either a press or release message.";
                o.Parameters[0].Description = "Key value of: A, B, Start, Select, Start, Right, Left, Up, Down, R, or L.";
                return o;
            });

            return group;
        }
    }
}
