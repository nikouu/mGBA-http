using mGBAHttpServerExamples;

//await new Keyboard().Run();

// Example usage in Program.cs
var client = new HttpClient() { BaseAddress = new Uri("http://localhost:5000") }; // Set your base address
var loadTest = new LoadTest(client, requestsPerSecond: 1); // Set desired requests per second
await loadTest.RunLoadTest(KeysEnum.A, TimeSpan.FromSeconds(30)); // Run for 30 seconds