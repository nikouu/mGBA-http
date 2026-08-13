using System.Diagnostics;

namespace mGBAHttp.Logging;

public class RequestResponseLoggingMiddleware
{
    private const string CorrelationIdHeaderName = "X-Correlation-ID";
    private readonly RequestDelegate _next;

    public RequestResponseLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Keep the API docs UI and document out of the request log.
        bool isDocsRequest = context.Request.Path.StartsWithSegments("/scalar", StringComparison.OrdinalIgnoreCase)
            || context.Request.Path.StartsWithSegments("/openapi", StringComparison.OrdinalIgnoreCase);

        if (isDocsRequest)
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
        var correlationId = (context.Request.Headers.Keys
            .FirstOrDefault(k => k.Equals(CorrelationIdHeaderName, StringComparison.OrdinalIgnoreCase))
            is string headerKey
                ? context.Request.Headers[headerKey].FirstOrDefault()
                : Activity.Current?.Id)
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

        // Response streams in ASP.NET Core are write-once forward-only, so we need to capture the response
        // in a temporary stream to read it for logging before sending it to the client

        // Save the original stream that would go to the client
        var originalBodyStream = context.Response.Body;

        // Create a temporary memory stream to capture the response
        using var memoryStream = new MemoryStream();
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

            const int maxLoggedBodyLength = 500;
            var loggedBody = responseBody.Length <= maxLoggedBodyLength
                ? responseBody
                : $"{responseBody[..maxLoggedBodyLength]}... ({responseBody.Length} chars total)";

            logger.LogInformation("Outgoing response from {Path}: {Response}",
                context.Request.Path, loggedBody);
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

