using mGBAHttp.Domain;
using mGBAHttp.Models;
using Microsoft.Extensions.ObjectPool;
using Microsoft.OpenApi.Models;

namespace mGBAHttp.Endpoints
{
    public static class CoreAdapterApi
    {
        public static RouteGroupBuilder MapCoreAdapterEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/coreadapter");
            group.WithTags("CoreAdapter");

            group.MapPost("/reset", async (ObjectPool<ReusableSocket> socketPool) =>
            {
                var messageModel = new MessageModel("coreAdapter.reset").ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Reset the emulation.";
                o.Description = "Reset the emulation and calls the reset callback.";
                o.Responses["200"].Description = "Empty success response.";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("");
                return o;
            });

            group.MapGet("/memory", async (ObjectPool<ReusableSocket> socketPool) =>
            {
                var messageModel = new MessageModel("coreAdapter.memory").ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Get the platform specific set of memory domains.";
                o.Description = "Get the platform specific set of memory domains.";
                o.Responses["200"].Description = "The memory domains as a comma separated string.";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("cart2,wram,cart0,oam,iwram,bios,vram,io,palette,cart1");
                return o;
            });

            return group;
        }
    }
}