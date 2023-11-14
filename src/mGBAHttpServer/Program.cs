using mGBAHttpServer.Endpoints;
using mGBAHttpServer.Services;
using Microsoft.OpenApi.Models;

Console.Title = "mGBA-http";

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
});

builder.Services.AddSingleton<SocketService>();
var loggingSection = builder.Configuration.GetSection("logging");

// Allow log filtering if the configs aren't present with prod exe
if (!builder.Environment.IsDevelopment() && !loggingSection.Exists())
{
    builder.Logging.AddFilter((provider, category, logLevel) =>
    {
        if (category.Contains("Microsoft.AspNetCore.Hosting.Diagnostics") && logLevel <= LogLevel.Error)
        {
            return false;
        }
        else if (category.Contains("Microsoft.AspNetCore.Mvc.Infrastructure.DefaultActionDescriptorCollectionProvider")
            && logLevel <= LogLevel.Error)
        {
            return false;
        }
        else if (category.Contains("Microsoft.AspNetCore.StaticFiles.StaticFileMiddleware")
            && logLevel <= LogLevel.Error)
        {
            return false;
        }
        else if (logLevel >= LogLevel.Information)
        {
            return true;
        }
        else
        {
            return false;
        }
    });
}

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