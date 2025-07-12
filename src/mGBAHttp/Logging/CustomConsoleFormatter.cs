using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace mGBAHttp.Logging;

public sealed class CustomConsoleFormatter : ConsoleFormatter
{
    private const string DefaultForegroundColor = "\x1B[39m";
    private const string ResetAll = "\x1B[0m";
    private const string DarkerGray = "\x1B[90m";  // Using bright black which appears as dark gray

    private readonly IDisposable? _optionsReloadToken;
    private readonly JsonSerializerOptions _jsonOptions;
    private mGBAHttpConsoleFormatterOptions _formatterOptions;

    public CustomConsoleFormatter(IOptionsMonitor<mGBAHttpConsoleFormatterOptions> options)
        : base("CustomFormat")
    {
        _optionsReloadToken = options.OnChange(ReloadLoggerOptions);
        _formatterOptions = options.CurrentValue;
        _jsonOptions = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = false
        };
    }

    private void ReloadLoggerOptions(mGBAHttpConsoleFormatterOptions options) => _formatterOptions = options;

    public override void Write<TState>(
        in LogEntry<TState> logEntry,
        IExternalScopeProvider? scopeProvider,
        TextWriter textWriter)
    {
        var message = logEntry.Formatter?.Invoke(logEntry.State, logEntry.Exception);
        if (message is null)
        {
            return;
        }

        var timestamp = DateTime.Now;

        // Write the main log line
        textWriter.Write(timestamp.ToString(_formatterOptions.TimestampFormat));

        var (colorCode, levelString) = GetLogLevelInfo(logEntry.LogLevel);
        textWriter.Write(colorCode);
        textWriter.Write(levelString);
        textWriter.Write(ResetAll);

        textWriter.Write(message);

        if (logEntry.Exception != null)
        {
            textWriter.Write(" Exception: ");
            textWriter.Write(logEntry.Exception.Message);
        }

        textWriter.WriteLine();

        // Only write JSON details if enabled
        if (_formatterOptions.IncludeJsonDetails)
        {
            textWriter.Write("    "); // Indent
            textWriter.Write(DarkerGray);

            var jsonLog = new
            {
                EventId = logEntry.EventId.Id,
                Level = logEntry.LogLevel.ToString(),
                Category = logEntry.Category,
                Message = message,
                Timestamp = timestamp.ToString("O"),
                CorrelationId = GetCorrelationId(scopeProvider)
            };

            textWriter.Write(JsonSerializer.Serialize(jsonLog, _jsonOptions));
            textWriter.Write(ResetAll);
            textWriter.WriteLine();
        }
    }

    private static string? GetCorrelationId(IExternalScopeProvider? scopeProvider)
    {
        string? correlationId = null;

        scopeProvider?.ForEachScope<Dictionary<string, object>>((scope, _) =>
        {
            if (scope is IEnumerable<KeyValuePair<string, object>> pairs)
            {
                foreach (var pair in pairs)
                {
                    if (pair.Key == "CorrelationId")
                    {
                        correlationId = pair.Value?.ToString();
                        break;
                    }
                }
            }
        }, state: null);

        return correlationId;
    }

    private static (string ColorCode, string Text) GetLogLevelInfo(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => (GetForegroundColorEscapeCode(ConsoleColor.Gray), "[TRC] "),
            LogLevel.Debug => (GetForegroundColorEscapeCode(ConsoleColor.Gray), "[DBG] "),
            LogLevel.Information => (GetForegroundColorEscapeCode(ConsoleColor.Green), "[INF] "),
            LogLevel.Warning => (GetForegroundColorEscapeCode(ConsoleColor.Yellow), "[WRN] "),
            LogLevel.Error => (GetForegroundColorEscapeCode(ConsoleColor.Red), "[ERR] "),
            LogLevel.Critical => (GetForegroundColorEscapeCode(ConsoleColor.DarkRed), "[CRT] "),
            _ => (GetForegroundColorEscapeCode(ConsoleColor.Gray), "[???] ")
        };
    }

    private static string GetForegroundColorEscapeCode(ConsoleColor color) =>
        color switch
        {
            ConsoleColor.Black => "\x1B[30m",
            ConsoleColor.DarkRed => "\x1B[31m",
            ConsoleColor.DarkGreen => "\x1B[32m",
            ConsoleColor.DarkYellow => "\x1B[33m",
            ConsoleColor.DarkBlue => "\x1B[34m",
            ConsoleColor.DarkMagenta => "\x1B[35m",
            ConsoleColor.DarkCyan => "\x1B[36m",
            ConsoleColor.Gray => "\x1B[37m",
            ConsoleColor.Red => "\x1B[1m\x1B[31m",
            ConsoleColor.Green => "\x1B[1m\x1B[32m",
            ConsoleColor.Yellow => "\x1B[1m\x1B[33m",
            ConsoleColor.Blue => "\x1B[1m\x1B[34m",
            ConsoleColor.Magenta => "\x1B[1m\x1B[35m",
            ConsoleColor.Cyan => "\x1B[1m\x1B[36m",
            ConsoleColor.White => "\x1B[1m\x1B[37m",
            _ => DefaultForegroundColor
        };

    public void Dispose() => _optionsReloadToken?.Dispose();
}

public sealed class mGBAHttpConsoleFormatterOptions : ConsoleFormatterOptions
{
    public string TimestampFormat { get; set; } = "[yyyy-MM-dd HH:mm:ss] ";
    public bool IncludeJsonDetails { get; set; } = false; // Defaults to false if not specified
}
