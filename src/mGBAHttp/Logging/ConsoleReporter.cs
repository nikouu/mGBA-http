using Microsoft.Extensions.Options;

namespace mGBAHttp.Logging;

public sealed class ConsoleOptions
{
    public const string Section = "mgba-http:Console";
    public bool Detailed { get; set; }
}

public sealed class ConsoleReporter
{
    private const string Reset = "\x1B[0m";
    private const string Green = "\x1B[1m\x1B[32m";
    private const string Red = "\x1B[1m\x1B[31m";
    private const string Purple = "\x1B[1m\x1B[35m";
    private const string Gray = "\x1B[90m";

    private const string Banner = @"
                ____ ____    _         _     _   _
     _ __ ___  / ___| __ )  / \       | |__ | |_| |_ _ __
    | '_ ` _ \| |  _|  _ \ / _ \ _____| '_ \| __| __| '_ \
    | | | | | | |_| | |_) / ___ \_____| | | | |_| |_| |_) |
    |_| |_| |_|\____|____/_/   \_\    |_| |_|\__|\__| .__/
                                                    |_|
";

    private readonly bool _detailed;
    private readonly Lock _gate = new();

    public ConsoleReporter(IOptions<ConsoleOptions> options) => _detailed = options.Value.Detailed;

    public void Header(IEnumerable<string> urls)
    {
        lock (_gate)
        {
            Console.WriteLine($"{Purple}{Banner}{Reset}");
            Console.WriteLine("    https://github.com/nikouu/mGBA-http\n");

            foreach (var url in urls)
            {
                Console.WriteLine($"Listening on {url}");
            }

            Console.WriteLine("Scalar UI: /scalar");
            Console.WriteLine("OpenAPI JSON: /openapi/v1.json\n");
        }
    }

    public void RequestIn(string method, string path, string? correlationId) =>
        Write(Green, "[INF] ", $"Incoming {method} request to: {path}", _detailed ? $"correlationId={correlationId}" : null);

    public void RequestOut(string path, int status, string text, long elapsedMs, string? correlationId)
    {
        var suffix = string.IsNullOrWhiteSpace(text) ? "" : $" {text.Trim()}";
        var detail = _detailed ? $"correlationId={correlationId} status={status} elapsed={elapsedMs}ms" : null;

        var (color, level) = status >= 400 ? (Red, "[ERR] ") : (Green, "[INF] ");
        Write(color, level, $"Outgoing response from {path}: HTTP {status}{suffix}", detail);
    }

    // For an exception that escaped exception handling entirely (no response/status was ever produced).
    public void RequestFailed(string path, Exception exception, long elapsedMs, string? correlationId)
    {
        var detail = _detailed ? $"correlationId={correlationId} elapsed={elapsedMs}ms" : null;
        Write(Red, "[ERR] ", $"Outgoing response from {path}: {exception.Message}", detail);
    }

    private void Write(string color, string level, string message, string? detail)
    {
        var timestamp = DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss] ");

        lock (_gate)
        {
            Console.Out.Write(timestamp);
            Console.Out.Write(color);
            Console.Out.Write(level);
            Console.Out.Write(Reset);
            Console.Out.WriteLine(message);

            if (_detailed && detail is not null)
            {
                Console.Out.WriteLine($"    {Gray}{detail}{Reset}");
            }
        }
    }
}
