﻿using mGBAHttpServer.Models;
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
            }).WithOpenApi(o =>
            {
                o.Summary = "Add a single key.";
                o.Description = "Add a single key to the currently active key list. As if the key were held down.";
                o.Parameters[0].Description = "Key value of: A, B, Start, Select, Start, Right, Left, Up, Down, R, or L.";
                return o;
            });

            group.MapPost("/addkeys", async (SocketService socket, int keyBitmask) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.addKeys", keyBitmask.ToString()));
            }).WithOpenApi(o =>
            {
                o.Summary = "Add a bitmask of keys.";
                o.Description = "Add a bitmask of keys to the currently active key list. As if the keys were held down.";
                o.Parameters[0].Description = "Bitmasking has the keys from the mGBA scripting documentation where each key has a value that goes up in that order in binary: A = 1, B = 2, Select = 4, Start = 8, etc. A bitmask of 12 is Start and Select.";
                return o;
            });

            group.MapPost("/autoloadsave", async (SocketService socket) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.autoloadSave"));
            }).WithOpenApi(o =>
            {
                o.Summary = "Load save data.";
                o.Description = "Load the save data associated with the currently loaded ROM file.";
                return o;
            });

            group.MapGet("/checksum", async (SocketService socket) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.checksum"));
            }).WithOpenApi(o =>
            {
                o.Summary = "Get the checksum of the loaded ROM.";
                o.Description = "Get the checksum of the loaded ROM as base 10.";
                return o;
            });

            group.MapPost("/clearkey", async (SocketService socket, KeysEnum key) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.clearKey", key.ToString()));
            }).WithOpenApi(o =>
            {
                o.Summary = "Remove a single key.";
                o.Description = "Remove a single key from the currently active key list. As if the key were released.";
                o.Parameters[0].Description = "Key value of: A, B, Start, Select, Start, Right, Left, Up, Down, R, or L.";
                return o;
            });

            group.MapPost("/clearkeys", async (SocketService socket, int keyBitmask) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.clearKeys", keyBitmask.ToString()));
            }).WithOpenApi(o =>
            {
                o.Summary = "Remove a bitmask of keys.";
                o.Description = "Remove a bitmask of keys from the currently active key list. As if the keys were released.";
                o.Parameters[0].Description = "Bitmasking has the keys from the mGBA scripting documentation where each key has a value that goes up in that order in binary: A = 1, B = 2, Select = 4, Start = 8, etc. A bitmask of 12 is Start and Select.";
                return o;
            });

            group.MapGet("/currentFrame", async (SocketService socket) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.currentFrame"));
            }).WithOpenApi(o =>
            {
                o.Summary = "Get the number of the current frame.";
                o.Description = "Get the number of the current frame.";
                return o;
            });

            group.MapGet("/framecycles", async (SocketService socket) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.frameCycles"));
            }).WithOpenApi(o =>
            {
                o.Summary = "Get the number of cycles per frame.";
                o.Description = "Get the number of cycles per frame.";
                return o;
            });

            group.MapGet("/frequency", async (SocketService socket) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.frequency"));
            }).WithOpenApi(o =>
            {
                o.Summary = "Get the number of cycles per second.";
                o.Description = "Get the number of cycles per second.";
                return o;
            });

            group.MapGet("/getgamecode", async (SocketService socket) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.getGameCode"));
            }).WithOpenApi(o =>
            {
                o.Summary = "Get internal product code for the game.";
                o.Description = "Get internal product code for the game from the ROM header, if available.";
                return o;
            });

            group.MapGet("/getgametitle", async (SocketService socket) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.getGameTitle"));
            }).WithOpenApi(o =>
            {
                o.Summary = "Get internal title of the game.";
                o.Description = "Get internal title of the game from the ROM header.";
                return o;
            });

            group.MapGet("/getkey", async (SocketService socket, KeysEnum key) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.getKey", key.ToString()));
            }).WithOpenApi(o =>
            {
                o.Summary = "Get the active state of a given key.";
                o.Description = "Get the active state of a given key.";
                o.Parameters[0].Description = "Key value of: A, B, Start, Select, Start, Right, Left, Up, Down, R, or L.";
                return o;
            });

            group.MapGet("/getkeys", async (SocketService socket) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.getKeys"));
            }).WithOpenApi(o =>
            {
                o.Summary = "Get the currently active keys as a bitmask.";
                o.Description = "Get the currently active keys as a bitmask.";
                return o;
            });

            group.MapPost("/loadfile", async (SocketService socket, string path) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.loadFile", path));
            }).WithOpenApi(o =>
            {
                o.Summary = "Load a ROM file.";
                o.Description = "Load a ROM file into the current state of this core.";
                return o;
            });

            group.MapPost("/loadsavefile", async (SocketService socket, string path, bool temporary) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.loadSaveFile", path, temporary.ToString()));
            }).WithOpenApi(o =>
            {
                o.Summary = "Load save data file.";
                o.Description = "Load save data from the given path. If the temporary flag is set, the given save data will not be written back to disk.";
                return o;
            });

            group.MapPost("/loadstatebuffer", async (SocketService socket, string buffer, int flags = 29) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.loadStateBuffer", buffer, flags.ToString()));
            }).WithOpenApi(o =>
            {
                o.Summary = "Load state from a buffer.";
                o.Description = "Load state from a buffer.";
                return o;
            });

            group.MapPost("/loadstatefile", async (SocketService socket, string path, int flags = 29) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.loadStateFile", path, flags.ToString()));
            }).WithOpenApi(o =>
            {
                o.Summary = "Load state from the given path.";
                o.Description = "Load state from the given path.";
                return o;
            });

            group.MapPost("/loadstateslot", async (SocketService socket, string slot, int flags = 29) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.loadStateSlot", slot, flags.ToString()));
            }).WithOpenApi(o =>
            {
                o.Summary = "Load state from the slot number.";
                o.Description = "Load state from the slot number.";
                return o;
            });

            group.MapGet("/platform", async (SocketService socket) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.platform"));
            }).WithOpenApi(o =>
            {
                o.Summary = "Get which platform is being emulated.";
                o.Description = "Get which platform is being emulated.";
                return o;
            });

            group.MapGet("/read16", async (SocketService socket, string address) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.read16", address));
            }).WithOpenApi(o =>
            {
                o.Summary = "Read a 16-bit value from the given bus address.";
                o.Description = "Read a 16-bit value from the given bus address.";
                o.Parameters[0].Description = "Address in hex, e.g. 0x03000000";
                return o;
            });

            group.MapGet("/read32", async (SocketService socket, string address) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.read32", address));
            }).WithOpenApi(o =>
            {
                o.Summary = "Read a 32-bit value from the given bus address.";
                o.Description = "Read a 32-bit value from the given bus address.";
                o.Parameters[0].Description = "Address in hex, e.g. 0x03000000";
                return o;
            });

            group.MapGet("/read8", async (SocketService socket, string address) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.read8", address));
            }).WithOpenApi(o =>
            {
                o.Summary = "Read an 8-bit value from the given bus address.";
                o.Description = "Read an 8-bit value from the given bus address.";
                o.Parameters[0].Description = "Address in hex, e.g. 0x03000000";
                return o;
            });

            // TODO: come back to these input datatypes for all the u32 and s32 types in the mGBA api documentation
            group.MapGet("/readrange", async (SocketService socket, string address, string length) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.readRange", address, length));
            }).WithOpenApi(o =>
            {
                o.Summary = "Read byte range from the given offset.";
                o.Description = "Read byte range from the given offset.";
                o.Parameters[0].Description = "Address in hex, e.g. 0x03000000";
                return o;
            });

            group.MapGet("/readregister", async (SocketService socket, string regName) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.readRegister", regName));
            }).WithOpenApi(o =>
            {
                o.Summary = "Read the value of the register with the given name.";
                o.Description = "Read the value of the register with the given name.";
                return o;
            });

            group.MapGet("/romsize", async (SocketService socket) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.romSize"));
            }).WithOpenApi(o =>
            {
                o.Summary = "Get the size of the loaded ROM.";
                o.Description = "Get the size of the loaded ROM.";
                return o;
            });

            group.MapPost("/runFrame", async (SocketService socket) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.runFrame"));
            }).WithOpenApi(o =>
            {
                o.Summary = "Run until the next frame.";
                o.Description = "Run until the next frame.";
                return o;
            });

            group.MapPost("/savestatebuffer", async (SocketService socket, int flags = 31) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.saveStateBuffer", flags.ToString()));
            }).WithOpenApi(o =>
            {
                o.Summary = "Save state and return as a buffer.";
                o.Description = "Save state and return as a buffer.";
                return o;
            });

            group.MapPost("/savestatefile", async (SocketService socket, string path, int flags = 31) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.saveStateFile", path, flags.ToString()));
            }).WithOpenApi(o =>
            {
                o.Summary = "Save state to the given path.";
                o.Description = "Save state to the given path.";
                return o;
            });

            group.MapPost("/savestateslot", async (SocketService socket, string slot, int flags = 31) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.saveStateSlot", slot, flags.ToString()));
            }).WithOpenApi(o =>
            {
                o.Summary = "Save state to the slot number.";
                o.Description = "Save state to the slot number.";
                return o;
            });

            group.MapPost("/screenshot", async (SocketService socket, string path) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.screenshot", path));
            }).WithOpenApi(o =>
            {
                o.Summary = "Save a screenshot.";
                o.Description = "Save a screenshot.";
                o.Parameters[0].Description = @"For example, C:\screenshots\screenshot.png";
                return o;
            });

            group.MapPost("/setkeys", async (SocketService socket, int keys) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.setKeys", keys.ToString()));
            }).WithOpenApi(o =>
            {
                o.Summary = "Set the currently active key list.";
                o.Description = "Set the currently active key list.";
                return o;
            });

            group.MapPost("/step", async (SocketService socket) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.step"));
            }).WithOpenApi(o =>
            {
                o.Summary = "Run a single instruction.";
                o.Description = "Run a single instruction.";
                return o;
            });

            group.MapPost("/write16", async (SocketService socket, string address, int value) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.write16", address, value.ToString()));
            }).WithOpenApi(o =>
            {
                o.Summary = "Write a 16-bit value from the given bus address.";
                o.Description = "Write a 16-bit value from the given bus address.";
                o.Parameters[0].Description = "Address in hex, e.g. 0x03000000";
                return o;
            });

            group.MapPost("/write32", async (SocketService socket, string address, int value) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.write32", address, value.ToString()));
            }).WithOpenApi(o =>
            {
                o.Summary = "Write a 32-bit value from the given bus address.";
                o.Description = "Write a 32-bit value from the given bus address.";
                o.Parameters[0].Description = "Address in hex, e.g. 0x03000000";
                return o;
            });

            group.MapPost("/write8", async (SocketService socket, string address, int value) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.write8", address, value.ToString()));
            }).WithOpenApi(o =>
            {
                o.Summary = "Write an 8-bit value from the given bus address.";
                o.Description = "Write an 8-bit value from the given bus address.";
                o.Parameters[0].Description = "Address in hex, e.g. 0x03000000";
                return o;
            });

            group.MapPost("/writeregister", async (SocketService socket, string regName, int value) =>
            {
                return await socket.SendMessageAsync(new MessageModel("core.writeRegister", regName, value.ToString()));
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
