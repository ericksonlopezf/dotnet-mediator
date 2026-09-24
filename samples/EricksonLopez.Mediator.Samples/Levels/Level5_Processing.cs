// Copyright © Erickson Lopez. MIT License.
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using EricksonLopez.Mediator;

namespace Sample.Levels.Level5_Processing;

// --- 1. Heavy Async Processing Command ---

/// <summary>Represents a command to process a batch of data items.</summary>
/// <param name="BatchId">The unique identifier of the batch.</param>
/// <param name="ItemCount">The total number of items to process.</param>
public sealed record ProcessBatchDataCommand(string BatchId, int ItemCount) : ICommand<int>;

/// <summary>Handles asynchronous processing for <see cref="ProcessBatchDataCommand"/>.</summary>
public sealed class ProcessBatchDataCommandHandler : ICommandHandler<ProcessBatchDataCommand, int>
{
    /// <inheritdoc/>
    public async ValueTask<int> Handle(ProcessBatchDataCommand command, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[Level 5 - Handler] Starting batch processing '{command.BatchId}' ({command.ItemCount} items)...");
        await Task.Delay(50, cancellationToken).ConfigureAwait(false); // Simulated I/O
        Console.WriteLine($"[Level 5 - Handler] Batch '{command.BatchId}' processed successfully.");
        return command.ItemCount;
    }
}

// --- 2. Notification with Concurrent Parallel Strategy ---

/// <summary>Represents a notification event published when real-time inventory levels change.</summary>
/// <param name="ProductId">The unique identifier of the product.</param>
/// <param name="NewStock">The updated stock quantity.</param>
[PublishStrategy(PublishStrategy.Parallel)]
public sealed record RealTimeInventoryUpdatedEvent(string ProductId, int NewStock) : INotification;

/// <summary>Handles search index updates in response to <see cref="RealTimeInventoryUpdatedEvent"/>.</summary>
public sealed class InventoryIndexNotificationHandler : INotificationHandler<RealTimeInventoryUpdatedEvent>
{
    /// <inheritdoc/>
    public async ValueTask Handle(RealTimeInventoryUpdatedEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[Level 5 - Parallel Handler A] Updating search index for {notification.ProductId}...");
        await Task.Delay(40, cancellationToken).ConfigureAwait(false);
        Console.WriteLine($"[Level 5 - Parallel Handler A] Index updated to stock={notification.NewStock}.");
    }
}

/// <summary>Handles cache invalidation in response to <see cref="RealTimeInventoryUpdatedEvent"/>.</summary>
public sealed class InventoryCacheNotificationHandler : INotificationHandler<RealTimeInventoryUpdatedEvent>
{
    /// <inheritdoc/>
    public async ValueTask Handle(RealTimeInventoryUpdatedEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[Level 5 - Parallel Handler B] Invalidating distributed cache for {notification.ProductId}...");
        await Task.Delay(40, cancellationToken).ConfigureAwait(false);
        Console.WriteLine($"[Level 5 - Parallel Handler B] Cache invalidated.");
    }
}

/// <summary>Handles dashboard updates in response to <see cref="RealTimeInventoryUpdatedEvent"/>.</summary>
public sealed class InventoryDashboardNotificationHandler : INotificationHandler<RealTimeInventoryUpdatedEvent>
{
    /// <inheritdoc/>
    public async ValueTask Handle(RealTimeInventoryUpdatedEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[Level 5 - Parallel Handler C] Notifying real-time monitoring dashboard...");
        await Task.Delay(40, cancellationToken).ConfigureAwait(false);
        Console.WriteLine($"[Level 5 - Parallel Handler C] Dashboard updated.");
    }
}

/// <summary>Demonstrates concurrent processing and publishing strategies.</summary>
public static class Demo
{
    /// <summary>Executes the Level 5 concurrent processing demonstration.</summary>
    /// <param name="mediator">The mediator instance to use for dispatching.</param>
    /// <returns>A task representing the asynchronous demonstration operation.</returns>
    public static async Task RunAsync(IMediator mediator)
    {
        Console.WriteLine("================================================================================");
        Console.WriteLine("  LEVEL 5: ASYNCHRONOUS PROCESSING & CONCURRENT STRATEGIES");
        Console.WriteLine("================================================================================");

        // 1. Asynchronous Command
        Console.WriteLine("1. Non-blocking Asynchronous Processing:");
        var batchCmd = new ProcessBatchDataCommand("BATCH-2026-X", 500);
        var processedCount = await mediator.Send(batchCmd, CancellationToken.None);
        Console.WriteLine($"   -> Total items processed: {processedCount}");
        Console.WriteLine();

        // 2. Parallel Concurrent Publishing with Task.WhenAll
        Console.WriteLine("2. Parallel Publishing with [PublishStrategy(PublishStrategy.Parallel)]:");
        var sw = Stopwatch.StartNew();
        var parallelEvent = new RealTimeInventoryUpdatedEvent("PRD-SERVER-99", 42);
        await mediator.Publish(parallelEvent, CancellationToken.None);
        sw.Stop();
        Console.WriteLine($"   -> All concurrent handlers completed in {sw.ElapsedMilliseconds} ms (parallel execution).");

        Console.WriteLine("--------------------------------------------------------------------------------\n");
    }
}
