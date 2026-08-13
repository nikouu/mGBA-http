using mGBAHttp;
using mGBAHttp.Domain;
using mGBAHttp.Endpoints;
using mGBAHttp.Logging;
using mGBAHttp.Models;
using mGBAHttp.OpenApi;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;
using System.Reflection;
using System.Runtime.InteropServices;

var version = Assembly.GetExecutingAssembly().GetName().Version;
var programVersionString = $"v{version?.Major}.{version?.Minor}.{version?.Build}";

SetupConsoleAnsiSupport();

Console.Title = $"mGBA-http {programVersionString}";

Console.WriteLine(
$"\x1B[1m\x1B[35m{"""
                ____ ____    _         _     _   _         
     _ __ ___  / ___| __ )  / \       | |__ | |_| |_ _ __  
    | '_ ` _ \| |  _|  _ \ / _ \ _____| '_ \| __| __| '_ \ 
    | | | | | | |_| | |_) / ___ \_____| | | | |_| |_| |_) |
    |_| |_| |_|\____|____/_/   \_\    |_| |_|\__|\__| .__/ 
                                                    |_|                                                       
"""}\x1B[0m");

Console.WriteLine("""
    https://github.com/nikouu/mGBA-http

""");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<DocumentInfoTransformer>();
    options.AddOperationTransformer<ResponseExampleTransformer>();
});

builder.Services.AddOptions<SocketOptions>()
    .Bind(builder.Configuration.GetSection(SocketOptions.Section))
    .Validate(o => System.Net.IPAddress.TryParse(o.IpAddress, out _), "mgba-http:Socket:IpAddress must be a valid IP address.")
    .Validate(o => o.Port is > 0 and <= 65535, "mgba-http:Socket:Port must be between 1 and 65535.")
    .Validate(o => o.ReadTimeout > 0, "mgba-http:Socket:ReadTimeout must be greater than 0 (milliseconds).")
    .Validate(o => o.WriteTimeout > 0, "mgba-http:Socket:WriteTimeout must be greater than 0 (milliseconds).")
    .ValidateOnStart();

builder.Services.TryAddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();

builder.Services.TryAddSingleton(serviceProvider =>
{
    var provider = serviceProvider.GetRequiredService<ObjectPoolProvider>();
    var socketOptions = serviceProvider.GetRequiredService<IOptions<SocketOptions>>();
    var policy = new ReusableSocketPooledObjectPolicy(socketOptions.Value);
    return provider.Create(policy);
});

builder.Services.AddExceptionHandler<MgbaExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Logging.ClearProviders();
builder.Logging.AddConsole(options =>
{
    options.FormatterName = "CustomFormat";
}).AddConsoleFormatter<CustomConsoleFormatter, mGBAHttpConsoleFormatterOptions>(options =>
{
    builder.Configuration.GetSection("Logging:Console:FormatterOptions").Bind(options);
});

var app = builder.Build();

app.UseExceptionHandler();

app.UseRequestResponseLogging();

app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options
        .WithClassicLayout()
        .WithTheme(ScalarTheme.Purple);
});

Console.WriteLine("Scalar UI: /scalar");
Console.WriteLine("OpenAPI JSON: /openapi/v1.json\n");

app.MapCoreEndpoints();
app.MapConsoleEndpoints();
app.MapCoreAdapterEndpoints();
app.MapMemoryDomainEndpoints();
app.MapButtonEndpoints();
app.MapExtensionEndpoints();

app.Run();

static void SetupConsoleAnsiSupport()
{
    if (OperatingSystem.IsWindows())
    {
        var handle = GetStdHandle(-11);
        GetConsoleMode(handle, out int mode);
        SetConsoleMode(handle, mode | 0x4);
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern nint GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll")]
    static extern bool GetConsoleMode(nint hConsoleHandle, out int lpMode);

    [DllImport("kernel32.dll")]
    static extern bool SetConsoleMode(nint hConsoleHandle, int dwMode);
}

// Make the implicit Program class public so test projects can access it
public partial class Program { }
