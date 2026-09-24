// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using EricksonLopez.Mediator;

// Assembly-level Global Behavior registration with explicit order priority.
// order: controls execution position among all global behaviors — lower values execute FIRST (outermost).
// Pipeline call order: order=0 wraps order=1 wraps order=2 … wraps the handler.
[assembly: UseGlobalBehavior(typeof(Sample.Levels.Level2_FullConfig.GlobalPerformanceBehavior<,>), order: 1)]

namespace Sample.Levels.Level2_FullConfig;

/// <summary>
/// Provides global pipeline behavior intercepting all requests with high-performance execution.
/// </summary>
public sealed class GlobalPerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    /// <inheritdoc/>
    public async ValueTask<TResponse> Handle<TNext>(TRequest request, TNext next, CancellationToken cancellationToken)
        where TNext : struct, INext<TResponse>
    {
        var reqName = typeof(TRequest).Name;
        // Avoid saturating console in high-throughput benchmarks or loops
        bool shouldLog = !reqName.Contains("HighThroughput");

        if (shouldLog)
        {
            Console.WriteLine($"[Global Pipeline (Order 1)] -> Intercepting {reqName}");
        }

        var response = await next.InvokeAsync().ConfigureAwait(false);

        if (shouldLog)
        {
            Console.WriteLine($"[Global Pipeline (Order 1)] <- Completed {reqName}");
        }

        return response;
    }
}

/// <summary>
/// Provides specific audit pipeline behavior applied to targeted requests.
/// </summary>
public sealed class SpecificAuditBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    /// <inheritdoc/>
    public async ValueTask<TResponse> Handle<TNext>(TRequest request, TNext next, CancellationToken cancellationToken)
        where TNext : struct, INext<TResponse>
    {
        Console.WriteLine($"[Specific Pipeline (Order 2)] -> Auditing {typeof(TRequest).Name} with struct INext<TResponse>");
        var response = await next.InvokeAsync().ConfigureAwait(false);
        Console.WriteLine($"[Specific Pipeline (Order 2)] <- Audit completed");
        return response;
    }
}

/// <summary>
/// Represents a payment command decorated with specific pipeline behavior.
/// </summary>
[UseBehavior(typeof(SpecificAuditBehavior<,>), order: 2)]
public sealed record ProcessPaymentCommand(string AccountId, decimal Amount) : ICommand<bool>;

/// <summary>
/// Processes <see cref="ProcessPaymentCommand"/> execution.
/// </summary>
public sealed class ProcessPaymentCommandHandler : ICommandHandler<ProcessPaymentCommand, bool>
{
    /// <inheritdoc/>
    public ValueTask<bool> Handle(ProcessPaymentCommand command, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[Level 2 - Handler] Processing payment of ${command.Amount} for account {command.AccountId}");
        return ValueTask.FromResult(true);
    }
}

/// <summary>
/// Represents a reactive asynchronous streaming request.
/// </summary>
public sealed record NumberStreamQuery(int Count, int DelayMs) : IStreamRequest<int>;

/// <summary>
/// Emits sequential order event items using an asynchronous stream.
/// </summary>
public sealed class NumberStreamQueryHandler : IStreamRequestHandler<NumberStreamQuery, int>
{
    /// <inheritdoc/>
    public async IAsyncEnumerable<int> Handle(
        NumberStreamQuery request,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        for (int i = 1; i <= request.Count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (request.DelayMs > 0)
            {
                await Task.Delay(request.DelayMs, cancellationToken);
            }
            yield return i * 10;
        }
    }
}

