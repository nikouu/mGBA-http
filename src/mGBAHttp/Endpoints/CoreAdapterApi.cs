using mGBAHttp.Domain;
using mGBAHttp.Models;
using mGBAHttp.OpenApi;
using Microsoft.Extensions.ObjectPool;

namespace mGBAHttp.Endpoints
{
    public static class CoreAdapterApi
    {
        public static RouteGroupBuilder MapCoreAdapterEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/coreadapter");
            group.WithTags("CoreAdapter");

            group.MapPost("/reset", Reset);
            group.MapGet("/memory", Memory).WithMetadata(new ResponseExample("cart2,wram,cart0,oam,iwram,bios,vram,io,palette,cart1"));

            return group;
        }

        /// <summary>Reset the emulation.</summary>
        /// <remarks>Reset the emulation and calls the reset callback.</remarks>
        /// <response code="200">Empty success response.</response>
        public static async Task<string> Reset(ObjectPool<ReusableSocket> socketPool)
        {
            var messageModel = new MessageModel("coreAdapter.reset").ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Get the names of the platform-specific memory domains.</summary>
        /// <remarks>Get the names of the platform-specific memory domains.</remarks>
        /// <response code="200">The memory domains as a comma separated string.</response>
        public static async Task<string> Memory(ObjectPool<ReusableSocket> socketPool)
        {
            var messageModel = new MessageModel("coreAdapter.memory").ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }
    }
}
