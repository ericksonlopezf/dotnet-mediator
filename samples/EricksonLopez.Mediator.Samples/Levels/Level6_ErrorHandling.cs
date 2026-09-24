// Copyright © Erickson Lopez. MIT License.
using System;
using System.Threading;
using System.Threading.RateLimiting;
using System.Threading.Tasks;
using EricksonLopez.Mediator;
using EricksonLopez.Mediator.RateLimiting;
using EricksonLopez.Mediator.Testing;

namespace Sample.Levels.Level6_ErrorHandling;

// --- 1. High-frequency Command for Rate Limiting Demonstration ---

/// <summary>
/// Represents a high-frequency operation subject to rate limiting.
/// </summary>
/// <remarks>
/// Decorated with <see cref="UseBehavior{T}"/> to apply <see cref="RateLimitingBehavior{TRequest,TResponse}"/>
/// exclusively to this request type, independent of other global behaviors.
/// </remarks>
/// <param name="ReportType">The type of report to generate.</param>
/// <param name="RequestorId">The identifier of the user or system requesting the report.</param>
[UseBehavior(typeof(RateLimitingBehavior<,>), order: 10)]
public sealed record GenerateReportCommand(string ReportType, string RequestorId) : ICommand<string>;

/// <summary>Handles report generation for <see cref="GenerateReportCommand"/>.</summary>
public sealed class GenerateReportCommandHandler : ICommandHandler<GenerateReportCommand, string>
{
    /// <inheritdoc/>
    public ValueTask<string> Handle(GenerateReportCommand command, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[Level 6 - Handler] Generating '{command.ReportType}' report for {command.RequestorId}.");
        return ValueTask.FromResult($"Report-{command.ReportType}-{Guid.NewGuid():N}");
    }
}

// --- 2. Notification with Exception Aggregation Strategy ---

/// <summary>
/// Represents a critical system notification published with <see cref="PublishStrategy.SequentialAggregateExceptions"/>
/// so that all handlers execute even when one or more fail, and all exceptions are collected into
/// a single <see cref="NotificationHandlerAggregateException"/>.
/// </summary>
/// <param name="AlertMessage">The message text associated with the alert.</param>
[PublishStrategy(PublishStrategy.SequentialAggregateExceptions)]
public sealed record CriticalSystemEvent(string AlertMessage) : INotification;

/// <summary>Demonstrates a notification handler that throws an exception during processing.</summary>
public sealed class HandlerThatFails1 : INotificationHandler<CriticalSystemEvent>
{
    /// <inheritdoc/>
    /// <exception cref="InvalidOperationException">The messaging service connection fails intentionally</exception>
    public ValueTask Handle(CriticalSystemEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine("[Level 6 - Handler 1] Processing alert... -> Failing intentionally.");
        throw new InvalidOperationException("Handler 1: Unable to connect to messaging service.");
    }
}

/// <summary>Demonstrates a notification handler that completes successfully.</summary>
public sealed class HandlerThatSucceeds : INotificationHandler<CriticalSystemEvent>
{
    /// <inheritdoc/>
    public ValueTask Handle(CriticalSystemEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine("[Level 6 - Handler 2] Processing alert... -> Completed successfully.");
        return ValueTask.CompletedTask;
    }
}

/// <summary>Demonstrates a notification handler that throws a timeout exception during processing.</summary>
public sealed class HandlerThatFails2 : INotificationHandler<CriticalSystemEvent>
{
    /// <inheritdoc/>
    /// <exception cref="TimeoutException">A timeout occurs writing to the database replica</exception>
    public ValueTask Handle(CriticalSystemEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine("[Level 6 - Handler 3] Processing alert... -> Failing intentionally.");
        throw new TimeoutException("Handler 3: Timeout writing to secondary database replica.");
    }
}

