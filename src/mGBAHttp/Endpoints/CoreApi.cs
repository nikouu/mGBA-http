using mGBAHttp.Domain;
using mGBAHttp.Models;
using Microsoft.Extensions.ObjectPool;

namespace mGBAHttp.Endpoints
{
    public static class CoreApi
    {
        public static RouteGroupBuilder MapCoreEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/core");
            group.WithTags("Core");

            group.MapPost("/addkey", async (ObjectPool<ReusableSocket> socketPool, KeysEnum key) =>
            {
                var messageModel = new MessageModel("core.addKey", key.ToString()).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Add a single key.";
                o.Description = "Add a single key to the currently active key list. As if the key were held down.";
                o.Parameters[0].Description = "Key value of: A, B, Start, Select, Start, Right, Left, Up, Down, R, or L.";
                return o;
            });

            group.MapPost("/addkeys", async (ObjectPool<ReusableSocket> socketPool, int keyBitmask) =>
            {
                var messageModel = new MessageModel("core.addKeys", keyBitmask.ToString()).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Add a bitmask of keys.";
                o.Description = "Add a bitmask of keys to the currently active key list. As if the keys were held down.";
                o.Parameters[0].Description = "Bitmasking has the keys from the mGBA scripting documentation where each key has a value that goes up in that order in binary: A = 1, B = 2, Select = 4, Start = 8, etc. A bitmask of 12 is Start and Select.";
                return o;
            });

            group.MapPost("/autoloadsave", async (ObjectPool<ReusableSocket> socketPool) =>
            {
                var messageModel = new MessageModel("core.autoloadSave").ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Load save data.";
                o.Description = "Load the save data associated with the currently loaded ROM file.";
                return o;
            });

            group.MapGet("/checksum", async (ObjectPool<ReusableSocket> socketPool) =>
            {
                var messageModel = new MessageModel("core.checksum").ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Get the checksum of the loaded ROM.";
                o.Description = "Get the checksum of the loaded ROM as base 10.";
                return o;
            });

            group.MapPost("/clearkey", async (ObjectPool<ReusableSocket> socketPool, KeysEnum key) =>
            {
                var messageModel = new MessageModel("core.clearKey", key.ToString()).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Remove a single key.";
                o.Description = "Remove a single key from the currently active key list. As if the key were released.";
                o.Parameters[0].Description = "Key value of: A, B, Start, Select, Start, Right, Left, Up, Down, R, or L.";
                return o;
            });

            group.MapPost("/clearkeys", async (ObjectPool<ReusableSocket> socketPool, int keyBitmask) =>
            {
                var messageModel = new MessageModel("core.clearKeys", keyBitmask.ToString()).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Remove a bitmask of keys.";
                o.Description = "Remove a bitmask of keys from the currently active key list. As if the keys were released.";
                o.Parameters[0].Description = "Bitmasking has the keys from the mGBA scripting documentation where each key has a value that goes up in that order in binary: A = 1, B = 2, Select = 4, Start = 8, etc. A bitmask of 12 is Start and Select.";
                return o;
            });

            group.MapGet("/currentFrame", async (ObjectPool<ReusableSocket> socketPool) =>
            {
                var messageModel = new MessageModel("core.currentFrame").ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Get the number of the current frame.";
                o.Description = "Get the number of the current frame.";
                return o;
            });

            group.MapGet("/framecycles", async (ObjectPool<ReusableSocket> socketPool) =>
            {
                var messageModel = new MessageModel("core.frameCycles").ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Get the number of cycles per frame.";
                o.Description = "Get the number of cycles per frame.";
                return o;
            });

            group.MapGet("/frequency", async (ObjectPool<ReusableSocket> socketPool) =>
            {
                var messageModel = new MessageModel("core.frequency").ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Get the number of cycles per second.";
                o.Description = "Get the number of cycles per second.";
                return o;
            });

            group.MapGet("/getgamecode", async (ObjectPool<ReusableSocket> socketPool) =>
            {
                var messageModel = new MessageModel("core.getGameCode").ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Get internal product code for the game.";
                o.Description = "Get internal product code for the game from the ROM header, if available.";
                return o;
            });

            group.MapGet("/getgametitle", async (ObjectPool<ReusableSocket> socketPool) =>
            {
                var messageModel = new MessageModel("core.getGameTitle").ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Get internal title of the game.";
                o.Description = "Get internal title of the game from the ROM header.";
                return o;
            });

