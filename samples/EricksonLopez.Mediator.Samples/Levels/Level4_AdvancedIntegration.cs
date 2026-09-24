// Copyright © Erickson Lopez. MIT License.
using System;
using System.Threading;
using System.Threading.Tasks;
using EricksonLopez.Mediator;
using EricksonLopez.Mediator.Result;
using EricksonLopez.Result;

namespace Sample.Levels.Level4_AdvancedIntegration;

/// <summary>
/// Represents receipt details for an order submission.
/// </summary>
public sealed record OrderSubmissionResult(Guid OrderId, string TrackingNumber);

/// <summary>
/// Provides pipeline validation with result pattern short-circuiting.
/// </summary>
public sealed class OrderValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IResultFactory<TResponse>? _resultFactory;

    public OrderValidationBehavior(IResultFactory<TResponse>? resultFactory = null)
    {
        _resultFactory = resultFactory;
    }

    /// <inheritdoc/>
    public ValueTask<TResponse> Handle<TNext>(TRequest request, TNext next, CancellationToken cancellationToken)
        where TNext : struct, INext<TResponse>
    {
        if (request is SubmitOrderCommand cmd)
        {
            // Business rule short-circuit (discontinued item)
            if (cmd.Sku.StartsWith("DISCONTINUED", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"[Level 4 - Pipeline] Short-circuit: SKU '{cmd.Sku}' is discontinued. Returning failure via IResultFactory.");
                if (_resultFactory is not null)
                {
                    var error = Error.Failure("Order.DiscontinuedProduct", $"Product SKU '{cmd.Sku}' is discontinued.");
                    return new ValueTask<TResponse>(_resultFactory.CreateFailure(error));
                }
            }
        }

        return next.InvokeAsync();
    }
}

/// <summary>
/// Represents an order submission command decorated with declarative validation rules.
/// </summary>
[UseBehavior(typeof(OrderValidationBehavior<,>))]
[ValidateRequest]
public sealed record SubmitOrderCommand(
    [property: ValidateNotEmpty] string Sku,
    [property: ValidateRange(1, 1000)] int Quantity,
    [property: ValidateLength(3, 50)] string CustomerName) : ICommand<Result<OrderSubmissionResult>>;

/// <summary>
/// Processes <see cref="SubmitOrderCommand"/> and yields an order submission result.
/// </summary>
public sealed class SubmitOrderCommandHandler : ICommandHandler<SubmitOrderCommand, Result<OrderSubmissionResult>>
{
    /// <inheritdoc/>
    public ValueTask<Result<OrderSubmissionResult>> Handle(SubmitOrderCommand command, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[Level 4 - Handler] Order processed successfully for '{command.CustomerName}' (SKU: {command.Sku}, Qty: {command.Quantity})");
        var result = new OrderSubmissionResult(Guid.NewGuid(), $"TRK-{Guid.NewGuid().ToString()[..6].ToUpperInvariant()}");
        return ValueTask.FromResult(Result<OrderSubmissionResult>.Success(result));
    }
}

