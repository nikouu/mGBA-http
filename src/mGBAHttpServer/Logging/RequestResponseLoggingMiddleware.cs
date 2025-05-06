using Microsoft.IO;
using System.Diagnostics;

namespace mGBAHttpServer.Logging;

public class RequestResponseLoggingMiddleware
{
    private const string CorrelationIdHeaderName = "X-Correlation-ID";
    private readonly RequestDelegate _next;
    private readonly RecyclableMemoryStreamManager _streamManager;

    public RequestResponseLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
        _streamManager = new RecyclableMemoryStreamManager();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        bool isSwaggerRequest = context.Request.Path.StartsWithSegments("/swagger", StringComparison.OrdinalIgnoreCase);

        if (isSwaggerRequest)
        {
            await _next(context);
            return;
        }

        // Check if endpoint exists
        var endpoint = context.GetEndpoint();
        if (endpoint == null)
        {
            // No endpoint found - let other middleware handle it
            await _next(context);
            return;
        }

        // Get or create correlation ID
        var correlationId = context.Request.Headers[CorrelationIdHeaderName].FirstOrDefault()
            ?? Activity.Current?.Id
            ?? Guid.NewGuid().ToString();

        // Add correlation ID to response headers
        context.Response.Headers[CorrelationIdHeaderName] = correlationId;

        var logger = context.RequestServices.GetRequiredService<ILogger<RequestResponseLoggingMiddleware>>();

        // Create logging scope with correlation ID
        using var scope = logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId
        });

        // Log incoming request
        logger.LogInformation("Incoming {Method} request to: {Path}",
            context.Request.Method,
            context.Request.Path + context.Request.QueryString);

        // Capture the response using recyclable stream
        var originalBodyStream = context.Response.Body;
        using var memoryStream = _streamManager.GetStream();
        context.Response.Body = memoryStream;

        try
        {
            await _next(context);

            // Log outgoing response
            memoryStream.Position = 0;
            var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();
            memoryStream.Position = 0;
            await memoryStream.CopyToAsync(originalBodyStream);

            logger.LogInformation("Outgoing response from {Path}: {Response}",
                context.Request.Path,
                responseBody.Length > 500 ? responseBody[..500] + "..." : responseBody);
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }
}

public static class RequestResponseLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestResponseLoggingMiddleware>();
    }
}

