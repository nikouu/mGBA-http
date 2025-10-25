using mGBAHttp.Domain;
using mGBAHttp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ObjectPool;
using Microsoft.OpenApi.Models;
using System.Text.RegularExpressions;

namespace mGBAHttp.Endpoints
{
    public static class CoreApi
    {
        public static RouteGroupBuilder MapCoreEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/core");
            group.WithTags("Core");

            group.MapPost("/addkey", async (ObjectPool<ReusableSocket> socketPool, int key) =>
            {
                var messageModel = new MessageModel("core.addKey", key.ToString()).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Add a single key.";
                o.Description = "Add a single key to the currently active key list. As if the key were held down.";
                o.Parameters[0].Description = "Key value of: 0-9";
                o.Responses["200"].Description = "Empty success response";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("");
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
                o.Responses["200"].Description = "Empty success response.";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("");
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
                o.Responses["200"].Description = "Boolean as a string.";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("true");
                return o;
            });

            group.MapGet("/checksum", async (ObjectPool<ReusableSocket> socketPool) =>
            {
                var messageModel = new MessageModel("core.checksum").ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Get the checksum of the loaded ROM.";
                o.Description = "Get the checksum of the loaded ROM.";
                o.Responses["200"].Description = "Checksum as a string.";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("123456789");
                return o;
            });

            group.MapPost("/clearkey", async (ObjectPool<ReusableSocket> socketPool, int key) =>
            {
                var messageModel = new MessageModel("core.clearKey", key.ToString()).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Remove a single key.";
                o.Description = "Remove a single key from the currently active key list. As if the key were released.";
                o.Parameters[0].Description = "Key value of: 0-9";
                o.Responses["200"].Description = "Empty success response.";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("");
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
                o.Responses["200"].Description = "Empty success response.";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("");
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
                o.Responses["200"].Description = "Current frame number as a string.";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("12345");
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
                o.Responses["200"].Description = "Cycles per frame as a string.";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("280896");
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
                o.Responses["200"].Description = "Cycles per second as a string.";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("16777216");
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
                o.Responses["200"].Description = "Product code string.";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("AGB-BPRE");
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
                o.Responses["200"].Description = "Game title string.";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("POKEMON FIRE");
                return o;
            });

            group.MapGet("/getkey", async (ObjectPool<ReusableSocket> socketPool, int key) =>
            {
                var messageModel = new MessageModel("core.getKey", key.ToString()).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Get the active state of a given key.";
                o.Description = "Get the active state of a given key.";
                o.Parameters[0].Description = "Key value of: 0-9";
                o.Responses["200"].Description = "0 if key is not pressed or 1 if the key is pressed.";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("1");
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
                o.Responses["200"].Description = "Bitmask string representing currently active keys.";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("12");
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
                o.Responses["200"].Description = "Boolean as a string.";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("true");
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
                o.Responses["200"].Description = "Boolean as a string.";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("true");
                return o;
            });

