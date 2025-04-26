using mGBAHttpServer.Endpoints;
using mGBAHttpServer.Models;
using mGBAHttpServer.SchemaFilter;
using mGBAHttpServer.Services;
using Microsoft.OpenApi.Models;
using System.Reflection;

var version = Assembly.GetExecutingAssembly().GetName().Version;
var programVersionString = $"v{version?.Major}.{version?.Minor}.{version?.Build}";
var swaggerVersionString = $"v{version?.Major}.{version?.Minor}";

Console.Title = $"mGBA-http {programVersionString}";

Console.WriteLine(
"""
                ____ ____    _         _     _   _         
     _ __ ___  / ___| __ )  / \       | |__ | |_| |_ _ __  
    | '_ ` _ \| |  _|  _ \ / _ \ _____| '_ \| __| __| '_ \ 
    | | | | | | |_| | |_) / ___ \_____| | | | |_| |_| |_) |
    |_| |_| |_|\____|____/_/   \_\    |_| |_|\__|\__| .__/ 
                                                    |_|                                                       
                                                      
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
builder.Services.AddTransient<SocketService>();

var loggingSection = builder.Configuration.GetSection("logging");

var app = builder.Build();

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

// Make the implicit Program class public so test projects can access it
public partial class Program { }
