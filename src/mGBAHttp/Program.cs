using mGBAHttp;
using mGBAHttp.Domain;
using mGBAHttp.Endpoints;
using mGBAHttp.Logging;
using mGBAHttp.Models;
using mGBAHttp.OpenApi;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
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

var builder = WebApplication.CreateSlimBuilder(new WebApplicationOptions
{
    Args = args,
    // appsettings.json is resolved from the content root, which defaults to the working directory.
    // mGBA-http ships appsettings.json beside the binary, so anything launching it from elsewhere
    // (a script, a shortcut with "Start in" set) would silently ignore the file.
    ContentRootPath = AppContext.BaseDirectory
});

builder.WebHost.UseKestrelHttpsConfiguration();

// Short aliases for the settings users override most. The full keys (--mgba-http:Socket:Port)
// and the built in --urls keep working.
builder.Configuration.AddCommandLine(args, new Dictionary<string, string>
{
    ["--mgba-ip"] = "mgba-http:Socket:IpAddress",
    ["--mgba-port"] = "mgba-http:Socket:Port",
    ["--detailed"] = "mgba-http:Console:Detailed"
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

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

// For binding request errors
builder.Services.Configure<RouteHandlerOptions>(options => options.ThrowOnBadRequest = true);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Error);

builder.Services.Configure<ConsoleOptions>(builder.Configuration.GetSection(ConsoleOptions.Section));
builder.Services.AddSingleton<ConsoleReporter>();

var app = builder.Build();

// Printed once the server is up so the addresses are the ones actually bound, not the ones configured.
app.Lifetime.ApplicationStarted.Register(() =>
{
    var addresses = app.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()?.Addresses;
    app.Services.GetRequiredService<ConsoleReporter>().Header(addresses ?? []);
});

app.UseRequestReporting();

app.UseExceptionHandler();

app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options
        .WithClassicLayout()
        .WithTheme(ScalarTheme.Purple);
});

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