/// <summary>
/// Demonstrates advanced integration with result patterns, declarative validation, and MediatR compatibility.
/// </summary>
public static class Demo
{
    /// <summary>
    /// Runs the advanced integration demonstration.
    /// </summary>
    /// <param name="mediator">The mediator instance used for dispatching messages.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task RunAsync(IMediator mediator)
    {
        Console.WriteLine("================================================================================");
        Console.WriteLine("  LEVEL 4: ADVANCED INTEGRATION (RESULT PATTERN & DECLARATIVE VALIDATION)");
        Console.WriteLine("================================================================================");

        // 1. Valid Request
        Console.WriteLine("1. Execution with Valid Data (Successful Return):");
        var validCmd = new SubmitOrderCommand("SKU-LAPTOP-01", 2, "Alice Enterprise Corp");
        var successRes = await mediator.Send(validCmd, CancellationToken.None);
        Console.WriteLine($"   -> IsSuccess: {successRes.IsSuccess}, Tracking: {successRes.Value?.TrackingNumber}");
        Console.WriteLine();

        // 2. Business Rule Short-Circuiting with IResultFactory (No exceptions thrown)
        Console.WriteLine("2. Business Rule Pipeline Short-Circuiting via IResultFactory (No Exceptions):");
        var businessFailCmd = new SubmitOrderCommand("DISCONTINUED-MODEL-X", 1, "Alice Enterprise Corp");
        var businessFailRes = await mediator.Send(businessFailCmd, CancellationToken.None);
        Console.WriteLine($"   -> IsSuccess: {businessFailRes.IsSuccess}");
        if (businessFailRes.IsFailure)
        {
            Console.WriteLine($"   -> Error Code: {businessFailRes.Error.Code}");
            Console.WriteLine($"   -> Error Description: {businessFailRes.Error.Description}");
        }
        Console.WriteLine();

        // 3. Declarative Compile-Time Validation ([ValidateRequest] / [ValidateRange])
        Console.WriteLine("3. Compile-Time Generated Validation ([ValidateRequest] / [ValidateRange]):");
        try
        {
            var invalidRangeCmd = new SubmitOrderCommand("SKU-LAPTOP-01", -10, "Alice Enterprise Corp");
            await mediator.Send(invalidRangeCmd, CancellationToken.None);
        }
        catch (MediatorValidationException valEx)
        {
            Console.WriteLine($"   -> Caught MediatorValidationException: {valEx.Message}");
            Console.WriteLine($"   -> Error Count: {valEx.Errors.Count}");
        }
        Console.WriteLine();

        // 4. MediatR Migration Compatibility (IRequest<T>, IRequest, IRequestHandler & Unit)
        Console.WriteLine("4. MediatR Migration Compatibility (IRequest<T>, IRequest, IRequestHandler & Unit):");
        var legacyUserRes = await mediator.Send(new LegacyCreateUserRequest("charlie_migrated"), CancellationToken.None);
        Console.WriteLine($"   -> Result from IRequest<string>: {legacyUserRes}");

        var pingRes = await mediator.Send(new LegacyPingRequest(), CancellationToken.None);
        Console.WriteLine($"   -> Result from IRequest (Unit): {pingRes}");
        Console.WriteLine();

        // 5. Unit Type Invariants & Operators
        Console.WriteLine("5. Unit Type Invariants & Operators:");
        var u1 = Unit.Value;
        var u2 = new Unit();
        Console.WriteLine($"   -> Unit.Value == new Unit(): {u1 == u2}");
        Console.WriteLine($"   -> Unit.Value != new Unit(): {u1 != u2}");
        Console.WriteLine($"   -> Unit.Value <= new Unit(): {u1 <= u2}");
        Console.WriteLine($"   -> Unit.Value.ToString(): '{u1}'");
        Console.WriteLine($"   -> Unit.CompareTo: {u1.CompareTo(u2)}");

        Console.WriteLine("--------------------------------------------------------------------------------\n");
    }
}

// --- 3. MediatR Migration Compatibility Contracts & Handlers ---

/// <summary>Represents a request contract implementing MediatR-compatible <see cref="IRequest{TResponse}"/>.</summary>
public sealed record LegacyCreateUserRequest(string Username) : IRequest<string>;

/// <summary>Processes <see cref="LegacyCreateUserRequest"/> via <see cref="IRequestHandler{TRequest, TResponse}"/>.</summary>
public sealed class LegacyCreateUserRequestHandler : IRequestHandler<LegacyCreateUserRequest, string>
{
    /// <inheritdoc/>
    public ValueTask<string> Handle(LegacyCreateUserRequest request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[Level 4 - Legacy Handler] Creating user '{request.Username}' via IRequestHandler<T, R>");
        return ValueTask.FromResult($"LEGACY_USR_{request.Username.ToUpperInvariant()}");
    }
}

/// <summary>Represents a parameterless request contract implementing MediatR-compatible <see cref="IRequest"/>.</summary>
public sealed record LegacyPingRequest() : IRequest;

/// <summary>Processes <see cref="LegacyPingRequest"/> via <see cref="IRequestHandler{TRequest}"/>.</summary>
public sealed class LegacyPingRequestHandler : IRequestHandler<LegacyPingRequest>
{
    /// <inheritdoc/>
    public ValueTask<Unit> Handle(LegacyPingRequest request, CancellationToken cancellationToken)
    {
        Console.WriteLine("[Level 4 - Legacy Handler] Pong received via IRequestHandler<T> (returning Unit)");
        return ValueTask.FromResult(Unit.Value);
    }
}

