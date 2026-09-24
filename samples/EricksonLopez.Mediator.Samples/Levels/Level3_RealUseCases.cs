// Copyright © Erickson Lopez. MIT License.
using System;
using System.Threading;
using System.Threading.Tasks;
using EricksonLopez.Mediator;

namespace Sample.Levels.Level3_RealUseCases;

// --- Query Contract (Pure idempotent read) ---
/// <summary>
/// Represents a query to retrieve a user profile by unique identifier.
/// </summary>
public sealed record GetUserByIdQuery(Guid UserId) : IQuery<UserProfileDto?>;

/// <summary>
/// Represents a data transfer object containing user profile details.
/// </summary>
public sealed record UserProfileDto(Guid UserId, string FullName, string Email, string Role);

/// <summary>
/// Processes <see cref="GetUserByIdQuery"/> without mutating state.
/// </summary>
public sealed class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserProfileDto?>
{
    /// <inheritdoc/>
    public ValueTask<UserProfileDto?> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[Level 3 - QueryHandler] Querying user profile for {query.UserId}");
        var profile = new UserProfileDto(query.UserId, "Erickson Lopez", "erickson@domain.dev", "Software Architect");
        return ValueTask.FromResult<UserProfileDto?>(profile);
    }
}

/// <summary>
/// Represents a notification emitted when an order is placed.
/// </summary>
public sealed record OrderPlacedDomainEvent(Guid OrderId, decimal TotalAmount, string CustomerEmail) : INotification;

/// <summary>
/// Processes order placed events to send confirmation emails.
/// </summary>
public sealed class SendOrderConfirmationEmailHandler : INotificationHandler<OrderPlacedDomainEvent>
{
    /// <inheritdoc/>
    public ValueTask Handle(OrderPlacedDomainEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[Level 3 - NotificationHandler 1] Sending confirmation email to {notification.CustomerEmail} for ${notification.TotalAmount}");
        return ValueTask.CompletedTask;
    }
}

/// <summary>
/// Processes order placed events to record audit logs.
/// </summary>
public sealed class AuditOrderCreationHandler : INotificationHandler<OrderPlacedDomainEvent>
{
    /// <inheritdoc/>
    public ValueTask Handle(OrderPlacedDomainEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[Level 3 - NotificationHandler 2] Writing audit log: Order {notification.OrderId} created successfully");
        return ValueTask.CompletedTask;
    }
}

/// <summary>
/// Demonstrates CQRS segregation of queries, commands, and notifications.
/// </summary>
public static class Demo
{
    /// <summary>
    /// Runs the CQRS use cases demonstration.
    /// </summary>
    /// <param name="mediator">The mediator instance used for dispatching.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task RunAsync(IMediator mediator)
    {
        Console.WriteLine("================================================================================");
        Console.WriteLine("  LEVEL 3: REAL CQRS USE CASES (IQUERY & INOTIFICATION)");
        Console.WriteLine("================================================================================");

        // 1. Query execution via segregated ISender
        ISender sender = mediator;
        var userId = Guid.NewGuid();
        Console.WriteLine("1. Query execution with ISender.SendQuery:");
        var user = await sender.SendQuery<GetUserByIdQuery, UserProfileDto?>(new GetUserByIdQuery(userId), CancellationToken.None);
        Console.WriteLine($"   -> Retrieved user: {user?.FullName} ({user?.Role})");
        Console.WriteLine();

        // 2. Domain event publishing via segregated IPublisher
        IPublisher publisher = mediator;
        var orderId = Guid.NewGuid();
        Console.WriteLine("2. Multi-subscriber event publishing with IPublisher.Publish:");
        var domainEvent = new OrderPlacedDomainEvent(orderId, 1250.75m, "customer@enterprise.com");
        await publisher.Publish(domainEvent, CancellationToken.None);

        Console.WriteLine("--------------------------------------------------------------------------------\n");
    }
}
