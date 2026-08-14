using mGBAHttp.Domain;
using mGBAHttp.Models;
using mGBAHttp.OpenApi;
using Microsoft.Extensions.ObjectPool;
using System.Text.RegularExpressions;

namespace mGBAHttp.Endpoints
{
    public static class CoreApi
    {
        public static RouteGroupBuilder MapCoreEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/core");
            group.WithTags("Core");

            group.MapPost("/addkey", AddKey);
            group.MapPost("/addkeys", AddKeys);
            group.MapPost("/autoloadsave", AutoloadSave).WithMetadata(new ResponseExample("true"));
            group.MapGet("/checksum", Checksum).WithMetadata(new ResponseExample("123456789"));
            group.MapPost("/clearkey", ClearKey);
            group.MapPost("/clearkeys", ClearKeys);
            group.MapGet("/currentFrame", CurrentFrame).WithMetadata(new ResponseExample("12345"));
            group.MapGet("/framecycles", FrameCycles).WithMetadata(new ResponseExample("280896"));
            group.MapGet("/frequency", Frequency).WithMetadata(new ResponseExample("16777216"));
            group.MapGet("/getgamecode", GetGameCode).WithMetadata(new ResponseExample("AGB-BPRE"));
            group.MapGet("/getgametitle", GetGameTitle).WithMetadata(new ResponseExample("POKEMON FIRE"));
            group.MapGet("/getkey", GetKey).WithMetadata(new ResponseExample("1"));
            group.MapGet("/getkeys", GetKeys).WithMetadata(new ResponseExample("12"));
            group.MapPost("/loadfile", LoadFile).WithMetadata(new ResponseExample("true"));
            group.MapPost("/loadsavefile", LoadSaveFile).WithMetadata(new ResponseExample("true"));
            group.MapPost("/loadstatebuffer", LoadStateBuffer).Accepts<string>("text/plain").WithMetadata(new ResponseExample("true"));
            group.MapPost("/loadstatefile", LoadStateFile).WithMetadata(new ResponseExample("true"));
            group.MapPost("/loadstateslot", LoadStateSlot).WithMetadata(new ResponseExample("true"));
            group.MapGet("/platform", Platform).WithMetadata(new ResponseExample("1"));
            group.MapGet("/read16", Read16).WithMetadata(new ResponseExample("65535"));
            group.MapGet("/read32", Read32).WithMetadata(new ResponseExample("4294967295"));
            group.MapGet("/read8", Read8).WithMetadata(new ResponseExample("255"));
            group.MapGet("/readrange", ReadRange).WithMetadata(new ResponseExample("d3,00,00,ea,66,00,00,ea"));
            group.MapGet("/readregister", ReadRegister).WithMetadata(new ResponseExample("50363837"));
            group.MapGet("/romsize", RomSize).WithMetadata(new ResponseExample("16777216"));
            group.MapPost("/savestatebuffer", SaveStateBuffer).WithMetadata(new ResponseExample("d3,00,00,ea,66,00,00,ea,0c,00,00,ea,fe,ff,ff,ea"));
            group.MapPost("/savestatefile", SaveStateFile).WithMetadata(new ResponseExample("true"));
            group.MapPost("/savestateslot", SaveStateSlot).WithMetadata(new ResponseExample("true"));
            group.MapPost("/screenshot", Screenshot);
            group.MapPost("/setkeys", SetKeys);
            group.MapPost("/step", Step);
            group.MapPost("/write16", Write16);
            group.MapPost("/write32", Write32);
            group.MapPost("/write8", Write8);
            group.MapPost("/writeregister", WriteRegister);

            return group;
        }

