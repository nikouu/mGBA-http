using mGBAHttpServer.Domain;
using mGBAHttpServer.Endpoints;
using mGBAHttpServer.Logging;
using mGBAHttpServer.Models;
using mGBAHttpServer.SchemaFilter;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Runtime.InteropServices;



var version = Assembly.GetExecutingAssembly().GetName().Version;
var programVersionString = $"v{version?.Major}.{version?.Minor}.{version?.Build}";
var swaggerVersionString = $"v{version?.Major}.{version?.Minor}";

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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(swaggerVersionString, new OpenApiInfo
    {
        Title = "mGBA-http",
        Description = "An HTTP interface for mGBA scripting.",
        Contact = new OpenApiContact
        {
            Name = "GitHub Repository",
            Url = new Uri("https://github.com/nikouu/mGBA-http/")
        }
    });

    // allows enums to be displayed and used as strings
    options.SchemaFilter<EnumSchemaFilter>();
});

builder.Services.Configure<SocketOptions>(builder.Configuration.GetSection(SocketOptions.Section));
builder.Services.TryAddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();

builder.Services.TryAddSingleton(serviceProvider =>
{
    var provider = serviceProvider.GetRequiredService<ObjectPoolProvider>();
    var socketOptions = serviceProvider.GetRequiredService<IOptions<SocketOptions>>();
    var policy = new ReusableSocketPooledObjectPolicy(socketOptions.Value.IpAddress, socketOptions.Value.Port);
    return provider.Create(policy);
});

builder.Logging.ClearProviders();
builder.Logging.AddConsole(options =>
{
    options.FormatterName = "CustomFormat";
}).AddConsoleFormatter<CustomConsoleFormatter, mGBAHttpConsoleFormatterOptions>(options =>
{
    // This will read from configuration if present, otherwise use defaults
    builder.Configuration.GetSection("Logging:Console:FormatterOptions").Bind(options);
});

var app = builder.Build();

app.UseRequestResponseLogging();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint($"/swagger/{swaggerVersionString}/swagger.json", swaggerVersionString);
    options.RoutePrefix = string.Empty;
});

Console.WriteLine("Swagger UI: /index.html");
Console.WriteLine($"Swagger JSON: /swagger/{swaggerVersionString}/swagger.json\n");

app.MapCoreEndpoints();
app.MapConsoleEndpoints();
app.MapCoreAdapterEndpoints();
app.MapMemoryDomainEndpoints();
app.MapButtonEndpoints();

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