/// <summary>
/// Demonstrates error handling, rate limiting, and exception aggregation strategies.
/// </summary>
/// <remarks>
/// <para>
/// <strong>On resilience (retry / circuit breaker / timeout):</strong> These strategies are not part
/// of <c>EricksonLopez.Mediator</c> core. Polly-based resilience is provided by the deprecated
/// <c>EricksonLopez.Mediator.Polly</c> package (archived in v2.0 per ADR-036). The recommended
/// replacement is <c>EricksonLopez.Resilience.Mediator</c>, which wires <c>IResiliencePipeline</c>
/// into the mediator pipeline via a Clean Architecture adapter without leaking Polly types into the
/// Application layer.
/// </para>
/// <para>
/// See: <c>EricksonLopez.Resilience</c> README — Migration from Mediator.Polly.
/// </para>
/// </remarks>
public static class Demo
{
    /// <summary>Executes the Level 6 error handling demonstration.</summary>
    /// <param name="mediator">The mediator instance to use for dispatching.</param>
    /// <returns>A task representing the asynchronous demonstration operation.</returns>
    public static async Task RunAsync(IMediator mediator)
    {
        Console.WriteLine("================================================================================");
        Console.WriteLine("  LEVEL 6: ERROR HANDLING, RATE LIMITING & EXCEPTION AGGREGATION");
        Console.WriteLine("================================================================================");

        // 1. In-process Rate Limiting with RateLimitingBehavior<TRequest,TResponse>
        //    EricksonLopez.Mediator.RateLimiting provides a 100% AOT-safe pipeline behavior built on
        //    System.Threading.RateLimiting (BCL-native, no external dependencies).
        //
        //    Configuration via DI:
        //      services.AddMediatorRateLimiting();
        //      services.AddSingleton<RateLimiter>(new TokenBucketRateLimiter(...));
        //
        //    Alternatively, use [UseBehavior(typeof(RateLimitingBehavior<,>))] for per-request application.

        Console.WriteLine("1. Rate Limiting — direct RateLimiter instantiation with ConcurrencyLimiter:");
        using var concurrencyLimiter = new ConcurrencyLimiter(new ConcurrencyLimiterOptions
        {
            PermitLimit = 3,
            QueueLimit = 0,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
        });

        var behavior = new RateLimitingBehavior<GenerateReportCommand, string>(concurrencyLimiter);
        Console.WriteLine($"   -> RateLimitingBehavior<GenerateReportCommand, string> instantiated with ConcurrencyLimiter(PermitLimit=3).");
        Console.WriteLine($"   -> Implements IPipelineBehavior<GenerateReportCommand, string> with struct INext<string> continuation.");
        Console.WriteLine($"   -> Registration pattern: [assembly: UseGlobalBehavior(typeof(RateLimitingBehavior<,>), order: 10)]");
        Console.WriteLine($"   -> Or per-request:       [UseBehavior(typeof(RateLimitingBehavior<,>), order: 10)] on the command/query.");
        Console.WriteLine();

        // Execute via IMediator — RateLimitingBehavior configured globally in Program.cs for all requests
        Console.WriteLine("2. Execute command through IMediator with rate limiting active:");
        var reportResult = await mediator.SendCommand<GenerateReportCommand, string>(
            new GenerateReportCommand("FinancialSummary", "user-42"), CancellationToken.None);
        Console.WriteLine($"   -> Report generated: {reportResult[..16]}...");
        Console.WriteLine();

        // RateLimitExceededException — when the rate limiter denies the lease
        Console.WriteLine("3. RateLimitExceededException — burst protection enforcement:");
        using var exhaustedLimiter = new ConcurrencyLimiter(new ConcurrencyLimiterOptions
        {
            PermitLimit = 1,   // One permit total
            QueueLimit = 0,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
        });
        // Pre-acquire the single permit — limiter is now fully saturated (0 slots available)
        using var preAcquiredLease = await exhaustedLimiter.AcquireAsync(1);
        var saturatedBehavior = new RateLimitingBehavior<GenerateReportCommand, string>(exhaustedLimiter);
        try
        {
            // Invoke the behavior directly to demonstrate the exception path
            await saturatedBehavior.Handle(
                new GenerateReportCommand("AuditReport", "attacker-99"),
                new DelegateNext<string>("never-reached"),
                CancellationToken.None);
        }
        catch (RateLimitExceededException ex)
        {
            Console.WriteLine($"   -> Caught RateLimitExceededException: {ex.Message}");
            Console.WriteLine($"   -> RetryAfter: {(ex.RetryAfter.HasValue ? ex.RetryAfter.Value.ToString() : "not provided")}");
        }
        Console.WriteLine();

        // 2. Publishing with Exception Aggregation Strategy
        Console.WriteLine("4. Publishing with [PublishStrategy(PublishStrategy.SequentialAggregateExceptions)]:");
        try
        {
            var alertEvent = new CriticalSystemEvent("High CPU alert on node 3");
            await mediator.Publish(alertEvent, CancellationToken.None);
        }
        catch (NotificationHandlerAggregateException aggEx)
        {
            Console.WriteLine($"   -> Caught NotificationHandlerAggregateException with {aggEx.HandlerExceptions.Count} exception(s):");
            foreach (var inner in aggEx.HandlerExceptions)
            {
                Console.WriteLine($"      * [{inner.GetType().Name}]: {inner.Message}");
            }
        }

        Console.WriteLine("--------------------------------------------------------------------------------\n");
    }
}