/// <summary>
/// Demonstrates pipeline configurations, custom behaviors, and streaming handlers.
/// </summary>
public static class Demo
{
    /// <summary>
    /// Runs the full configuration demonstration.
    /// </summary>
    /// <param name="mediator">The mediator instance used for dispatching messages.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task RunAsync(IMediator mediator)
    {
        Console.WriteLine("================================================================================");
        Console.WriteLine("  LEVEL 2: FULL CONFIGURATION (PIPELINES, BEHAVIORS & STREAMING)");
        Console.WriteLine("================================================================================");
        Console.WriteLine("1. Chained Pipeline Execution (Global Order 1 -> Specific Order 2 -> Handler):");
        var paymentCmd = new ProcessPaymentCommand("ACC-9842", 250.00m);
        var paymentSuccess = await mediator.Send(paymentCmd, CancellationToken.None);
        Console.WriteLine($"   -> Payment approved: {paymentSuccess}");
        Console.WriteLine();
        Console.WriteLine("2. Reactive Asynchronous Streaming via IStreamRequest<T> and CreateStream:");
        var streamQuery = new NumberStreamQuery(Count: 5, DelayMs: 20);
        Console.Write("   -> Stream items received: ");
        await foreach (var item in mediator.CreateStream(streamQuery, CancellationToken.None))
        {
            Console.Write($"[{item}] ");
        }
        Console.WriteLine();
        Console.WriteLine();

        // --- All 5 Compile-Time Validation Attributes ---
        Console.WriteLine("3. [ValidateNotNull] — null-guard attribute on request properties:");
        try
        {
            var nullCmd = new RegisterAccountCommand(null!, "alice", "password123", 25.0, "alice@example.com");
            await mediator.Send(nullCmd, CancellationToken.None);
        }
        catch (MediatorValidationException ex)
        {
            Console.WriteLine($"   -> MediatorValidationException caught: {ex.Message}");
            Console.WriteLine($"   -> Errors.Count: {ex.Errors.Count}");
        }
        Console.WriteLine();

        Console.WriteLine("4. [ValidateNotEmpty] — empty-guard attribute on string properties:");
        try
        {
            var emptyCmd = new RegisterAccountCommand("alice", "", "password123", 25.0, "alice@example.com");
            await mediator.Send(emptyCmd, CancellationToken.None);
        }
        catch (MediatorValidationException ex)
        {
            Console.WriteLine($"   -> MediatorValidationException caught: {ex.Message}");
        }
        Console.WriteLine();

        Console.WriteLine("5. [ValidateLength(8, 128)] — string length bounds attribute:");
        try
        {
            var lengthCmd = new RegisterAccountCommand("alice", "Alice Corp", "abc", 25.0, "alice@example.com");
            await mediator.Send(lengthCmd, CancellationToken.None);
        }
        catch (MediatorValidationException ex)
        {
            Console.WriteLine($"   -> MediatorValidationException caught: {ex.Message}");
        }
        Console.WriteLine();

        Console.WriteLine("6. [ValidateRange(18.0, 120.0)] — numeric range attribute:");
        try
        {
            var rangeCmd = new RegisterAccountCommand("alice", "Alice Corp", "secretpassword", 15.0, "alice@example.com");
            await mediator.Send(rangeCmd, CancellationToken.None);
        }
        catch (MediatorValidationException ex)
        {
            Console.WriteLine($"   -> MediatorValidationException caught: {ex.Message}");
        }
        Console.WriteLine();

        Console.WriteLine("7. [ValidateRegex] — pattern validation attribute on request properties:");
        try
        {
            var regexCmd = new RegisterAccountCommand("alice", "Alice Corp", "secretpassword", 25.0, "NOT_AN_EMAIL");
            await mediator.Send(regexCmd, CancellationToken.None);
        }
        catch (MediatorValidationException ex)
        {
            Console.WriteLine($"   -> MediatorValidationException caught: {ex.Message}");
        }
        Console.WriteLine();

        Console.WriteLine("8. MediatorValidationException(IReadOnlyList<string>) — multi-error constructor:");
        var errors = new List<string> { "Name is required.", "Email format is invalid.", "Password too short." };
        var multiError = new MediatorValidationException(errors);
        Console.WriteLine($"   -> Message: {multiError.Message}");
        Console.WriteLine($"   -> Errors.Count: {multiError.Errors.Count}");

        // Happy path — all validations pass
        Console.WriteLine("9. [ValidateRequest] success path — all constraints satisfied:");
        var validCmd = new RegisterAccountCommand("alice", "Alice Corp", "secureP@ss1", 28.0, "alice@example.com");
        var registered = await mediator.Send(validCmd, CancellationToken.None);
        Console.WriteLine($"   -> Registered: {registered}");

        Console.WriteLine("--------------------------------------------------------------------------------\n");
    }
}

// ─── [ValidateNotNull] + [ValidateRegex] + [ValidateNotEmpty] + [ValidateLength] + [ValidateRange] ───

/// <summary>
/// Demonstrates all compile-time validation attributes:
/// <see cref="ValidateNotNullAttribute"/>, <see cref="ValidateNotEmptyAttribute"/>,
/// <see cref="ValidateLengthAttribute"/>, <see cref="ValidateRangeAttribute"/>,
/// and <see cref="ValidateRegexAttribute"/>.
/// The Roslyn Source Generator emits all validation code — zero runtime reflection.
/// </summary>
[ValidateRequest]
public sealed record RegisterAccountCommand(
    [property: ValidateNotNull("Username must not be null.")]
    string Username,
    [property: ValidateNotEmpty("Username must not be empty.")]
    string DisplayName,
    [property: ValidateLength(8, 128, "Password must be 8-128 characters.")]
    string Password,
    [property: ValidateRange(18.0, 120.0, "Age must be between 18 and 120.")]
    double Age,
    [property: ValidateRegex(".+@.+", "A valid email address is required.")]
    string Email) : ICommand<bool>;

/// <summary>Processes <see cref="RegisterAccountCommand"/> validation and registration.</summary>
public sealed class RegisterAccountCommandHandler : ICommandHandler<RegisterAccountCommand, bool>
{
    /// <inheritdoc/>
    public ValueTask<bool> Handle(RegisterAccountCommand command, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[Level 2 - Handler] Account registered: {command.Username} / {command.Email}");
        return ValueTask.FromResult(true);
    }
}
