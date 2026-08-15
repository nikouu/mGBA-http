using Microsoft.AspNetCore.Diagnostics;
using System.Net.Sockets;

namespace mGBAHttp
{
    internal sealed class MgbaExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
        {
            var (status, message) = exception switch
            {
                MgbaException => (StatusCodes.Status502BadGateway, exception.Message),
                TimeoutException => (StatusCodes.Status504GatewayTimeout, exception.Message),
                SocketException => (StatusCodes.Status502BadGateway, "Could not communicate with mGBA. Is mGBA running with mGBASocketServer.lua loaded?"),
                BadHttpRequestException badRequest => (badRequest.StatusCode, badRequest.Message),
                _ => (StatusCodes.Status500InternalServerError, exception.Message)
            };

            context.Response.StatusCode = status;
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync(message, cancellationToken);
            return true;
        }
    }
}
