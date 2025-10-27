using mGBAHttp.Domain;
using mGBAHttp.Models;
using Microsoft.Extensions.ObjectPool;

namespace mGBAHttp.Endpoints
{
    public static class ExtensionApi
    {
        public static RouteGroupBuilder MapExtensionEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/mgba-http/extension");
            group.WithTags("Extension");

            group.MapPost("/loadfile", async (ObjectPool<ReusableSocket> socketPool, string path) =>
            {
                var messageModel = new MessageModel("mgba-http.extension.loadFile", path).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Load a ROM file.";
                o.Description = "Load a ROM file into the current state of this core. This convenience API handles the ROM load and reset of the emulator.";
                o.Parameters[0].Description = "Path to ROM file to load.";
                o.Responses["200"].Description = "Success status as a boolean.";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("true");
                return o;
            });

            return group;
        }
    }
}