            group.MapPost("/loadstatebuffer", async (ObjectPool<ReusableSocket> socketPool, HttpContext context, int flags = 29) =>
            {
                using var reader = new StreamReader(context.Request.Body);
                var buffer = await reader.ReadToEndAsync();
                var cleanedBuffer = Regex.Replace(buffer, @"\s", string.Empty);

                var messageModel = new MessageModel("core.loadStateBuffer", cleanedBuffer, flags.ToString()).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Load state from a buffer.";
                o.Description = "Load state from a hex array string buffer in the format: [d3,00,ea,66,...].";
                o.Parameters[0].Description = "State flags bitmask. Default 29 excludes screenshot flag (all except SCREENSHOT). Flags: SCREENSHOT=1, SAVEDATA=2, CHEATS=4, RTC=8, METADATA=16.";
                o.Responses["200"].Description = "Boolean as a string.";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("true");
                o.RequestBody = new OpenApiRequestBody
                {
                    Description = "State buffer data as hex array string format: [d3,00,ea,66,...]",
                    Required = true,
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["text/plain"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "string",
                                Example = new Microsoft.OpenApi.Any.OpenApiString("[d3,00,00,ea,66,00,00,ea,0c,00,00,ea,fe,ff,ff,ea]")
                            }
                        }
                    }
                };

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
                o.Responses["200"].Description = "Boolean as a string.";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("true");
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
                o.Responses["200"].Description = "Boolean as a string.";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("true");
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
                o.Responses["200"].Description = "Platform identifier. None: -1, GBA: 0, GB: 1.";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("1");
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
                o.Parameters[0].Description = "Address as hex string with 0x prefix (e.g., '0x0300').";
                o.Responses["200"].Description = "16-bit value as string.";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("65535");
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
                o.Parameters[0].Description = "Address as hex string with 0x prefix (e.g., '0x0300').";
                o.Responses["200"].Description = "32-bit value as string.";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("4294967295");
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
                o.Parameters[0].Description = "Address as hex string with 0x prefix (e.g., '0x0300').";
                o.Responses["200"].Description = "8-bit value as string.";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("255");
                return o;
            });

            // TODO: come back to these input datatypes for all the u32 and s32 types in the mGBA api documentation
            group.MapGet("/readrange", async (ObjectPool<ReusableSocket> socketPool, string address, string length) =>
            {
                var messageModel = new MessageModel("core.readRange", address, length).ToString();
                var result = await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
                return result;
            }).WithOpenApi(o =>
            {
                o.Summary = "Read byte range from the given offset.";
                o.Description = "Read byte range from the given offset.";
                o.Parameters[0].Description = "Address as hex string with 0x prefix (e.g., '0x0300').";
                o.Parameters[1].Description = "Number of bytes to read.";
                o.Responses["200"].Description = "Comma separated hex values.";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("d3,00,00,ea,66,00,00,ea");
                return o;
            });

            group.MapGet("/readregister", async (ObjectPool<ReusableSocket> socketPool, string regName) =>
            {
                var messageModel = new MessageModel("core.readRegister", regName).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Read the value of the register with the given name.";
                o.Description = "Read the value of the register with the given name.";
                o.Responses["200"].Description = "Register value as string";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("50363837");
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
                o.Responses["200"].Description = "ROM size in bytes.";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("16777216");
                return o;
            });

            group.MapPost("/savestatebuffer", async (ObjectPool<ReusableSocket> socketPool, int flags = 31) =>
            {
                var messageModel = new MessageModel("core.saveStateBuffer", flags.ToString()).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Save state and return as a buffer.";
                o.Description = "Save state and return as a buffer. This can be used with /core/loadstatebuffer to restore state.";
                o.Parameters[0].Description = "State flags bitmask. Default 31 includes all flags (screenshot, metadata, etc). Flags: SCREENSHOT=1, SAVEDATA=2, CHEATS=4, RTC=8, METADATA=16.";
                o.Responses["200"].Description = "Comma separated hex values.";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("d3,00,00,ea,66,00,00,ea,0c,00,00,ea,fe,ff,ff,ea");
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
                o.Responses["200"].Description = "Boolean as a string.";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("true");
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
                o.Responses["200"].Description = "Boolean as a string.";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("true");
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
                o.Responses["200"].Description = "Empty success response.";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("");
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
                o.Responses["200"].Description = "Empty success response.";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("");
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
                o.Responses["200"].Description = "Empty success response.";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("");
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
                o.Parameters[0].Description = "Address in hex, e.g. 0x0300";
                o.Responses["200"].Description = "Empty success response.";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("");
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
                o.Parameters[0].Description = "Address in hex, e.g. 0x0300";
                o.Responses["200"].Description = "Empty success response.";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("");
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
                o.Parameters[0].Description = "Address in hex, e.g. 0x0300";
                o.Responses["200"].Description = "Empty success response.";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("");
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
                o.Parameters[0].Description = "Address in hex, e.g. 0x0300";
                o.Responses["200"].Description = "Empty success response.";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("");
                return o;
            });

            return group;
        }
    }
}
