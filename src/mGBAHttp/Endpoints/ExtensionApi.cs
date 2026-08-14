using mGBAHttp.Domain;
using mGBAHttp.Models;
using mGBAHttp.OpenApi;
using Microsoft.Extensions.ObjectPool;

namespace mGBAHttp.Endpoints
{
    public static class ExtensionApi
    {
        public static RouteGroupBuilder MapExtensionEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/mgba-http/extension");
            group.WithTags("Extension");

            group.MapPost("/loadfile", LoadFile).WithMetadata(new ResponseExample("true"));

            return group;
        }

        /// <summary>Load a ROM file.</summary>
        /// <remarks>Load a ROM file into the current state of this core. This convenience API handles the ROM load and reset of the emulator.</remarks>
        /// <param name="path">Path to ROM file to load.</param>
        /// <response code="200">Success status as a boolean.</response>
        public static async Task<string> LoadFile(ObjectPool<ReusableSocket> socketPool, string path)
        {
            var messageModel = new MessageModel("mgba-http.extension.loadFile", path).ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }
    }
}
