using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Diagnosers;
BenchmarkRunner.Run<RecipeApiBenchmarks>();

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net90, launchCount: 1, warmupCount: 2, iterationCount: 5)]
public class RecipeApiBenchmarks
{
    private HttpClient _client = null!;

    [GlobalSetup]
    public void Setup()
    {
        _client = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5200/"),
            Timeout = TimeSpan.FromSeconds(30)
        };

        try
        {
            var testResponse = _client.GetAsync("/api/recipes").Result;
            testResponse.EnsureSuccessStatusCode();
            Debug.WriteLine("API connection verified");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"API connection failed: {ex.Message}", ex);
        }
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _client?.Dispose();
    }

    [Benchmark(Baseline = true, Description = "500 Sequential Requests")]
    public async Task Sequential500()
    {
        for (int i = 0; i < 500; i++)
        {
            var response = await _client.GetAsync("/api/recipes");
            response.EnsureSuccessStatusCode();
            _ = await response.Content.ReadAsStringAsync();
        }
    }

    [Benchmark(Description = "500 Requests in Batches of 50")]
    public async Task BatchesOf50()
    {
        const int totalRequests = 500;
        const int batchSize = 50;

        for (int batchStart = 0; batchStart < totalRequests; batchStart += batchSize)
        {
            var tasks = new List<Task>();
            for (int i = 0; i < batchSize && (batchStart + i) < totalRequests; i++)
            {
                tasks.Add(ProcessRequestAsync());
            }
            await Task.WhenAll(tasks);
        }
    }

    [Benchmark(Description = "500 Requests with 20 Concurrent")]
    public async Task ConcurrentWithLimit()
    {
        var semaphore = new SemaphoreSlim(20, 20);
        var tasks = new List<Task>();

        for (int i = 0; i < 500; i++)
        {
            tasks.Add(ProcessRequestWithSemaphoreAsync(semaphore));
        }

        await Task.WhenAll(tasks);
    }

    [Benchmark(Description = "100 Concurrent Requests")]
    public async Task Concurrent100()
    {
        var semaphore = new SemaphoreSlim(10, 10);
        var tasks = new List<Task>();

        for (int i = 0; i < 100; i++)
        {
            tasks.Add(ProcessRequestWithSemaphoreAsync(semaphore));
        }

        await Task.WhenAll(tasks);
    }

    private async Task ProcessRequestAsync()
    {
        var response = await _client.GetAsync("/api/recipes");
        response.EnsureSuccessStatusCode();
        _ = await response.Content.ReadAsStringAsync();
    }

    private async Task ProcessRequestWithSemaphoreAsync(SemaphoreSlim semaphore)
    {
        await semaphore.WaitAsync();
        try
        {
            await ProcessRequestAsync();
        }
        finally
        {
            semaphore.Release();
        }
    }
}