        /// <summary>Add a single key.</summary>
        /// <remarks>Add a single key to the currently active key list. As if the key were held down.</remarks>
        /// <param name="key">Key value of: 0-9</param>
        /// <response code="200">Empty success response</response>
        public static async Task<string> AddKey(ObjectPool<ReusableSocket> socketPool, int key)
        {
            var messageModel = new MessageModel("core.addKey", key.ToString()).ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Add a bitmask of keys.</summary>
        /// <remarks>Add a bitmask of keys to the currently active key list. As if the keys were held down.</remarks>
        /// <param name="keyBitmask">The bitmask of keys to add. Bitmasking has the keys from the mGBA scripting documentation where each key has a value that goes up in that order in binary: A = 1, B = 2, Select = 4, Start = 8, etc. A bitmask of 12 is Start and Select.</param>
        /// <response code="200">Empty success response.</response>
        public static async Task<string> AddKeys(ObjectPool<ReusableSocket> socketPool, int keyBitmask)
        {
            var messageModel = new MessageModel("core.addKeys", keyBitmask.ToString()).ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Load save data.</summary>
        /// <remarks>Load the save data associated with the currently loaded ROM file.</remarks>
        /// <response code="200">Boolean representing success.</response>
        public static async Task<string> AutoloadSave(ObjectPool<ReusableSocket> socketPool)
        {
            var messageModel = new MessageModel("core.autoloadSave").ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Get the checksum of the loaded ROM.</summary>
        /// <remarks>Get the checksum of the loaded ROM.</remarks>
        /// <response code="200">The checksum.</response>
        public static async Task<string> Checksum(ObjectPool<ReusableSocket> socketPool)
        {
            var messageModel = new MessageModel("core.checksum").ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Remove a single key.</summary>
        /// <remarks>Remove a single key from the currently active key list. As if the key were released.</remarks>
        /// <param name="key">Key value of: 0-9</param>
        /// <response code="200">Empty success response.</response>
        public static async Task<string> ClearKey(ObjectPool<ReusableSocket> socketPool, int key)
        {
            var messageModel = new MessageModel("core.clearKey", key.ToString()).ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Remove a bitmask of keys.</summary>
        /// <remarks>Remove a bitmask of keys from the currently active key list. As if the keys were released.</remarks>
        /// <param name="keyBitmask">The bitmask of keys to clear. Bitmasking has the keys from the mGBA scripting documentation where each key has a value that goes up in that order in binary: A = 1, B = 2, Select = 4, Start = 8, etc. A bitmask of 12 is Start and Select.</param>
        /// <response code="200">Empty success response.</response>
        public static async Task<string> ClearKeys(ObjectPool<ReusableSocket> socketPool, int keyBitmask)
        {
            var messageModel = new MessageModel("core.clearKeys", keyBitmask.ToString()).ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Get the number of the current frame.</summary>
        /// <remarks>Get the number of the current frame.</remarks>
        /// <response code="200">Current frame number.</response>
        public static async Task<string> CurrentFrame(ObjectPool<ReusableSocket> socketPool)
        {
            var messageModel = new MessageModel("core.currentFrame").ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Get the number of cycles per frame.</summary>
        /// <remarks>Get the number of cycles per frame.</remarks>
        /// <response code="200">Cycles per frame.</response>
        public static async Task<string> FrameCycles(ObjectPool<ReusableSocket> socketPool)
        {
            var messageModel = new MessageModel("core.frameCycles").ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Get the number of cycles per second.</summary>
        /// <remarks>Get the number of cycles per second.</remarks>
        /// <response code="200">Cycles per second.</response>
        public static async Task<string> Frequency(ObjectPool<ReusableSocket> socketPool)
        {
            var messageModel = new MessageModel("core.frequency").ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Get internal product code for the game.</summary>
        /// <remarks>Get internal product code for the game from the ROM header, if available.</remarks>
        /// <response code="200">Product code.</response>
        public static async Task<string> GetGameCode(ObjectPool<ReusableSocket> socketPool)
        {
            var messageModel = new MessageModel("core.getGameCode").ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Get internal title of the game.</summary>
        /// <remarks>Get internal title of the game from the ROM header.</remarks>
        /// <response code="200">Game title.</response>
        public static async Task<string> GetGameTitle(ObjectPool<ReusableSocket> socketPool)
        {
            var messageModel = new MessageModel("core.getGameTitle").ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Get the active state of a given key.</summary>
        /// <remarks>Get the active state of a given key.</remarks>
        /// <param name="key">Key value of: 0-9</param>
        /// <response code="200">0 if key is not pressed or 1 if the key is pressed.</response>
        public static async Task<string> GetKey(ObjectPool<ReusableSocket> socketPool, int key)
        {
            var messageModel = new MessageModel("core.getKey", key.ToString()).ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Get the currently active keys as a bitmask.</summary>
        /// <remarks>Get the currently active keys as a bitmask.</remarks>
        /// <response code="200">Bitmask string representing currently active keys.</response>
        public static async Task<string> GetKeys(ObjectPool<ReusableSocket> socketPool)
        {
            var messageModel = new MessageModel("core.getKeys").ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Load a ROM file.</summary>
        /// <remarks>Load a ROM file into the current state of this core. **Note: Prefer using the /mgba-http/extension/loadfile endpoint.**</remarks>
        /// <param name="path">Path to ROM file to load.</param>
        /// <response code="200">Success status as a boolean.</response>
        public static async Task<string> LoadFile(ObjectPool<ReusableSocket> socketPool, string path)
        {
            var messageModel = new MessageModel("core.loadFile", path).ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Load save data file.</summary>
        /// <remarks>Load save data from the given path. If the temporary flag is set, the given save data will not be written back to disk.</remarks>
        /// <param name="path">Path to save file to load.</param>
        /// <param name="temporary">If true, save data will not be written back to disk.</param>
        /// <response code="200">Success status as a boolean.</response>
        public static async Task<string> LoadSaveFile(ObjectPool<ReusableSocket> socketPool, string path, bool temporary)
        {
            var messageModel = new MessageModel("core.loadSaveFile", path, temporary.ToString()).ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Load state from a buffer.</summary>
        /// <remarks>
        /// Load state from a hex array string buffer in the format: [d3,00,ea,66,...]. Send the buffer as the
        /// text/plain request body, for example [d3,00,00,ea,66,00,00,ea,0c,00,00,ea,fe,ff,ff,ea].
        /// </remarks>
        /// <param name="flags">State flags bitmask. Default 29 excludes screenshot flag (all except SCREENSHOT). Flags: SCREENSHOT=1, SAVEDATA=2, CHEATS=4, RTC=8, METADATA=16.</param>
        /// <response code="200">Success status as a boolean.</response>
        public static async Task<string> LoadStateBuffer(ObjectPool<ReusableSocket> socketPool, HttpContext context, int? flags)
        {
            using var reader = new StreamReader(context.Request.Body);
            var buffer = await reader.ReadToEndAsync();
            var cleanedBuffer = Regex.Replace(buffer, @"\s", string.Empty);

            var messageModel = new MessageModel("core.loadStateBuffer", cleanedBuffer, (flags ?? 29).ToString()).ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Load state from the given path.</summary>
        /// <remarks>Load state from the given path.</remarks>
        /// <param name="path">Path to state file to load.</param>
        /// <param name="flags">State flags bitmask. Default 29 excludes screenshot flag. Flags: SCREENSHOT=1, SAVEDATA=2, CHEATS=4, RTC=8, METADATA=16.</param>
        /// <response code="200">Success status as a boolean.</response>
        public static async Task<string> LoadStateFile(ObjectPool<ReusableSocket> socketPool, string path, int? flags)
        {
            var messageModel = new MessageModel("core.loadStateFile", path, (flags ?? 29).ToString()).ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Load state from the slot number.</summary>
        /// <remarks>Load state from the slot number.</remarks>
        /// <param name="slot">State slot number to load from.</param>
        /// <param name="flags">State flags bitmask. Default 29 excludes screenshot flag. Flags: SCREENSHOT=1, SAVEDATA=2, CHEATS=4, RTC=8, METADATA=16.</param>
        /// <response code="200">Success status as a boolean.</response>
        public static async Task<string> LoadStateSlot(ObjectPool<ReusableSocket> socketPool, string slot, int? flags)
        {
            var messageModel = new MessageModel("core.loadStateSlot", slot, (flags ?? 29).ToString()).ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Get which platform is being emulated.</summary>
        /// <remarks>Get which platform is being emulated.</remarks>
        /// <response code="200">Platform identifier. None: -1, GBA: 0, GB: 1.</response>
        public static async Task<string> Platform(ObjectPool<ReusableSocket> socketPool)
        {
            var messageModel = new MessageModel("core.platform").ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Read a 16-bit value from the given bus address.</summary>
        /// <remarks>Read a 16-bit value from the given bus address.</remarks>
        /// <param name="address">Address as hex string with 0x prefix (e.g., '0x0300').</param>
        /// <response code="200">16-bit value of the read memory.</response>
        public static async Task<string> Read16(ObjectPool<ReusableSocket> socketPool, string address)
        {
            var messageModel = new MessageModel("core.read16", address).ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Read a 32-bit value from the given bus address.</summary>
        /// <remarks>Read a 32-bit value from the given bus address.</remarks>
        /// <param name="address">Address as hex string with 0x prefix (e.g., '0x0300').</param>
        /// <response code="200">32-bit value of the read memory.</response>
        public static async Task<string> Read32(ObjectPool<ReusableSocket> socketPool, string address)
        {
            var messageModel = new MessageModel("core.read32", address).ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Read an 8-bit value from the given bus address.</summary>
        /// <remarks>Read an 8-bit value from the given bus address.</remarks>
        /// <param name="address">Address as hex string with 0x prefix (e.g., '0x0300').</param>
        /// <response code="200">8-bit value of the read memory.</response>
        public static async Task<string> Read8(ObjectPool<ReusableSocket> socketPool, string address)
        {
            var messageModel = new MessageModel("core.read8", address).ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Read byte range from the given offset.</summary>
        /// <remarks>Read byte range from the given offset.</remarks>
        /// <param name="address">Address as hex string with 0x prefix (e.g., '0x0300').</param>
        /// <param name="length">Number of bytes to read.</param>
        /// <response code="200">Comma separated hex values of the read memory.</response>
        public static async Task<string> ReadRange(ObjectPool<ReusableSocket> socketPool, string address, string length)
        {
            var messageModel = new MessageModel("core.readRange", address, length).ToString();
            var result = await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            return result;
        }

        /// <summary>Read the value of the register with the given name.</summary>
        /// <remarks>Read the value of the register with the given name.</remarks>
        /// <param name="regName">Register name to read (e.g., 'r0', 'r1', 'sp', 'lr', 'pc').</param>
        /// <response code="200">The value of the register.</response>
        public static async Task<string> ReadRegister(ObjectPool<ReusableSocket> socketPool, string regName)
        {
            var messageModel = new MessageModel("core.readRegister", regName).ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Get the size of the loaded ROM.</summary>
        /// <remarks>Get the size of the loaded ROM.</remarks>
        /// <response code="200">The ROM size.</response>
        public static async Task<string> RomSize(ObjectPool<ReusableSocket> socketPool)
        {
            var messageModel = new MessageModel("core.romSize").ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Save state and return as a buffer.</summary>
        /// <remarks>Save state and return as a buffer. This can be used with /core/loadstatebuffer to restore state.</remarks>
        /// <param name="flags">State flags bitmask. Default 31 includes all flags (screenshot, metadata, etc). Flags: SCREENSHOT=1, SAVEDATA=2, CHEATS=4, RTC=8, METADATA=16.</param>
        /// <response code="200">Comma separated hex values.</response>
        public static async Task<string> SaveStateBuffer(ObjectPool<ReusableSocket> socketPool, int? flags)
        {
            var messageModel = new MessageModel("core.saveStateBuffer", (flags ?? 31).ToString()).ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Save state to the given path.</summary>
        /// <remarks>Save state to the given path.</remarks>
        /// <param name="path">Path where state file will be saved.</param>
        /// <param name="flags">State flags bitmask. Default 31 includes all flags. Flags: SCREENSHOT=1, SAVEDATA=2, CHEATS=4, RTC=8, METADATA=16.</param>
        /// <response code="200">Success status as a boolean.</response>
        public static async Task<string> SaveStateFile(ObjectPool<ReusableSocket> socketPool, string path, int? flags)
        {
            var messageModel = new MessageModel("core.saveStateFile", path, (flags ?? 31).ToString()).ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Save state to the slot number.</summary>
        /// <remarks>Save state to the slot number.</remarks>
        /// <param name="slot">State slot number to save to.</param>
        /// <param name="flags">State flags bitmask. Default 31 includes all flags. Flags: SCREENSHOT=1, SAVEDATA=2, CHEATS=4, RTC=8, METADATA=16.</param>
        /// <response code="200">Success status as a boolean.</response>
        public static async Task<string> SaveStateSlot(ObjectPool<ReusableSocket> socketPool, string slot, int? flags)
        {
            var messageModel = new MessageModel("core.saveStateSlot", slot, (flags ?? 31).ToString()).ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Save a screenshot.</summary>
        /// <remarks>Save a screenshot.</remarks>
        /// <param name="path">For example, C:\screenshots\screenshot.png</param>
        /// <response code="200">Empty success response.</response>
        public static async Task<string> Screenshot(ObjectPool<ReusableSocket> socketPool, string path)
        {
            var messageModel = new MessageModel("core.screenshot", path).ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Set the currently active key list.</summary>
        /// <remarks>Set the currently active key list.</remarks>
        /// <param name="keys">The bitmask of keys to set as active. Bitmasking has the keys from the mGBA scripting documentation where each key has a value that goes up in that order in binary: A = 1, B = 2, Select = 4, Start = 8, etc. A bitmask of 12 is Start and Select.</param>
        /// <response code="200">Empty success response.</response>
        public static async Task<string> SetKeys(ObjectPool<ReusableSocket> socketPool, int keys)
        {
            var messageModel = new MessageModel("core.setKeys", keys.ToString()).ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Run a single instruction.</summary>
        /// <remarks>Run a single instruction.</remarks>
        /// <response code="200">Empty success response.</response>
        public static async Task<string> Step(ObjectPool<ReusableSocket> socketPool)
        {
            var messageModel = new MessageModel("core.step").ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Write a 16-bit value from the given bus address.</summary>
        /// <remarks>Write a 16-bit value from the given bus address.</remarks>
        /// <param name="address">Address in hex, e.g. 0x0300</param>
        /// <param name="value">16-bit unsigned integer value to write (0-65535).</param>
        /// <response code="200">Empty success response.</response>
        public static async Task<string> Write16(ObjectPool<ReusableSocket> socketPool, string address, uint value)
        {
            var messageModel = new MessageModel("core.write16", address, value.ToString()).ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Write a 32-bit value from the given bus address.</summary>
        /// <remarks>Write a 32-bit value from the given bus address.</remarks>
        /// <param name="address">Address in hex, e.g. 0x0300</param>
        /// <param name="value">32-bit unsigned integer value to write.</param>
        /// <response code="200">Empty success response.</response>
        public static async Task<string> Write32(ObjectPool<ReusableSocket> socketPool, string address, uint value)
        {
            var messageModel = new MessageModel("core.write32", address, value.ToString()).ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Write an 8-bit value from the given bus address.</summary>
        /// <remarks>Write an 8-bit value from the given bus address.</remarks>
        /// <param name="address">Address in hex, e.g. 0x0300</param>
        /// <param name="value">8-bit decimal value to write (0-255).</param>
        /// <response code="200">Empty success response.</response>
        public static async Task<string> Write8(ObjectPool<ReusableSocket> socketPool, string address, int value)
        {
            var messageModel = new MessageModel("core.write8", address, value.ToString()).ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Write the value of the register with the given name.</summary>
        /// <remarks>Write the value of the register with the given name.</remarks>
        /// <param name="regName">Register name to write (e.g., 'r0', 'r1', 'sp', 'lr', 'pc', etc).</param>
        /// <param name="value">Decimal value to write to the register.</param>
        /// <response code="200">Empty success response.</response>
        public static async Task<string> WriteRegister(ObjectPool<ReusableSocket> socketPool, string regName, int value)
        {
            var messageModel = new MessageModel("core.writeRegister", regName, value.ToString()).ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }
    }
}
