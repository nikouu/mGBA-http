Console.Title = "mGBA-http Test Client";
var client = new HttpClient();

var keysDictionary = new Dictionary<string, string>()
{
    {"X", "A"},
    {"Z", "B"},
    {"A", "L"},
    {"S", "R"},
    {"Enter", "Start"},
    {"Backspace", "Select"},
    {"UpArrow", "Up" },
    {"RightArrow", "Right" },
    {"DownArrow", "Down" },
    {"LeftArrow", "Left" },
};

Console.Title = "mGBA-http test console";
Console.WriteLine("Test client to pass keys to mGBA via mGBA-http.");
Console.WriteLine("Buttons: A:X, B:Z, L:A, R:S, Start:Enter, Select:Backspace, Arrow keys. q to exit");

Console.Write(">>");

while (Console.ReadKey(true) is ConsoleKeyInfo consoleKeyInfo)
{
    var keyString = consoleKeyInfo.Key.ToString();

    if (keysDictionary.ContainsKey(keyString))
    {
        await SendKey(keysDictionary[keyString]);
        Console.WriteLine(keysDictionary[keyString]);
    }
}

async Task SendKey(string key)
{
    var request = new HttpRequestMessage(HttpMethod.Post, $"http://localhost:5000/mgba-http/button/tap?key={key}");
    var response = await client.SendAsync(request);
    response.EnsureSuccessStatusCode();
}