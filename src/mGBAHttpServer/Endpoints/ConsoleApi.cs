using mGBAHttpServer.Models;
using mGBAHttpServer.Services;

namespace mGBAHttpServer.Endpoints
{
    public static class ConsoleApi
    {
        public static RouteGroupBuilder MapConsoleEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/console");
            group.WithTags("Console");

            group.MapPost("/error", async (SocketService socket, string message) =>
            {
                await socket.SendMessageAsync(new MessageModel("console.error", message));
            }).WithOpenApi(o =>
            {
                o.Summary = "Print an error to the console.";
                o.Description = "Presents textual information to the user via a console.";
                return o;
            });

            group.MapPost("/log", async (SocketService socket, string message) =>
            {
                await socket.SendMessageAsync(new MessageModel("console.log", message));
            }).WithOpenApi(o =>
            {
                o.Summary = "Print a log to the console.";
                o.Description = "Presents textual information to the user via a console.";
                return o;
            });

            group.MapPost("/warn", async (SocketService socket, string message) =>
            {
                await socket.SendMessageAsync(new MessageModel("console.warn", message));
            }).WithOpenApi(o =>
            {
                o.Summary = "Print a warning to the console.";
                o.Description = "Presents textual information to the user via a console.";
                return o;
            });

            return group;
        }
    }
}
