using mGBAHttp.Domain;
using mGBAHttp.Models;
using Microsoft.Extensions.ObjectPool;

namespace mGBAHttp.Endpoints
{
    public static class ConsoleApi
    {
        public static RouteGroupBuilder MapConsoleEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/console");
            group.WithTags("Console");

            group.MapPost("/error", async (ObjectPool<ReusableSocket> socketPool, string message) =>
            {
                var messageModel = new MessageModel("console.error", message).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Print an error to the console.";
                o.Description = "Presents textual information to the user via a console.";
                return o;
            });

            group.MapPost("/log", async (ObjectPool<ReusableSocket> socketPool, string message) =>
            {
                var messageModel = new MessageModel("console.log", message).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Print a log to the console.";
                o.Description = "Presents textual information to the user via a console.";
                return o;
            });

            group.MapPost("/warn", async (ObjectPool<ReusableSocket> socketPool, string message) =>
            {
                var messageModel = new MessageModel("console.warn", message).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
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
