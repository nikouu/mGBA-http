using System.Net.Sockets;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.MapPost("/message", async ([FromBody] mGBAMessage message) =>
{
    await SendMessage(message.message);

    return Results.Ok();
});

async Task SendMessage(string message)
{

    IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync("localhost");
    IPAddress ipAddress = ipHostInfo.AddressList[1]; // 1 to get the ipv4 version, as for me it seems to do the ipv6 as the 0th 
    IPEndPoint ipEndPoint = new(ipAddress, 8888);

    // https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/sockets/socket-services
    using Socket client = new(
        ipEndPoint.AddressFamily,
        SocketType.Stream,
        ProtocolType.Tcp);

    await client.ConnectAsync(ipEndPoint);
    while (true)
    {
        // Send message.
        Console.WriteLine("Input string and hit enter:");
        var messageBytes = Encoding.UTF8.GetBytes(message);
        _ = await client.SendAsync(messageBytes, SocketFlags.None);
        Console.WriteLine($"Socket client sent message: \"{message}\"");

        // Receive ack.
        var buffer = new byte[1_024];
        var received = await client.ReceiveAsync(buffer, SocketFlags.None);
        var response = Encoding.UTF8.GetString(buffer, 0, received);
        if (response == "<|ACK|>")
        {
            Console.WriteLine(
                $"Socket client received acknowledgment: \"{response}\"");
            break;
        }
        // Sample output:
        //     Socket client sent message: "Hi friends 👋!<|EOM|>"
        //     Socket client received acknowledgment: "<|ACK|>"
    }

    client.Shutdown(SocketShutdown.Both);
}


app.Run();

readonly record struct mGBAMessage(string message);
