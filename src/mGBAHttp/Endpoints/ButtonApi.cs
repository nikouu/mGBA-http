using mGBAHttp.Domain;
using mGBAHttp.Models;
using mGBAHttp.OpenApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ObjectPool;

namespace mGBAHttp.Endpoints
{
    public static class ButtonApi
    {
        public static RouteGroupBuilder MapButtonEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/mgba-http/button");
            group.WithTags("Button");

            group.MapPost("/add", Add);
            group.MapPost("/addmany", AddMany);
            group.MapPost("/clear", Clear);
            group.MapPost("/clearmany", ClearMany);
            group.MapGet("/get", Get).WithMetadata(new ResponseExample("1"));
            group.MapGet("/getall", GetAll).WithMetadata(new ResponseExample("A,B,Start"));
            group.MapPost("/tap", Tap);
            group.MapPost("/tapmany", TapMany);
            group.MapPost("/hold", Hold);
            group.MapPost("/holdmany", HoldMany);

            return group;
        }

        /// <summary>Adds a single button.</summary>
        /// <remarks>A custom convenience API that mimics /core/addkey but uses button names as opposed to a bitmask.</remarks>
        /// <param name="button">Key value of: A, B, Select, Start, Right, Left, Up, Down, R, or L.</param>
        /// <response code="200">Empty success response.</response>
        public static async Task<string> Add(ObjectPool<ReusableSocket> socketPool, ButtonEnum button)
        {
            var messageModel = new MessageModel("mgba-http.button.add", button.ToString()).ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Adds multiple buttons.</summary>
        /// <remarks>A custom convenience API that mimics /core/addkeys but uses button names as opposed to their number value.</remarks>
        /// <param name="buttons">Key values of: A, B, Select, Start, Right, Left, Up, Down, R, or L.</param>
        /// <response code="200">Empty success response.</response>
        public static async Task<string> AddMany(ObjectPool<ReusableSocket> socketPool, [FromQuery] ButtonEnum[] buttons)
        {
            var messageModel = new MessageModel("mgba-http.button.addMany", string.Join(";", buttons)).ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Remove a single button.</summary>
        /// <remarks>A custom convenience API that mimics /core/clearkey but uses button names as opposed to their number value.</remarks>
        /// <param name="button">Key value of: A, B, Select, Start, Right, Left, Up, Down, R, or L.</param>
        /// <response code="200">Empty success response.</response>
        public static async Task<string> Clear(ObjectPool<ReusableSocket> socketPool, ButtonEnum button)
        {
            var messageModel = new MessageModel("mgba-http.button.clear", button.ToString()).ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Remove multiple buttons.</summary>
        /// <remarks>A custom convenience API that mimics /core/clearkeys but uses button names as opposed to a bitmask.</remarks>
        /// <param name="buttons">Key values of: A, B, Select, Start, Right, Left, Up, Down, R, or L.</param>
        /// <response code="200">Empty success response.</response>
        public static async Task<string> ClearMany(ObjectPool<ReusableSocket> socketPool, [FromQuery] ButtonEnum[] buttons)
        {
            var messageModel = new MessageModel("mgba-http.button.clearMany", string.Join(";", buttons)).ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Get the active state of a given button.</summary>
        /// <remarks>A custom convenience API that mimics /core/getkey but uses button names as opposed to their number value.</remarks>
        /// <param name="button">Button value of: A, B, Select, Start, Right, Left, Up, Down, R, or L.</param>
        /// <response code="200">0 if key is not pressed or 1 if the key is pressed.</response>
        public static async Task<string> Get(ObjectPool<ReusableSocket> socketPool, ButtonEnum button)
        {
            var messageModel = new MessageModel("mgba-http.button.get", button.ToString()).ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Gets all active buttons.</summary>
        /// <remarks>A custom convenience API that gets all active buttons.</remarks>
        /// <response code="200">Comma separated string with all active keys</response>
        public static async Task<string> GetAll(ObjectPool<ReusableSocket> socketPool)
        {
            var messageModel = new MessageModel("mgba-http.button.getAll").ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Sends button presses.</summary>
        /// <remarks>A custom convenience API that implements a key press and release. This is as opposed to the key based core API that sends only either a press or release message.</remarks>
        /// <param name="button">Key value of: A, B, Select, Start, Right, Left, Up, Down, R, or L.</param>
        /// <response code="200">Empty success response</response>
        public static async Task<string> Tap(ObjectPool<ReusableSocket> socketPool, ButtonEnum button)
        {
            var messageModel = new MessageModel("mgba-http.button.tap", button.ToString()).ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Sends multiple button presses simultaneously.</summary>
        /// <remarks>A custom convenience API that implements multiple simultaneous keys being pressed and released. This is as opposed to the key based core API that sends only either a press or release message.</remarks>
        /// <param name="buttons">A key array.</param>
        /// <response code="200">Empty success response</response>
        public static async Task<string> TapMany(ObjectPool<ReusableSocket> socketPool, [FromQuery] ButtonEnum[] buttons)
        {
            var messageModel = new MessageModel("mgba-http.button.tapMany", string.Join(";", buttons)).ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Sends a held down button for a given duration in frames.</summary>
        /// <remarks>A custom convenience API that implements a held down button for a given duration in frames. This is as opposed to the key based core API that sends only either a press or release message.</remarks>
        /// <param name="button">Key value of: A, B, Select, Start, Right, Left, Up, Down, R, or L.</param>
        /// <param name="duration">Duration in frames.</param>
        /// <response code="200">Empty success response</response>
        public static async Task<string> Hold(ObjectPool<ReusableSocket> socketPool, ButtonEnum button, int duration)
        {
            var messageModel = new MessageModel("mgba-http.button.hold", button.ToString(), duration.ToString()).ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }

        /// <summary>Sends multiple button presses simultaneously for a given duration in frames.</summary>
        /// <remarks>A custom convenience API that implements multiple simultaneous keys being pressed and released for a given duration in frames. This is as opposed to the key based core API that sends only either a press or release message.</remarks>
        /// <param name="buttons">A key array.</param>
        /// <param name="duration">Duration in frames.</param>
        /// <response code="200">Empty success response</response>
        public static async Task<string> HoldMany(ObjectPool<ReusableSocket> socketPool, [FromQuery] ButtonEnum[] buttons, int duration)
        {
            var messageModel = new MessageModel("mgba-http.button.holdMany", string.Join(";", buttons), duration.ToString()).ToString();
            return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
        }
    }
}
