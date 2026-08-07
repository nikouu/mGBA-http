using Microsoft.AspNetCore.Diagnostics;
using System.Net.Sockets;

namespace mGBAHttp
{
    internal sealed class MgbaExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
        {
            (int status, string message)? mapped = exception switch
            {
                MgbaException => (StatusCodes.Status502BadGateway, exception.Message),
                TimeoutException => (StatusCodes.Status504GatewayTimeout, exception.Message),
                SocketException => (StatusCodes.Status502BadGateway, "Could not communicate with mGBA. Is mGBA running with mGBASocketServer.lua loaded?"),
                _ => null
            };

            if (mapped is not { } response)
            {
                return false; // not ours, let the default handler deal with it
            }

            context.Response.StatusCode = response.status;
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync(response.message, cancellationToken);
            return true;
        }
    }
}
