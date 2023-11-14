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
                return await socket.SendMessageAsync(new MessageModel("core.addKey", key.ToString()));
            });

            group.MapPost("/addkeys", async (SocketService socket, string keyBitmask) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.addKeys", keyBitmask));
            });

            group.MapPost("/autoloadsave", async (SocketService socket) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.autoloadSave"));
            });

            group.MapGet("/checksum", async (SocketService socket) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.checksum"));
            });

            group.MapPost("/clearkey", async (SocketService socket, KeysEnum key) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.clearKey", key.ToString()));
            });

            group.MapPost("/clearkeys", async (SocketService socket, string keyBitmask) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.clearKeys", keyBitmask));
            });

            group.MapGet("/currentFrame", async (SocketService socket) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.currentFrame"));
            });

            group.MapGet("/framecycles", async (SocketService socket) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.frameCycles"));
            });

            group.MapGet("/frequency", async (SocketService socket) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.frequency"));
            });

            group.MapGet("/getgamecode", async (SocketService socket) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.getGameCode"));
            });

            group.MapGet("/getgametitle", async (SocketService socket) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.getGameTitle"));
            });

            group.MapGet("/getkey", async (SocketService socket, KeysEnum key) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.getKey", key.ToString()));
            });

            group.MapGet("/getkeys", async (SocketService socket) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.getKeys"));
            });

            group.MapPost("/loadfile", async (SocketService socket, string path) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.loadFile", path));
            });

            group.MapPost("/loadsavefile", async (SocketService socket, string path, bool temporary) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.loadSaveFile", path, temporary.ToString()));
            });

            group.MapPost("/loadstatebuffer", async (SocketService socket, string buffer, string flags) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.loadStateBuffer", buffer, flags));
            });

            group.MapPost("/loadstatefile", async (SocketService socket, string path, string flags) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.loadStateFile", path, flags));
            });

            group.MapPost("/loadstateslot", async (SocketService socket, string slot, string flags) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.loadStateSlot", slot, flags));
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

            // TODO: come back to these input datatypes for all the u32 and s32 types in the mGBA api documentation
            group.MapGet("/readrange", async (SocketService socket, string address, string length) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.readRange", address, length));
            });

            group.MapGet("/readregister", async (SocketService socket, string regName) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.readRegister", regName));
            });

            group.MapPost("/reset", async (SocketService socket) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.reset"));
            });

            group.MapGet("/romsize", async (SocketService socket) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.romSize"));
            });

            group.MapPost("/runFrame", async (SocketService socket) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.runFrame"));
            });

            group.MapPost("/savestatebuffer", async (SocketService socket, string flags) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.saveStateBuffer", flags));
            });

            group.MapPost("/savestatefile", async (SocketService socket, string path, string flags) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.saveStateFile", path, flags));
            });

            group.MapPost("/savestateslot", async (SocketService socket, string slot, string flags) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.saveStateSlot", slot, flags));
            });

            group.MapPost("/screenshot", async (SocketService socket, string path) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.screenshot", path));
            });

            group.MapPost("/setkeys", async (SocketService socket, string keys) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.setKeys", keys));
            });

            group.MapPost("/step", async (SocketService socket) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.step"));
            });

            group.MapPost("/write16", async (SocketService socket, string address, string value) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.write16", address, value));
            });

            group.MapPost("/write32", async (SocketService socket, string address, string value) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.write32", address, value));
            });

            group.MapPost("/write8", async (SocketService socket, string address, string value) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.write8", address, value));
            });

            group.MapPost("/writeregister", async (SocketService socket, string regName, string value) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.writeRegister", regName, value));
            });

            return group;
        }
    }
}
