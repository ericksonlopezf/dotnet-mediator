// Copyright © Erickson Lopez. MIT License.
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.RateLimiting;
using System.Threading.Tasks;
using EricksonLopez.Mediator;
using EricksonLopez.Mediator.RateLimiting;

namespace Sample.Levels.Level7_Scalability;

// --- 1. High Throughput Command for Benchmarking ---

/// <summary>Represents a lightweight command used for high-throughput benchmarking.</summary>
/// <param name="Value">The integer value to process.</param>
public sealed record HighThroughputCommand(int Value) : ICommand<int>;

/// <summary>Handles benchmarking calculations for <see cref="HighThroughputCommand"/>.</summary>
public sealed class HighThroughputCommandHandler : ICommandHandler<HighThroughputCommand, int>
{
    /// <inheritdoc/>
    public ValueTask<int> Handle(HighThroughputCommand command, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(command.Value * 2);
    }
}

// --- 2. Rate-Limited Command ---

/// <summary>Represents an API operation subject to rate limiting constraints.</summary>
/// <param name="ClientId">The identifier of the calling client.</param>
public sealed record RateLimitedApiCommand(string ClientId) : ICommand<string>;

/// <summary>Handles authorization and processing for <see cref="RateLimitedApiCommand"/>.</summary>
public sealed class RateLimitedApiCommandHandler : ICommandHandler<RateLimitedApiCommand, string>
{
    /// <inheritdoc/>
    public ValueTask<string> Handle(RateLimitedApiCommand command, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult($"Request approved for client {command.ClientId}");
    }
}

/// <summary>Demonstrates mediator scalability, throughput, and rate limiting.</summary>
public static class Demo
{
    /// <summary>Executes the Level 7 scalability and throughput demonstration.</summary>
    /// <param name="mediator">The mediator instance to use for dispatching.</param>
    /// <returns>A task representing the asynchronous demonstration operation.</returns>
    public static async Task RunAsync(IMediator mediator)
    {
        Console.WriteLine("================================================================================");
        Console.WriteLine("  LEVEL 7: SCALABILITY, THROUGHPUT & RATE LIMITING");
        Console.WriteLine("================================================================================");

        // 1. Rate Limiting Demonstration with RateLimitingBehavior
        Console.WriteLine("1. Rate Limiting Demonstration:");
        var rateLimiter = new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions
        {
            TokenLimit = 2,
            TokensPerPeriod = 2,
            ReplenishmentPeriod = TimeSpan.FromSeconds(10),
            QueueLimit = 0
        });

        var rateLimitedBehavior = new RateLimitingBehavior<RateLimitedApiCommand, string>(rateLimiter);
        var directNext = new EricksonLopez.Mediator.Testing.DelegateNext<string>(() => ValueTask.FromResult("OK"));

        for (int i = 1; i <= 3; i++)
        {
            try
            {
                var result = await rateLimitedBehavior.Handle(new RateLimitedApiCommand("CLIENT_01"), directNext, CancellationToken.None);
                Console.WriteLine($"   -> Request #{i}: {result}");
            }
            catch (RateLimitExceededException ex)
            {
                Console.WriteLine($"   -> Request #{i}: [RateLimitExceededException] {ex.Message}");
            }
        }
        Console.WriteLine();

        // 2. High-Throughput Zero-Allocations Execution (5,000 operations)
        Console.WriteLine("2. Dispatching Throughput Benchmark (5,000 sequential operations):");
        var sw = Stopwatch.StartNew();
        int accumulator = 0;
        const int iterations = 5000;
        for (int i = 0; i < iterations; i++)
        {
            accumulator += await mediator.Send(new HighThroughputCommand(1), CancellationToken.None);
        }
        sw.Stop();

        var opsPerSec = (iterations / sw.Elapsed.TotalSeconds);
        Console.WriteLine($"   -> {iterations:N0} commands executed in {sw.ElapsedMilliseconds} ms ({opsPerSec:N0} ops/sec).");
        Console.WriteLine($"   -> Zero heap overhead across pipeline wrappers (struct INext<TResponse>).");

        Console.WriteLine("--------------------------------------------------------------------------------\n");
    }
}
