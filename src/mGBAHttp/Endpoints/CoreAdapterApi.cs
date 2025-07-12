using mGBAHttp.Domain;
using mGBAHttp.Models;
using Microsoft.Extensions.ObjectPool;

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
                return o;
            });

            group.MapGet("/memory", async (ObjectPool<ReusableSocket> socketPool) =>
            {
                var messageModel = new MessageModel("coreAdapter.memory").ToString();
                var result = await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
                return result.Split(',', StringSplitOptions.TrimEntries);
            }).WithOpenApi(o =>
            {
                o.Summary = "Get the platform specific set of MemoryDomains.";
                o.Description = "Gets the platform specific set of memory domains as an array of strings.";
                return o;
            });

            return group;
        }
    }
}