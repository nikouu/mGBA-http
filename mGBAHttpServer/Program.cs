using mGBAHttpServer.Endpoints;
using mGBAHttpServer.Services;

Console.Title ="mGBA-http";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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