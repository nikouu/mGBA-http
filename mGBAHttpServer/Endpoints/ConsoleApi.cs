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
                await socket.SendMessage(new MessageModel("console.error", message));
            });

            group.MapPost("/log", async (SocketService socket, string message) =>
            {
                await socket.SendMessage(new MessageModel("console.log", message));
            });

            group.MapPost("/warn", async (SocketService socket, string message) =>
            {
                await socket.SendMessage(new MessageModel("console.warn", message));
            });

            return group;
        }
    }
}
