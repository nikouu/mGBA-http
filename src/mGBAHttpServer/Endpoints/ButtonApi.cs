using mGBAHttpServer.Models;
using mGBAHttpServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace mGBAHttpServer.Endpoints
{
    public static class ButtonApi
    {
        public static RouteGroupBuilder MapButtonEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/mgba-http/button");
            group.WithTags("Button");

            group.MapPost("/tap", async (SocketService socket, KeysEnum key) =>
            {
                await socket.SendMessageAsync(new MessageModel("mgba-http.button.tap", key.ToString()));
            }).WithOpenApi(o =>
            {
                o.Summary = "Sends button presses.";
                o.Description = "A custom convenience API that implements a key press and release. This is as opposed to the key based core API that sends only either a press or release message.";
                o.Parameters[0].Description = "Key value of: A, B, Start, Select, Start, Right, Left, Up, Down, R, or L.";
                return o;
            });

            group.MapPost("/tapmany", async (SocketService socket, [FromQuery] KeysEnum[] keys) =>
            {
                await socket.SendMessageAsync(new MessageModel("mgba-http.button.tapmany", string.Join(";", keys)));
            }).WithOpenApi(o =>
            {
                o.Summary = "Sends multiple button presses simultaneously.";
                o.Description = "A custom convenience API that implements multiple simultaneously keys being pressed and released. This is as opposed to the key based core API that sends only either a press or release message.";
                o.Parameters[0].Description = "A key array.";
                return o;
            });

            group.MapPost("/hold", async (SocketService socket, KeysEnum key, int duration) =>
            {
                await socket.SendMessageAsync(new MessageModel("mgba-http.button.hold", key.ToString(), duration.ToString()));
            }).WithOpenApi(o =>
            {
                o.Summary = "Sends a held down button for a given duration in frames.";
                o.Description = "A custom convenience API that implements a held down button for a given duration in frames. This is as opposed to the key based core API that sends only either a press or release message.";
                o.Parameters[0].Description = "Key value of: A, B, Start, Select, Start, Right, Left, Up, Down, R, or L.";
                o.Parameters[1].Description = "Duration in frames.";
                return o;
            });

            group.MapPost("/holdmany", async (SocketService socket, [FromQuery] KeysEnum[] keys, int duration) =>
            {
                await socket.SendMessageAsync(new MessageModel("mgba-http.button.holdmany", string.Join(";", keys), duration.ToString()));
            }).WithOpenApi(o =>
            {
                o.Summary = "Sends multiple button presses simultaneously for a given duration in frames.";
                o.Description = "A custom convenience API that implements multiple simultaneously keys being pressed and released for a given duration in frames. This is as opposed to the key based core API that sends only either a press or release message.";
                o.Parameters[0].Description = "A key array.";
                o.Parameters[1].Description = "Duration in frames.";
                return o;
            });

            return group;
        }
    }
}
