using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace mGBAHttpServerExamples
{
    public enum KeysEnum
    {
        A,
        B,
        L,
        R,
        Start,
        Select,
        Up,
        Down,
        Left,
        Right
    }

    public class LoadTest
    {
        private readonly HttpClient _client;
        private readonly int _requestsPerSecond;
        
        public LoadTest(HttpClient client, int requestsPerSecond = 10)
        {
            _client = client ?? new HttpClient();
            _requestsPerSecond = requestsPerSecond;
        }
        
        public async Task RunLoadTest(KeysEnum key, TimeSpan duration)
        {
            Console.WriteLine($"Starting load test: {_requestsPerSecond} requests/second for {duration.TotalSeconds} seconds");
            
            var stopwatch = Stopwatch.StartNew();
            int totalRequests = 0;
            int successfulRequests = 0;
            
            // Create a PeriodicTimer that ticks every second
            using var secondTimer = new PeriodicTimer(TimeSpan.FromSeconds(1));
            
            while (await secondTimer.WaitForNextTickAsync())
            {
                if (stopwatch.Elapsed >= duration)
                    break;
                    
                var tasks = new List<Task>();
                
                // Create batch of requests for this second
                for (int i = 0; i < _requestsPerSecond; i++)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        try
                        {
                            var result = await GetKeyRequest(key);
                            if (result)
                            {
                                Interlocked.Increment(ref successfulRequests);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Request error: {ex.Message}");
                        }
                        
                        Interlocked.Increment(ref totalRequests);
                    }));
                }
                
                // Wait for all requests in this batch to complete
                await Task.WhenAll(tasks);
            }
            
            stopwatch.Stop();
            
            Console.WriteLine($"Load test completed: {totalRequests} total requests, {successfulRequests} successful");
            Console.WriteLine($"Actual rate: {totalRequests / stopwatch.Elapsed.TotalSeconds:F2} requests/second");
        }
        
        private async Task<bool> GetKeyRequest(KeysEnum key)
        {
            try
            {
                var response = await _client.GetAsync($"/core/getkey?key={key}");
                
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return false;
                }
                
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(content);
                return content == "0";
            }
            catch
            {
                return false;
            }
        }
    }
}
