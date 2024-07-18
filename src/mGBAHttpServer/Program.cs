using mGBAHttpServer.Endpoints;
using mGBAHttpServer.Models;
using mGBAHttpServer.SchemaFilter;
using mGBAHttpServer.Services;
using Microsoft.OpenApi.Models;
using System.Reflection;

var version = Assembly.GetExecutingAssembly().GetName().Version;
Console.Title = $"mGBA-http {version?.Major}.{version?.Minor}.{version?.Build}";

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
    options.SwaggerDoc("v1", new OpenApiInfo
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
builder.Services.AddSingleton<SocketService>();

var loggingSection = builder.Configuration.GetSection("logging");

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

Console.WriteLine("Swagger UI: /index.html");
Console.WriteLine("Swagger JSON: /swagger/v1/swagger.json\n");

app.MapCoreEndpoints();
app.MapConsoleEndpoints();
app.MapCoreAdapterEndpoints();
app.MapMemoryDomainEndpoints();
app.MapButtonEndpoints();

app.Run();