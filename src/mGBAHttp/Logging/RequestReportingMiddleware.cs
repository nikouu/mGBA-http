using System.Diagnostics;

namespace mGBAHttp.Logging;

public sealed class RequestReportingMiddleware
{
    private const int MaxBodyLength = 500;

    private readonly RequestDelegate _next;
    private readonly ConsoleReporter _reporter;

    public RequestReportingMiddleware(RequestDelegate next, ConsoleReporter reporter)
    {
        _next = next;
        _reporter = reporter;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path;
        bool isDocsRequest = path.StartsWithSegments("/scalar", StringComparison.OrdinalIgnoreCase)
            || path.StartsWithSegments("/openapi", StringComparison.OrdinalIgnoreCase);

        if (isDocsRequest)
        {
            await _next(context);
            return;
        }

        var correlationId = Activity.Current?.TraceId.ToString();
        _reporter.RequestIn(context.Request.Method, path + context.Request.QueryString, correlationId);

        var originalBody = context.Response.Body;
        using var buffer = new MemoryStream();
        context.Response.Body = buffer;

        var stopwatch = Stopwatch.StartNew();

        try
        {
            await _next(context);
            stopwatch.Stop();

            buffer.Position = 0;
            var body = await new StreamReader(buffer).ReadToEndAsync();
            buffer.Position = 0;
            await buffer.CopyToAsync(originalBody);

            _reporter.RequestOut(path, context.Response.StatusCode, Trim(body), stopwatch.ElapsedMilliseconds, correlationId);
        }
        catch (Exception exception)
        {
            stopwatch.Stop();
            _reporter.RequestFailed(path, exception, stopwatch.ElapsedMilliseconds, correlationId);
            throw;
        }
        finally
        {
            context.Response.Body = originalBody;
        }
    }

    private static string Trim(string body) =>
        body.Length <= MaxBodyLength ? body : $"{body[..MaxBodyLength]}... ({body.Length} chars total)";
}

public static class RequestReportingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestReporting(this IApplicationBuilder app) =>
        app.UseMiddleware<RequestReportingMiddleware>();
}