            group.MapGet("/getkey", async (ObjectPool<ReusableSocket> socketPool, KeysEnum key) =>
            {
                var messageModel = new MessageModel("core.getKey", key.ToString()).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Get the active state of a given key.";
                o.Description = "Get the active state of a given key.";
                o.Parameters[0].Description = "Key value of: A, B, Start, Select, Start, Right, Left, Up, Down, R, or L.";
                return o;
            });

            group.MapGet("/getkeys", async (ObjectPool<ReusableSocket> socketPool) =>
            {
                var messageModel = new MessageModel("core.getKeys").ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Get the currently active keys as a bitmask.";
                o.Description = "Get the currently active keys as a bitmask.";
                return o;
            });

            group.MapPost("/loadfile", async (ObjectPool<ReusableSocket> socketPool, string path) =>
            {
                var messageModel = new MessageModel("core.loadFile", path).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Load a ROM file.";
                o.Description = "Load a ROM file into the current state of this core. **Note: Prefer using the /mgba-http/extension/loadfile endpoint.**";
                return o;
            });

            group.MapPost("/loadsavefile", async (ObjectPool<ReusableSocket> socketPool, string path, bool temporary) =>
            {
                var messageModel = new MessageModel("core.loadSaveFile", path, temporary.ToString()).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Load save data file.";
                o.Description = "Load save data from the given path. If the temporary flag is set, the given save data will not be written back to disk.";
                return o;
            });

            group.MapPost("/loadstatebuffer", async (ObjectPool<ReusableSocket> socketPool, string buffer, int flags = 29) =>
            {
                var messageModel = new MessageModel("core.loadStateBuffer", buffer, flags.ToString()).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Load state from a buffer.";
                o.Description = "Load state from a buffer.";
                return o;
            });

            group.MapPost("/loadstatefile", async (ObjectPool<ReusableSocket> socketPool, string path, int flags = 29) =>
            {
                var messageModel = new MessageModel("core.loadStateFile", path, flags.ToString()).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Load state from the given path.";
                o.Description = "Load state from the given path.";
                return o;
            });

            group.MapPost("/loadstateslot", async (ObjectPool<ReusableSocket> socketPool, string slot, int flags = 29) =>
            {
                var messageModel = new MessageModel("core.loadStateSlot", slot, flags.ToString()).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Load state from the slot number.";
                o.Description = "Load state from the slot number.";
                return o;
            });

            group.MapGet("/platform", async (ObjectPool<ReusableSocket> socketPool) =>
            {
                var messageModel = new MessageModel("core.platform").ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Get which platform is being emulated.";
                o.Description = "Get which platform is being emulated.";
                return o;
            });

            group.MapGet("/read16", async (ObjectPool<ReusableSocket> socketPool, string address) =>
            {
                var messageModel = new MessageModel("core.read16", address).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Read a 16-bit value from the given bus address.";
                o.Description = "Read a 16-bit value from the given bus address.";
                o.Parameters[0].Description = "Address in hex, e.g. 0x03000000";
                return o;
            });

            group.MapGet("/read32", async (ObjectPool<ReusableSocket> socketPool, string address) =>
            {
                var messageModel = new MessageModel("core.read32", address).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Read a 32-bit value from the given bus address.";
                o.Description = "Read a 32-bit value from the given bus address.";
                o.Parameters[0].Description = "Address in hex, e.g. 0x03000000";
                return o;
            });

            group.MapGet("/read8", async (ObjectPool<ReusableSocket> socketPool, string address) =>
            {
                var messageModel = new MessageModel("core.read8", address).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Read an 8-bit value from the given bus address.";
                o.Description = "Read an 8-bit value from the given bus address.";
                o.Parameters[0].Description = "Address in hex, e.g. 0x03000000";
                return o;
            });

            // TODO: come back to these input datatypes for all the u32 and s32 types in the mGBA api documentation
            group.MapGet("/readrange", async (ObjectPool<ReusableSocket> socketPool, string address, string length) =>
            {
                var messageModel = new MessageModel("core.readRange", address, length).ToString();
                var result = await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
                return result.Split(',', StringSplitOptions.TrimEntries).Select(x => int.Parse(x));
            }).WithOpenApi(o =>
            {
                o.Summary = "Read byte range from the given offset.";
                o.Description = "Read byte range from the given offset and returns an array of unsigned integers";
                o.Parameters[0].Description = "Address in hex, e.g. 0x03000000";
                return o;
            });

