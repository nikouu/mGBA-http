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

            group.MapPost("/error", Error);
            group.MapPost("/log", Log);
            group.MapPost("/warn", Warn);

            return group;
        }

        /// <summary>Print an error to the console.</summary>
        /// <remarks>Print an error to the console. This will be shown as red text on light red background.</remarks>
        /// <param name="message">The error message to display in console.</param>
        /// <response code="200">Empty success response.</response>
        public static async Task<string> Error(ObjectPool<ReusableSocket> socketPool, string message)
        {
            var messageModel = new MessageModel("console.error", message).ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Print a log to the console.</summary>
        /// <remarks>Print a log to the console. This will be shown as regular text.</remarks>
        /// <param name="message">The log message to display in console.</param>
        /// <response code="200">Empty success response.</response>
        public static async Task<string> Log(ObjectPool<ReusableSocket> socketPool, string message)
        {
            var messageModel = new MessageModel("console.log", message).ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Print a warning to the console.</summary>
        /// <remarks>Print a warning to the console. This will be shown as yellow text on light yellow background.</remarks>
        /// <param name="message">The warning message to display in console.</param>
        /// <response code="200">Empty success response.</response>
        public static async Task<string> Warn(ObjectPool<ReusableSocket> socketPool, string message)
        {
            var messageModel = new MessageModel("console.warn", message).ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }
    }
}
