using mGBAHttp.Domain;
using mGBAHttp.Models;
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

            group.MapPost("/add", async (ObjectPool<ReusableSocket> socketPool, ButtonEnum button) =>
            {
                var messageModel = new MessageModel("mgba-http.button.add", button.ToString()).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Adds a single button.";
                o.Description = "A custom convenience API that mimics /core/addkey but uses button names as opposed to their number value.";
                o.Parameters[0].Description = "Key value of: A, B, Start, Select, Start, Right, Left, Up, Down, R, or L.";
                o.Responses["200"].Description = "Empty success response.";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("");
                return o;
            });

            group.MapPost("/clear", async (ObjectPool<ReusableSocket> socketPool, ButtonEnum button) =>
            {
                var messageModel = new MessageModel("mgba-http.button.clear", button.ToString()).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Remove a single button.";
                o.Description = "A custom convenience API that mimics /core/clearkey but uses button names as opposed to their number value.";
                o.Parameters[0].Description = "Key value of: A, B, Start, Select, Start, Right, Left, Up, Down, R, or L.";
                o.Responses["200"].Description = "Empty success response/";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("");
                return o;
            });

            group.MapGet("/get", async (ObjectPool<ReusableSocket> socketPool, ButtonEnum button) =>
            {
                var messageModel = new MessageModel("mgba-http.button.get", button.ToString()).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Get the active state of a given button.";
                o.Description = "A custom convenience API that mimics /core/getkey but uses button names as opposed to their number value.";
                o.Parameters[0].Description = "Button value of: A, B, Start, Select, Start, Right, Left, Up, Down, R, or L.";
                o.Responses["200"].Description = "0 if key is not pressed or 1 if the key is pressed.";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("1");
                return o;
            });

            group.MapPost("/tap", async (ObjectPool<ReusableSocket> socketPool, ButtonEnum button) =>
            {
                var messageModel = new MessageModel("mgba-http.button.tap", button.ToString()).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Sends button presses.";
                o.Description = "A custom convenience API that implements a key press and release. This is as opposed to the key based core API that sends only either a press or release message.";
                o.Parameters[0].Description = "Key value of: A, B, Start, Select, Start, Right, Left, Up, Down, R, or L.";
                o.Responses["200"].Description = "Empty success response";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("");
                return o;
            });

            group.MapPost("/tapmany", async (ObjectPool<ReusableSocket> socketPool, [FromQuery] ButtonEnum[] keys) =>
            {
                var messageModel = new MessageModel("mgba-http.button.tapmany", string.Join(";", keys)).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Sends multiple button presses simultaneously.";
                o.Description = "A custom convenience API that implements multiple simultaneously keys being pressed and released. This is as opposed to the key based core API that sends only either a press or release message.";
                o.Parameters[0].Description = "A key array.";
                o.Responses["200"].Description = "Empty success response";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("");
                return o;
            });

            group.MapPost("/hold", async (ObjectPool<ReusableSocket> socketPool, ButtonEnum button, int duration) =>
            {
                var messageModel = new MessageModel("mgba-http.button.hold", button.ToString(), duration.ToString()).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Sends a held down button for a given duration in frames.";
                o.Description = "A custom convenience API that implements a held down button for a given duration in frames. This is as opposed to the key based core API that sends only either a press or release message.";
                o.Parameters[0].Description = "Key value of: A, B, Start, Select, Start, Right, Left, Up, Down, R, or L.";
                o.Parameters[1].Description = "Duration in frames.";
                o.Responses["200"].Description = "Empty success response";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("");
                return o;
            });

            group.MapPost("/holdmany", async (ObjectPool<ReusableSocket> socketPool, [FromQuery] ButtonEnum[] keys, int duration) =>
            {
                var messageModel = new MessageModel("mgba-http.button.holdmany", string.Join(";", keys), duration.ToString()).ToString();
                return await PooledSocketHelper.SendMessageAsync(socketPool, messageModel);
            }).WithOpenApi(o =>
            {
                o.Summary = "Sends multiple button presses simultaneously for a given duration in frames.";
                o.Description = "A custom convenience API that implements multiple simultaneously keys being pressed and released for a given duration in frames. This is as opposed to the key based core API that sends only either a press or release message.";
                o.Parameters[0].Description = "A key array.";
                o.Parameters[1].Description = "Duration in frames.";
                o.Responses["200"].Description = "Empty success response";
                o.Responses["200"].Content["text/plain"].Example = new Microsoft.OpenApi.Any.OpenApiString("");
                return o;
            });

            return group;
        }
    }
}
