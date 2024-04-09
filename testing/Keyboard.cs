using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mGBAHttpServerExamples
{
    public class Keyboard
    {
        public async Task Run()
        {
            Console.Title = "mGBA-http Examples: Keyboard";
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

            Console.WriteLine("Test client to pass keys to mGBA via mGBA-http.");
            Console.WriteLine("Buttons: A:X, B:Z, L:A, R:S, Start:Enter, Select:Backspace, Arrow keys. q to exit");

            Console.Write(">>\n");

            while (Console.ReadKey(true) is ConsoleKeyInfo consoleKeyInfo)
            {
                var keyString = consoleKeyInfo.Key.ToString();

                if (keyString == "Q")
                {
                    return;
                }

                if (keysDictionary.ContainsKey(keyString))
                {
                    Console.WriteLine(keysDictionary[keyString]);
                    await SendKey(keysDictionary[keyString]);                    
                }
            }

            async Task SendKey(string key)
            {
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, $"http://localhost:5000/mgba-http/button/tap?key={key}");
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Unable to send key: {e.Message}");
                }
            }
        }
    }
}
