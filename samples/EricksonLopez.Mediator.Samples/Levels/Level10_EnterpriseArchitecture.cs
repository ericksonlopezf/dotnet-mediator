// Copyright © Erickson Lopez. MIT License.
using System;
using System.Threading;
using System.Threading.Tasks;
using EricksonLopez.Mediator;

namespace Sample.Levels.Level10_EnterpriseArchitecture;

// --- 1. Domain & Microservice Contracts ---

/// <summary>Represents a command executed within a serverless runtime environment.</summary>
/// <param name="Payload">The payload string to process.</param>
public sealed record ServerlessProcessCommand(string Payload) : ICommand<string>;

/// <summary>Handles serverless execution for <see cref="ServerlessProcessCommand"/>.</summary>
public sealed class ServerlessProcessCommandHandler : ICommandHandler<ServerlessProcessCommand, string>
{
    /// <inheritdoc/>
    public ValueTask<string> Handle(ServerlessProcessCommand command, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult($"PROCESSED: {command.Payload.ToUpperInvariant()}");
    }
}

/// <summary>Represents a query to check service availability in a serverless environment.</summary>
/// <param name="ServiceKey">The unique service identifier to query.</param>
public sealed record ServerlessStatusQuery(string ServiceKey) : IQuery<bool>;

/// <summary>Handles availability queries for <see cref="ServerlessStatusQuery"/>.</summary>
public sealed class ServerlessStatusQueryHandler : IQueryHandler<ServerlessStatusQuery, bool>
{
    /// <inheritdoc/>
    public ValueTask<bool> Handle(ServerlessStatusQuery query, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(true);
    }
}

// --- 2. Multi-Layer Enterprise Event ---

/// <summary>Represents a domain event across an enterprise architecture.</summary>
/// <param name="AggregateId">The unique identifier of the source aggregate.</param>
/// <param name="EventType">The name or classification of the domain event.</param>
public sealed record EnterpriseDomainEvent(Guid AggregateId, string EventType) : INotification;

/// <summary>Handles persistence of <see cref="EnterpriseDomainEvent"/> to a transactional outbox.</summary>
public sealed class OutboxPatternNotificationHandler : INotificationHandler<EnterpriseDomainEvent>
{
    /// <inheritdoc/>
    public ValueTask Handle(EnterpriseDomainEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[Level 10 - Outbox] Persisting event '{notification.EventType}' to Transactional Outbox table (ID: {notification.AggregateId})");
        return ValueTask.CompletedTask;
    }
}

/// <summary>Handles relaying of <see cref="EnterpriseDomainEvent"/> to a distributed message bus.</summary>
public sealed class KafkaMessageRelayNotificationHandler : INotificationHandler<EnterpriseDomainEvent>
{
    /// <inheritdoc/>
    public ValueTask Handle(EnterpriseDomainEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[Level 10 - Broker] Relaying event '{notification.EventType}' to distributed message bus");
        return ValueTask.CompletedTask;
    }
}

/// <summary>
/// Demonstrates enterprise architectural patterns including StaticMediator and transactional outbox eventing.
/// </summary>
public static class Demo
{
    /// <summary>Executes the Level 10 enterprise architecture demonstration.</summary>
    /// <param name="mediator">The mediator instance to use for dispatching.</param>
    /// <returns>A task representing the asynchronous demonstration operation.</returns>
    public static async Task RunAsync(IMediator mediator)
    {
        Console.WriteLine("================================================================================");
        Console.WriteLine("  LEVEL 10: ENTERPRISE ARCHITECTURE & STATICMEDIATOR (ZERO-DI)");
        Console.WriteLine("================================================================================");

        // 1. StaticMediator: Static dispatch without DI Container (AWS Lambda, Azure Functions, Cloudflare Workers, IoT)
        Console.WriteLine("1. Using StaticMediator for Ultra-Low Latency Serverless Environments (Zero-DI):");
        StaticMediator.Reset();

        // Manual static registration
        StaticMediator.RegisterCommandHandler(new ServerlessProcessCommandHandler());
        StaticMediator.RegisterQueryHandler(new ServerlessStatusQueryHandler());
        StaticMediator.RegisterNotificationHandler(new ServerlessEventAuditHandler());

        // Direct and strongly typed static invocation
        var cmdRes = await StaticMediator.SendCommand<ServerlessProcessCommand, string>(
            new ServerlessProcessCommand("cold-start-elimination"), CancellationToken.None);
        var qryRes = await StaticMediator.SendQuery<ServerlessStatusQuery, bool>(
            new ServerlessStatusQuery("PaymentGateway"), CancellationToken.None);

        // StaticMediator.Publish — notification fan-out without DI
        await StaticMediator.Publish(
            new ServerlessFunctionInvokedEvent("HandleOrder", DateTime.UtcNow), CancellationToken.None);

        Console.WriteLine($"   -> StaticMediator Command Result: {cmdRes}");
        Console.WriteLine($"   -> StaticMediator Query Result: {qryRes}");
        Console.WriteLine($"   -> StaticMediator.Publish dispatched ServerlessFunctionInvokedEvent");
        Console.WriteLine();

        // 2. Enterprise Event Architecture & Outbox Pattern
        Console.WriteLine("2. Multi-Layer Domain Event Orchestration (Clean Architecture & Outbox):");
        var aggregateId = Guid.NewGuid();
        var domainEvent = new EnterpriseDomainEvent(aggregateId, "AccountBalanceAdjusted");
        await mediator.Publish(domainEvent, CancellationToken.None);

        Console.WriteLine("--------------------------------------------------------------------------------\n");
    }
}

/// <summary>Represents a serverless domain event demonstrating static notification publishing.</summary>
/// <param name="FunctionName">The name of the invoked serverless function.</param>
/// <param name="InvokedAt">The UTC timestamp when the function was invoked.</param>
public sealed record ServerlessFunctionInvokedEvent(string FunctionName, DateTime InvokedAt) : INotification;

/// <summary>Handles audit logging for <see cref="ServerlessFunctionInvokedEvent"/> registered through <see cref="StaticMediator"/>.</summary>
public sealed class ServerlessEventAuditHandler : INotificationHandler<ServerlessFunctionInvokedEvent>
{
    /// <inheritdoc/>
    public ValueTask Handle(ServerlessFunctionInvokedEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"   [StaticMediator Notification] Function '{notification.FunctionName}' invoked at {notification.InvokedAt:HH:mm:ss}");
        return ValueTask.CompletedTask;
    }
}

