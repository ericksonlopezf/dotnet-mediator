# Cookbook — Practical Recipes for EricksonLopez.Mediator

Exhaustive collection of architectural recipes and integration patterns for the `EricksonLopez.Mediator` ecosystem. Every recipe addresses a concrete technical requirement derived strictly from the library's public API surface.

---

## Recipe Index

1. [Validation with Result Pattern and IResultFactory](#recipe-1-validation-with-result-pattern-and-iresultfactory)
2. [Transparent Caching and Prefix Invalidation](#recipe-2-transparent-caching-and-prefix-invalidation)
3. [Enterprise Validation with FluentValidation](#recipe-3-enterprise-validation-with-fluentvalidation)
4. [Throughput Control with Rate Limiting](#recipe-4-throughput-control-with-rate-limiting)
5. [Distributed Tracing with OpenTelemetry](#recipe-5-distributed-tracing-with-opentelemetry)
6. [Concurrent Event Publishing in Parallel](#recipe-6-concurrent-event-publishing-in-parallel)
7. [Exception Aggregation in Notification Publishing](#recipe-7-exception-aggregation-in-notification-publishing)
8. [Boilerplate-Free Minimal APIs (MapCommand and MapQuery)](#recipe-8-boilerplate-free-minimal-apis-mapcommand-and-mapquery)
9. [Reactive Asynchronous Streaming with IStreamRequest](#recipe-9-reactive-asynchronous-streaming-with-istreamrequest)
10. [AOT Unit Testing without Dynamic Mocks using FakeMediator](#recipe-10-aot-unit-testing-without-dynamic-mocks-using-fakemediator)
11. [Zero-DI Serverless Acceleration with StaticMediator](#recipe-11-zero-di-serverless-acceleration-with-staticmediator)
12. [Kubernetes Health Probes with MediatorHealthCheck](#recipe-12-kubernetes-health-probes-with-mediatorhealthcheck)
13. [Gradual Migration from MediatR (IRequest and Unit)](#recipe-13-gradual-migration-from-mediatr-irequest-and-unit)
14. [Isolated Pipeline Behavior Testing with DelegateNext](#recipe-14-isolated-pipeline-behavior-testing-with-delegatenext)
15. [Polly v8 Resilience and Architectural Transition](#recipe-15-polly-v8-resilience-and-architectural-transition)

---

## Recipe 1: Validation with Result Pattern and IResultFactory

### Problem
Validate business domain rules within the mediator pipeline and return a typed `Result<T>` containing error codes without throwing slow exceptions or breaking Native AOT trimming compatibility.

### Solution
Use `IResultFactory<TResponse>` (from `EricksonLopez.Mediator.Result`) inside an `IPipelineBehavior<TRequest, TResponse>` to short-circuit the pipeline and return a failure response directly.

### Complete Code
```csharp
using System.Threading;
using System.Threading.Tasks;
using EricksonLopez.Mediator;
using EricksonLopez.Mediator.Result;
using EricksonLopez.Result;

public sealed record RegisterCustomerCommand(string Email, string Name) : ICommand<Result<Guid>>;

public sealed class CustomerValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IResultFactory<TResponse>? _resultFactory;

    public CustomerValidationBehavior(IResultFactory<TResponse>? resultFactory = null)
    {
        _resultFactory = resultFactory;
    }

    public ValueTask<TResponse> Handle<TNext>(TRequest request, TNext next, CancellationToken cancellationToken)
        where TNext : struct, INext<TResponse>
    {
        if (request is RegisterCustomerCommand cmd && !cmd.Email.Contains('@'))
        {
            if (_resultFactory is not null)
            {
                var error = Error.Validation("Customer.InvalidEmail", "The provided email is not in a valid format.");
                return new ValueTask<TResponse>(_resultFactory.CreateFailure(error));
            }
        }

        return next.InvokeAsync();
    }
}
```

### Explanation
The Roslyn Source Generator detects handler return types wrapped in `Result<T>` and automatically registers an `IResultFactory<TResponse>` implementation. When the behavior identifies an invalid request, it synthesizes the typed failure response without reflection and without allocating heap closures.

### Best Practices
- Inject `IResultFactory<TResponse>?` as an optional constructor dependency defaulting to `null`.
- Use semantic error codes (`Domain.ErrorDetail`).

### Common Pitfalls
- Performing an explicit cast like `(TResponse)(object)Result<T>.Failure(...)`, which causes boxing and fails under Native AOT.

---

## Recipe 2: Transparent Caching and Prefix Invalidation

### Problem
Accelerate high-throughput read-heavy queries via response caching and guarantee automated cache invalidation when a command modifies the underlying dataset.

### Solution
Implement `ICacheableRequest` and apply `[Cacheable]` to the query, implement `IInvalidateCacheRequest` on mutating commands, and register `CachingPipelineBehavior`.

### Complete Code
```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using EricksonLopez.Mediator;
using EricksonLopez.Mediator.Caching;
using Microsoft.Extensions.DependencyInjection;

// 1. DI Registration in Program.cs
services.AddMediatorCaching();

// 2. Cacheable Query
[Cacheable(300)]
public sealed record GetProductByIdQuery(string ProductId) : IQuery<ProductDto>, ICacheableRequest
{
    public string CacheKey => $"products:{ProductId}";
    public TimeSpan? Expiration => TimeSpan.FromMinutes(5);
}

// 3. Invalidating Command
public sealed record UpdateProductPriceCommand(string ProductId, decimal NewPrice) : ICommand<bool>, IInvalidateCacheRequest
{
    public string CachePrefix => "products:";
}
```

### Explanation
`CachingPipelineBehavior` intercepts the query; if present in `ICacheProvider`, the cached value is returned immediately without invoking downstream pipeline handlers. When `UpdateProductPriceCommand` executes successfully, the behavior calls `RemoveByPrefixAsync("products:")` to purge stale entries.

### Best Practices
- Set conservative time-to-live (TTL) limits paired with targeted prefix invalidation.
- Use colon-delimited hierarchical namespaces (`domain:entity:id`) for `CacheKey`.

### Common Pitfalls
- Defining an overly broad or overly narrow `CachePrefix`, causing either cache underutilization or stale orphaned entries.

---

## Recipe 3: Enterprise Validation with FluentValidation

### Problem
Validate complex incoming models using `FluentValidation` in a Native AOT application without performing dynamic runtime assembly scanning.

### Solution
Use `AddMediatorFluentValidationValidator<TValidator, TRequest>()` to register strongly-typed validators explicitly, processed through `ValidationPipelineBehavior`.

### Complete Code
```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using EricksonLopez.Mediator;
using EricksonLopez.Mediator.FluentValidation;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

// AOT-Safe DI Registration in Program.cs
services.AddMediatorFluentValidation();
services.AddMediatorFluentValidationValidator<CreateInvoiceValidator, CreateInvoiceCommand>();

public sealed record CreateInvoiceCommand(string CustomerId, decimal Amount) : ICommand<Guid>;

public sealed class CreateInvoiceValidator : AbstractValidator<CreateInvoiceCommand>
{
    public CreateInvoiceValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty().WithMessage("Customer ID is required.");
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount must be greater than zero.");
    }
}
```

### Explanation
`ValidationPipelineBehavior` resolves all registered `IValidator<TRequest>` instances, executes `ValidateAsync` concurrently via `Task.WhenAll`, and throws `ValidationException` or returns a synthesized `Result.Failure` when `IResultFactory` is present.

### Best Practices
- In Native AOT applications, avoid `AddMediatorFluentValidatorsFromAssembly` and prefer explicit registrations.

### Common Pitfalls
- Using runtime reflection scanners in projects compiled with `PublishAot=true`, which generates `IL2026` trimming warnings and crashes at runtime.

---

## Recipe 4: Throughput Control with Rate Limiting

### Problem
Prevent service degradation and protect constrained downstream dependencies by throttling request throughput per handler or endpoint.

### Solution
Register `AddMediatorRateLimiting` alongside a standard BCL `RateLimiter` (`System.Threading.RateLimiting`) and bind `RateLimitingBehavior`.

### Complete Code
```csharp
using System;
using System.Threading.RateLimiting;
using EricksonLopez.Mediator;
using EricksonLopez.Mediator.RateLimiting;
using Microsoft.Extensions.DependencyInjection;

services.AddMediatorRateLimiting();
services.AddSingleton<RateLimiter>(new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions
{
    TokenLimit = 20,
    TokensPerPeriod = 10,
    ReplenishmentPeriod = TimeSpan.FromSeconds(1),
    QueueLimit = 0
}));

// Command selectively protected by behavior
[UseBehavior(typeof(RateLimitingBehavior<,>), order: 1)]
public sealed record ExportAuditReportCommand(string TenantId) : ICommand<byte[]>;
```

### Explanation
Before delegating to the handler, `RateLimitingBehavior` attempts to acquire a lease via `AcquireAsync(1)`. If the rate quota is exhausted, it throws `RateLimitExceededException` populated with the optional `RetryAfter` duration.

### Best Practices
- Use `[UseBehavior]` to throttle specific expensive commands rather than throttling the entire mediator pipeline indiscriminately.

### Common Pitfalls
- Leaving `QueueLimit` unbounded, which can cause severe memory accumulation during denial-of-service traffic spikes.

---

## Recipe 5: Distributed Tracing with OpenTelemetry

### Problem
Instrument all mediator operations with W3C distributed tracing spans and execution latency metrics without incurring measurable overhead when no collectors are active.

### Solution
Register `AddMediatorOpenTelemetry()` during host startup and configure exporter sinks via the OpenTelemetry SDK.

### Complete Code
```csharp
using EricksonLopez.Mediator.OpenTelemetry;
using Microsoft.Extensions.DependencyInjection;

services.AddMediatorOpenTelemetry(options =>
{
    options.ActivitySourceName = "EnterpriseService.Mediator";
    options.EnrichActivity = (activity, request) =>
    {
        activity.SetTag("messaging.system", "ericksonlopez.mediator");
    };
});
```

### Explanation
`OpenTelemetryBehavior` starts an `Internal` `Activity`. If no listeners are attached, overhead is negligible. When completed, it records execution duration in the `mediator.request.duration` histogram and tracks success and failure counters using the standard BCL `Meter`.

### Best Practices
- Configure `EnrichActivity` callbacks to attach contextual correlation tags (tenant identifier, environment, user ID).

### Common Pitfalls
- Attaching sensitive credentials or PII into telemetry activity tags.

---

## Recipe 6: Concurrent Event Publishing in Parallel

### Problem
Publish a domain event to multiple independent subscribers without letting cumulative sequential latency degrade application throughput.

### Solution
Decorate the `INotification` with `[PublishStrategy(PublishStrategy.Parallel)]`.

### Complete Code
```csharp
using System.Threading;
using System.Threading.Tasks;
using EricksonLopez.Mediator;

[PublishStrategy(PublishStrategy.Parallel)]
public sealed record OrderPaidEvent(Guid OrderId, decimal Amount) : INotification;

public sealed class SendReceiptHandler : INotificationHandler<OrderPaidEvent>
{
    public async ValueTask Handle(OrderPaidEvent notification, CancellationToken ct) =>
        await Task.Delay(50, ct); // Asynchronous I/O operation
}

public sealed class NotifyWarehouseHandler : INotificationHandler<OrderPaidEvent>
{
    public async ValueTask Handle(OrderPaidEvent notification, CancellationToken ct) =>
        await Task.Delay(50, ct); // Asynchronous I/O operation
}
```

### Explanation
The dispatcher checks the notification attribute and runs all subscribers concurrently via `Task.WhenAll`, bounding total execution latency to the slowest individual handler rather than their sum.

### Best Practices
- Use this strategy when subscribers are strictly decoupled and do not share mutable state or local database transactions.

### Common Pitfalls
- Using `PublishStrategy.Parallel` with handlers that inject the same EF Core `DbContext` instance, triggering invalid concurrent access errors.

---

## Recipe 7: Exception Aggregation in Notification Publishing

### Problem
When publishing critical audit or security events, all subscribers must execute even if one throws an exception, collecting all failures into a single aggregate error.

### Solution
Decorate the `INotification` with `[PublishStrategy(PublishStrategy.SequentialAggregateExceptions)]`.

### Complete Code
```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using EricksonLopez.Mediator;

[PublishStrategy(PublishStrategy.SequentialAggregateExceptions)]
public sealed record SecurityBreachDetectedEvent(string IpAddress) : INotification;

// Caller invocation
try
{
    await publisher.Publish(new SecurityBreachDetectedEvent("192.168.1.100"));
}
catch (NotificationHandlerAggregateException aggEx)
{
    foreach (var error in aggEx.HandlerExceptions)
    {
        Console.WriteLine($"Subscriber error: {error.Message}");
    }
}
```

### Explanation
The dispatcher wraps each subscriber invocation in a `try/catch` block. Any encountered exceptions are collected into an internal list, and after all subscribers complete, a `NotificationHandlerAggregateException` is thrown containing all collected failures.

### Best Practices
- Use for security audits, synchronization events, and compliance triggers where partial execution must be avoided.

### Common Pitfalls
- Catching generic `Exception` instead of `NotificationHandlerAggregateException`, obscuring inner subscriber failure details.

---

## Recipe 8: Boilerplate-Free Minimal APIs (MapCommand and MapQuery)

### Problem
Expose CQRS commands directly as HTTP POST or PUT endpoints and queries as HTTP GET endpoints without writing manual controller classes or boilerplate glue code.

### Solution
Use `MapCommand` and `MapQuery` extension methods from `EricksonLopez.Mediator.AspNetCore`.

### Complete Code
```csharp
using EricksonLopez.Mediator.AspNetCore;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEricksonLopezMediator();
var app = builder.Build();

// Default HTTP POST mapping
app.MapCommand<CreateProductCommand, ProductResponse>("/api/products");

// Explicit HTTP PUT mapping
app.MapCommand<UpdateProductCommand, bool>("/api/products/{id}", "PUT");

// HTTP GET mapping with automatic query-string and route binding
app.MapQuery<GetProductByIdQuery, ProductDto>("/api/products/{id}");

app.Run();
```

### Explanation
`MapCommand` and `MapQuery` register Minimal API endpoints that resolve `ISender` from the request HTTP scope, invoke `SendCommand` or `SendQuery`, and automatically return `Results.Ok(response)`.

### Best Practices
- Keep command and query contracts flat to enable seamless automatic model binding in ASP.NET Core.

### Common Pitfalls
- Configuring route parameters whose names do not match the corresponding property names on the request record.

---

## Recipe 9: Reactive Asynchronous Streaming with IStreamRequest

### Problem
Stream millions of data records from a database to consumers with constant $O(1)$ memory consumption without buffering full datasets in memory.

### Solution
Implement `IStreamRequest<TResponse>` and consume via `ISender.CreateStream`.

### Complete Code
```csharp
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using EricksonLopez.Mediator;

public sealed record StreamTelemetryQuery(string SensorId) : IStreamRequest<double>;

public sealed class StreamTelemetryHandler : IStreamRequestHandler<StreamTelemetryQuery, double>
{
    public async IAsyncEnumerable<double> Handle(
        StreamTelemetryQuery request,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        for (int i = 0; i < 1000; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return 20.0 + (i * 0.1);
        }
    }
}

// Consumer loop
await foreach (var reading in sender.CreateStream(new StreamTelemetryQuery("SENSOR-01"), cancellationToken))
{
    Console.WriteLine($"Reading received: {reading}");
}
```

### Explanation
The `CreateStream` method routes requests through the compile-time dispatch table and connects the `IAsyncEnumerable<TResponse>` stream emitted by the handler directly to the caller via `yield return`.

### Best Practices
- Always annotate the handler token with `[EnumeratorCancellation]` to ensure downstream cancellation breaks early.

### Common Pitfalls
- Calling `.ToListAsync()` on the returned stream, which defeats the constant memory advantage of reactive streaming.

---

## Recipe 10: AOT Unit Testing without Dynamic Mocks using FakeMediator

### Problem
Write fast, Native AOT-compatible unit tests for application services that consume `IMediator` without relying on dynamic proxy mocking libraries like Moq or Castle Core.

### Solution
Use the official `FakeMediator` test double from `EricksonLopez.Mediator.Testing`.

### Complete Code
```csharp
using System.Threading.Tasks;
using EricksonLopez.Mediator.Testing;
using Xunit;

public class OrderServiceTests
{
    [Fact]
    public async Task ProcessOrder_Should_Dispatch_Command()
    {
        // 1. Arrange
        var fake = new FakeMediator();
        fake.SetupCommand<PlaceOrderCommand, OrderReceipt>(cmd => new OrderReceipt(cmd.OrderId, "COMPLETED"));
        var service = new OrderService(fake);

        // 2. Act
        var receipt = await service.CheckoutAsync(new OrderDto("ORD-99"));

        // 3. Assert
        fake.ShouldHaveReceived<PlaceOrderCommand>(c => c.OrderId == "ORD-99");
        Assert.Equal(1, fake.ReceivedCount<PlaceOrderCommand>());
    }
}
```

### Explanation
`FakeMediator` is an in-memory test double that operates without dynamic proxies. It records all dispatched requests and published notifications into thread-safe queues and provides fluent assertions (`ShouldHaveReceived`, `ShouldNotHaveReceived`).

### Best Practices
- Call `fake.Reset()` between test cases if reusing the test double instance.

### Common Pitfalls
- Using Moq or NSubstitute on `IMediator` in projects targeting Native AOT, triggering trimming warnings.

---

## Recipe 11: Zero-DI Serverless Acceleration with StaticMediator

### Problem
Execute CQRS commands in serverless functions (AWS Lambda, Azure Functions, Cloudflare Workers) with cold-start requirements under 5 milliseconds without initializing an `IServiceCollection` container.

### Solution
Configure and dispatch handlers directly using `StaticMediator`.

### Complete Code
```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using EricksonLopez.Mediator;

public static class ServerlessFunction
{
    static ServerlessFunction()
    {
        // One-time static handler registration during cold-start
        StaticMediator.RegisterCommandHandler(new FastOrderCommandHandler());
    }

    public static async Task<string> FunctionHandler(string orderPayload)
    {
        return await StaticMediator.SendCommand<FastOrderCommand, string>(
            new FastOrderCommand(orderPayload), CancellationToken.None);
    }
}
```

### Explanation
`StaticMediator` indexes handlers into thread-safe `ConcurrentDictionary` tables and executes strongly-typed dispatch without instantiating DI containers or allocating DI scopes.

### Best Practices
- Prefer `SendCommand` and `SendQuery` over polymorphic overloads to avoid runtime type checking.

### Common Pitfalls
- Injecting scoped dependencies into static handlers without manually managing their operational lifecycles.

---

## Recipe 12: Kubernetes Health Probes with MediatorHealthCheck

### Problem
Expose the readiness and operational status of the mediator dispatch pipeline for Kubernetes liveness and readiness probes.

### Solution
Register `AddMediatorHealthCheck()` into the ASP.NET Core Health Checks infrastructure.

### Complete Code
```csharp
using EricksonLopez.Mediator.HealthChecks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEricksonLopezMediator();
builder.Services.AddMediatorHealthCheck();
builder.Services.AddHealthChecks();

var app = builder.Build();
app.MapHealthChecks("/healthz");
app.Run();
```

### Explanation
`MediatorHealthCheck` verifies that `ISender` is registered and ready to handle dispatches. If the dispatcher is missing or uninitialized, it reports `HealthStatus.Degraded` to orchestrator probes.

### Best Practices
- Expose the `/healthz` endpoint protected and monitored by ingress controllers.

### Common Pitfalls
- Registering the health check before invoking `AddEricksonLopezMediator()`, resulting in continuous Degraded reports.

---

## Recipe 13: Gradual Migration from MediatR (IRequest and Unit)

### Problem
Migrate an existing legacy codebase containing hundreds of classes implementing MediatR's `IRequest<T>` and `IRequestHandler<T, R>` without rewriting all contracts in the initial sprint.

### Solution
Use the compatibility interfaces `IRequest<TResponse>`, `IRequest`, `IRequestHandler`, and `Unit` provided directly in the core package.

### Complete Code
```csharp
using System.Threading;
using System.Threading.Tasks;
using EricksonLopez.Mediator;

// Request returning a value
public sealed record GetUserStatsRequest(string UserId) : IRequest<UserStatsDto>;

public sealed class GetUserStatsHandler : IRequestHandler<GetUserStatsRequest, UserStatsDto>
{
    public ValueTask<UserStatsDto> Handle(GetUserStatsRequest request, CancellationToken cancellationToken) =>
        ValueTask.FromResult(new UserStatsDto(42, 100));
}

// Void request (yields Unit)
public sealed record PurgeLogsRequest() : IRequest;

public sealed class PurgeLogsHandler : IRequestHandler<PurgeLogsRequest>
{
    public ValueTask<Unit> Handle(PurgeLogsRequest request, CancellationToken cancellationToken)
    {
        // Purge logic
        return ValueTask.FromResult(Unit.Value);
    }
}
```

### Explanation
The Roslyn Source Generator detects `IRequestHandler` implementations and integrates them directly into the `GeneratedMediator` static switch table. New code can subsequently transition to `ICommand<T>` and `IQuery<T>`.

### Best Practices
- Plan a phased migration: first adopt the Source Generator with `IRequest`, then segregate commands and queries.

### Common Pitfalls
- Forgetting to return `Unit.Value` in handlers implementing `IRequestHandler<TRequest>`.

---

## Recipe 14: Isolated Pipeline Behavior Testing with DelegateNext

### Problem
Write unit tests for an `IPipelineBehavior<TRequest, TResponse>` or `INotificationBehavior<TNotification>` verifying interception logic without mocking interfaces or instantiating DI containers.

### Solution
Use the `DelegateNext<TResponse>` and `DelegateNext` struct continuations from `EricksonLopez.Mediator.Testing`.

### Complete Code
```csharp
using System.Threading;
using System.Threading.Tasks;
using EricksonLopez.Mediator.Testing;
using Xunit;

public class PerformanceBehaviorTests
{
    [Fact]
    public async Task Behavior_Should_Intercept_And_Return_Value()
    {
        // Arrange
        var behavior = new GlobalPerformanceBehavior<SampleRequest, string>();
        var fakeContinuation = new DelegateNext<string>(() => ValueTask.FromResult("PROCESSED_SUCCESSFULLY"));

        // Act
        var result = await behavior.Handle(new SampleRequest(), fakeContinuation, CancellationToken.None);

        // Assert
        Assert.Equal("PROCESSED_SUCCESSFULLY", result);
    }
}
```

### Explanation
`DelegateNext<TResponse>` implements `INext<TResponse>` accepting a `Func<ValueTask<TResponse>>` or a direct constant value, enabling instant simulation of downstream continuations.

### Best Practices
- Use the constant value constructor `new DelegateNext<T>(constantValue)` for ultra-fast synchronous test evaluation.

### Common Pitfalls
- Creating custom class-based implementations of `INext<T>`, losing the zero-allocation struct performance invariant.

---

## Recipe 15: Polly v8 Resilience and Architectural Transition

### Problem
Protect commands interacting with unstable external services using retry policies, while adhering to the Clean Architecture guidelines documented in ADR-036.

### Solution
Apply `[UseResiliencePipeline]` with `PollyResilienceBehavior` in legacy projects while planning the architectural transition to `EricksonLopez.Resilience.Mediator`.

### Complete Code
```csharp
using System;
using EricksonLopez.Mediator;
using EricksonLopez.Mediator.Polly;
using Microsoft.Extensions.DependencyInjection;
using Polly;

#pragma warning disable CS0618
services.AddMediatorDefaultResiliencePipeline(builder =>
{
    builder.AddRetry(new Polly.Retry.RetryStrategyOptions
    {
        MaxRetryAttempts = 3,
        Delay = TimeSpan.FromMilliseconds(100)
    });
});

[UseResiliencePipeline("Default")]
public sealed record SyncCustomerDataCommand(string CustomerId) : ICommand<bool>;
#pragma warning restore CS0618
```

### Explanation
`PollyResilienceBehavior` resolves the named Polly v8 pipeline and wraps handler execution. Per ADR-036, this direct dependency in the application layer is deprecated in favor of the decoupled architecture in `EricksonLopez.Resilience.Mediator`.

### Best Practices
- In new architectures, isolate resilience strategies within the infrastructure layer.

### Common Pitfalls
- Coupling heavy external third-party resilience libraries directly into domain contracts rather than using assembly-configured interceptors.
