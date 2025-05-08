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
        var correlationId = context.Request.Headers.Keys
            .FirstOrDefault(k => k.Equals(CorrelationIdHeaderName, StringComparison.OrdinalIgnoreCase))
            is string headerKey
                ? context.Request.Headers[headerKey].FirstOrDefault()
                : Activity.Current?.Id ?? Guid.NewGuid().ToString();

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

        // Response streams in ASP.NET Core are write-once forward-only, so we need to capture the response
        // in a temporary stream to read it for logging before sending it to the client

        // Save the original stream that would go to the client
        var originalBodyStream = context.Response.Body;

        // Create a temporary memory stream to capture the response
        using var memoryStream = _streamManager.GetStream();
        context.Response.Body = memoryStream;

        try
        {
            await _next(context); // Response gets written to our memory stream

            // Read the response for logging (requires rewinding the stream)
            memoryStream.Position = 0;
            var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();

            // Reset position and copy to original stream for client
            memoryStream.Position = 0;
            await memoryStream.CopyToAsync(originalBodyStream);

            logger.LogInformation("Outgoing response from {Path}: {Response}",
                context.Request.Path, responseBody);
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

