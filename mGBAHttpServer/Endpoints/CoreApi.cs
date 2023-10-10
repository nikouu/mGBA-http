using mGBAHttpServer.Models;
using mGBAHttpServer.Services;

namespace mGBAHttpServer.Endpoints
{
    public static class CoreApi
    {
        public static RouteGroupBuilder MapCoreEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/core");
            group.WithTags("Core");

            group.MapPost("/addkey", async (SocketService socket, KeysEnum key) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.addkey", key.ToString()));
            });

            group.MapPost("/addkeys", async (SocketService socket, string keyBitmask) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.addkeys", keyBitmask));
            });

            group.MapPost("/autoloadsave", async (SocketService socket) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.autoloadsave"));
            });

            group.MapGet("/checksum", async (SocketService socket) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.checksum"));
            });

            group.MapPost("/clearkey", async (SocketService socket, KeysEnum key) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.clearkey", key.ToString()));
            });

            group.MapPost("/clearkeys", async (SocketService socket, string keyBitmask) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.clearkeys", keyBitmask));
            });

            group.MapGet("/currentFrame", async (SocketService socket) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.currentframe"));
            });

            group.MapGet("/framecycles", async (SocketService socket) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.framecycles"));
            });

            group.MapGet("/frequency", async (SocketService socket) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.frequency"));
            });

            group.MapGet("/getgamecode", async (SocketService socket) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.getgamecode"));
            });

            group.MapGet("/getgametitle", async (SocketService socket) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.getgametitle"));
            });

            group.MapGet("/getkey", async (SocketService socket, KeysEnum key) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.getkey", key.ToString()));
            });

            group.MapGet("/getkeys", async (SocketService socket) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.getkeys"));
            });

            group.MapPost("/loadfile", async (SocketService socket, string path) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.loadfile", path));
            });

            group.MapGet("/platform", async (SocketService socket) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.platform"));
            });

            group.MapGet("/read16", async (SocketService socket, string address) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.read16", address));
            });

            group.MapGet("/read32", async (SocketService socket, string address) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.read32", address));
            });

            group.MapGet("/read8", async (SocketService socket, string address) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.read8", address));
            });

            group.MapGet("/readregister", async (SocketService socket, string regName) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.readregister", regName));
            });

            group.MapPost("/reset", async (SocketService socket) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.reset"));
            });

            group.MapGet("/romsize", async (SocketService socket) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.romsize"));
            });

            group.MapPost("/runFrame", async (SocketService socket) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.runFrame"));
            });

            group.MapPost("/savestatebuffer", async (SocketService socket, string flags) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.savestatebuffer", flags));
            });

            group.MapPost("/screenshot", async (SocketService socket, string path) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.screenshot", path));
            });

            group.MapPost("/setkeys", async (SocketService socket, string keys) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.setkeys", keys));
            });

            group.MapPost("/step", async (SocketService socket) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.step"));
            });

            return group;
        }
    }
}
