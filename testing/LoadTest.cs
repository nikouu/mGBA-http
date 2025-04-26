using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
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
        private readonly ConcurrentQueue<(DateTime RequestTime, TimeSpan Duration, bool Success)> _metrics;
        private readonly ConcurrentBag<Task> _pendingRequests;

        public LoadTest(HttpClient client, int requestsPerSecond = 10)
        {
            _client = client ?? new HttpClient();
            _requestsPerSecond = requestsPerSecond;
            _metrics = new ConcurrentQueue<(DateTime, TimeSpan, bool)>();
            _pendingRequests = new ConcurrentBag<Task>();
        }

        public async Task RunLoadTest(KeysEnum key, TimeSpan duration)
        {
            Console.WriteLine($"Starting load test: target {_requestsPerSecond} requests/second for {duration.TotalSeconds} seconds");

            using var cts = new CancellationTokenSource(duration);
            using var secondTimer = new PeriodicTimer(TimeSpan.FromSeconds(1));

            // Start a background task to periodically report metrics
            var reportingTask = ReportMetricsAsync(duration, cts.Token);

            try
            {
                while (await secondTimer.WaitForNextTickAsync(cts.Token))
                {
                    // Launch requests and keep track of them
                    for (int i = 0; i < _requestsPerSecond; i++)
                    {
                        var task = SendRequestAndRecordMetricsAsync(key);
                        _pendingRequests.Add(task);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Normal termination when duration expires
                Console.WriteLine("\nTest duration completed. Waiting for pending requests...");
            }

            try
            {
                // Wait for all pending requests to complete
                await Task.WhenAll(_pendingRequests);
                
                // Cancel and wait for the reporting task
                cts.Cancel();
                try { await reportingTask; } catch (OperationCanceledException) { }
            }
            finally
            {
                // Final report after all requests have completed
                AnalyzeAndReportFinalMetrics();
            }
        }

        private async Task SendRequestAndRecordMetricsAsync(KeysEnum key)
        {
            var requestTime = DateTime.UtcNow;
            var startTime = Stopwatch.GetTimestamp();

            try
            {
                var success = await GetKeyRequest(key);
                var duration = Stopwatch.GetElapsedTime(startTime);
                _metrics.Enqueue((requestTime, duration, success));
            }
            catch
            {
                var duration = Stopwatch.GetElapsedTime(startTime);
                _metrics.Enqueue((requestTime, duration, false));
            }
        }

        private async Task ReportMetricsAsync(TimeSpan testDuration, CancellationToken ct)
        {
            var reportInterval = TimeSpan.FromSeconds(5); // Report every 5 seconds

            try
            {
                while (!ct.IsCancellationRequested)
                {
                    await Task.Delay(reportInterval, ct);
                    var now = DateTime.UtcNow;
                    var recentMetrics = _metrics
                        .Where(m => m.RequestTime >= now.AddSeconds(-5))
                        .ToList();

                    if (recentMetrics.Any())
                    {
                        var pendingCount = _pendingRequests.Count(t => !t.IsCompleted);
                        var requestRate = recentMetrics.Count / 5.0; // 5-second window
                        var avgLatency = recentMetrics.Average(m => m.Duration.TotalMilliseconds);
                        var successRate = recentMetrics.Count(m => m.Success) / (double)recentMetrics.Count * 100;

                        Console.WriteLine($"Last 5s - Rate: {requestRate:F1} req/s, Avg Latency: {avgLatency:F1}ms, Success: {successRate:F1}%, Pending: {pendingCount}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when the test duration expires
                Console.WriteLine("Metrics reporting completed.");
            }
        }

        private void AnalyzeAndReportFinalMetrics()
        {
            var allMetrics = _metrics.ToList();
            if (!allMetrics.Any()) return;

            var totalTime = (allMetrics.Max(m => m.RequestTime) - allMetrics.Min(m => m.RequestTime)).TotalSeconds;
            var actualRate = allMetrics.Count / totalTime;
            var avgLatency = allMetrics.Average(m => m.Duration.TotalMilliseconds);
            var successRate = allMetrics.Count(m => m.Success) / (double)allMetrics.Count * 100;

            Console.WriteLine("\nFinal Results:");
            Console.WriteLine($"Total Requests: {allMetrics.Count}");
            Console.WriteLine($"Actual Rate: {actualRate:F1} requests/second");
            Console.WriteLine($"Average Latency: {avgLatency:F1}ms");
            Console.WriteLine($"Success Rate: {successRate:F1}%");
        }

        private async Task<bool> GetKeyRequest(KeysEnum key)
        {
            try
            {
                var response = await _client.GetAsync($"/core/getkey?key={key}");
                if (response.StatusCode != HttpStatusCode.OK)
                    return false;

                var content = await response.Content.ReadAsStringAsync();
                return content == "0";
            }
            catch
            {
                return false;
            }
        }
    }
}
