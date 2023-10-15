using mGBAHttpServer.Endpoints;
using mGBAHttpServer.Services;
using Microsoft.OpenApi.Models;

Console.Title ="mGBA-http";

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

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.MapButtonEndpoints();
app.MapConsoleEndpoints();
app.MapCoreEndpoints();
app.MapMemoryEndpoints();

app.Run();