            // todo: why is this int.parse and the /romsize request isnt?
            group.MapGet("/readregister", async (ObjectPool<ReusableSocket> socketPool, string regName) =>
            {
                var messageModel = new MessageModel("core.readRegister", regName).ToString();
                var result = await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
                return int.Parse(result);
            }).WithOpenApi(o =>
            {
                o.Summary = "Read the value of the register with the given name.";
                o.Description = "Read the value of the register with the given name.";
                return o;
            });

            group.MapGet("/romsize", async (ObjectPool<ReusableSocket> socketPool) =>
            {
                var messageModel = new MessageModel("core.romSize").ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Get the size of the loaded ROM.";
                o.Description = "Get the size of the loaded ROM.";
                return o;
            });

            group.MapPost("/runFrame", async (ObjectPool<ReusableSocket> socketPool) =>
            {
                var messageModel = new MessageModel("core.runFrame").ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Run until the next frame.";
                o.Description = "Run until the next frame.";
                return o;
            });

            group.MapPost("/savestatebuffer", async (ObjectPool<ReusableSocket> socketPool, int flags = 31) =>
            {
                var messageModel = new MessageModel("core.saveStateBuffer", flags.ToString()).ToString();
                var result = await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
                return result.Split(',', StringSplitOptions.TrimEntries).Select(x => int.Parse(x));
            }).WithOpenApi(o =>
            {
                o.Summary = "Save state and return as a buffer.";
                o.Description = "Save state and returns an array of unsigned integers";
                return o;
            });

            group.MapPost("/savestatefile", async (ObjectPool<ReusableSocket> socketPool, string path, int flags = 31) =>
            {
                var messageModel = new MessageModel("core.saveStateFile", path, flags.ToString()).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Save state to the given path.";
                o.Description = "Save state to the given path.";
                return o;
            });

            group.MapPost("/savestateslot", async (ObjectPool<ReusableSocket> socketPool, string slot, int flags = 31) =>
            {
                var messageModel = new MessageModel("core.saveStateSlot", slot, flags.ToString()).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Save state to the slot number.";
                o.Description = "Save state to the slot number.";
                return o;
            });

            group.MapPost("/screenshot", async (ObjectPool<ReusableSocket> socketPool, string path) =>
            {
                var messageModel = new MessageModel("core.screenshot", path).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Save a screenshot.";
                o.Description = "Save a screenshot.";
                o.Parameters[0].Description = @"For example, C:\screenshots\screenshot.png";
                return o;
            });

            group.MapPost("/setkeys", async (ObjectPool<ReusableSocket> socketPool, int keys) =>
            {
                var messageModel = new MessageModel("core.setKeys", keys.ToString()).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Set the currently active key list.";
                o.Description = "Set the currently active key list.";
                return o;
            });

            group.MapPost("/step", async (ObjectPool<ReusableSocket> socketPool) =>
            {
                var messageModel = new MessageModel("core.step").ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Run a single instruction.";
                o.Description = "Run a single instruction.";
                return o;
            });

            group.MapPost("/write16", async (ObjectPool<ReusableSocket> socketPool, string address, int value) =>
            {
                var messageModel = new MessageModel("core.write16", address, value.ToString()).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Write a 16-bit value from the given bus address.";
                o.Description = "Write a 16-bit value from the given bus address.";
                o.Parameters[0].Description = "Address in hex, e.g. 0x03000000";
                return o;
            });

            group.MapPost("/write32", async (ObjectPool<ReusableSocket> socketPool, string address, int value) =>
            {
                var messageModel = new MessageModel("core.write32", address, value.ToString()).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Write a 32-bit value from the given bus address.";
                o.Description = "Write a 32-bit value from the given bus address.";
                o.Parameters[0].Description = "Address in hex, e.g. 0x03000000";
                return o;
            });

            group.MapPost("/write8", async (ObjectPool<ReusableSocket> socketPool, string address, int value) =>
            {
                var messageModel = new MessageModel("core.write8", address, value.ToString()).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Write an 8-bit value from the given bus address.";
                o.Description = "Write an 8-bit value from the given bus address.";
                o.Parameters[0].Description = "Address in hex, e.g. 0x03000000";
                return o;
            });

            group.MapPost("/writeregister", async (ObjectPool<ReusableSocket> socketPool, string regName, int value) =>
            {
                var messageModel = new MessageModel("core.writeRegister", regName, value.ToString()).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Write the value of the register with the given name.";
                o.Description = "Write the value of the register with the given name.";
                o.Parameters[0].Description = "Address in hex, e.g. 0x03000000";
                return o;
            });

            return group;
        }
    }
}
