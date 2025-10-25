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
                o.Description = "Print an error to the console. This will be shown as red text on light red background.";
                o.Parameters[0].Description = "The error message to display in console.";
                o.Responses["200"].Description = "Empty success response.";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("");
                return o;
            });

            group.MapPost("/log", async (ObjectPool<ReusableSocket> socketPool, string message) =>
            {
                var messageModel = new MessageModel("console.log", message).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Print a log to the console.";
                o.Description = "Print a log to the console. This will be shown as regular text.";
                o.Parameters[0].Description = "The log message to display in console.";
                o.Responses["200"].Description = "Empty success response.";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("");
                return o;
            });

            group.MapPost("/warn", async (ObjectPool<ReusableSocket> socketPool, string message) =>
            {
                var messageModel = new MessageModel("console.warn", message).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Print a warning to the console.";
                o.Description = "Print a warning to the console. This will be shown as yellow text on light yellow background.";
                o.Parameters[0].Description = "The warning message to display in console.";
                o.Responses["200"].Description = "Empty success response.";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("");
                return o;
            });

            return group;
        }
    }
}
