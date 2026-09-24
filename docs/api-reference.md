# API Reference — EricksonLopez.Mediator Ecosystem

Technical reference documentation structured according to **Microsoft Learn** standards for all public interfaces, classes, structs, attributes, enums, and extension methods in the `EricksonLopez.Mediator` ecosystem.

---

## 1. Dispatch Kernel (`EricksonLopez.Mediator`)

### `ISender`
Defines the contract for dispatching commands, queries, and continuous data streams to their respective handlers.

#### Methods

---

##### `ISender.SendCommand<TCommand, TResponse>`
Dispatches a strongly-typed command directly without runtime type-checks or boxing.

```csharp
ValueTask<TResponse> SendCommand<TCommand, TResponse>(
    TCommand command, 
    CancellationToken cancellationToken = default)
    where TCommand : ICommand<TResponse>;
```

- **Parameters**:
  - `command`: The command instance implementing `ICommand<TResponse>`. Cannot be `null`.
  - `cancellationToken`: Optional cancellation token to abort the operation.
- **Return**: `ValueTask<TResponse>` representing the asynchronous operation.
- **Exceptions**:
  - `ArgumentNullException`: Thrown when `command` is `null`.
  - `OperationCanceledException`: Thrown when the cancellation token is cancelled.
  - `MediatorValidationException`: Thrown when declarative validation constraints fail.
- **Remarks**: Recommended method in `EricksonLopez.Mediator` for CQRS state mutation operations. Bound via compile-time static dispatch emitted by Roslyn.
- **Basic Example**:
  ```csharp
  var result = await sender.SendCommand<CreateOrderCommand, Guid>(new CreateOrderCommand("P1"));
  ```
- **Advanced Example**:
  ```csharp
  using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
  var response = await sender.SendCommand<ProcessPaymentCommand, Result<PaymentReceipt>>(
      new ProcessPaymentCommand("ACC-1", 500m), cts.Token);
  ```
- **Best Practices**: Prefer `SendCommand` over the polymorphic `Send` overload to allow the compiler to infer and optimize the call site.
- **Performance**: 0 heap allocations when the pipeline uses struct `INext<TResponse>` and the handler returns a cached `ValueTask`.
- **Common Pitfalls**: Passing a query to `SendCommand`, which produces a compile error due to the `where TCommand : ICommand<TResponse>` constraint.
- **When to Use**: All CQRS write commands.
- **When NOT to Use**: Pure read operations (use `SendQuery` instead).

---

##### `ISender.SendQuery<TQuery, TResponse>`
Dispatches a strongly-typed query directly without runtime reflection.

```csharp
ValueTask<TResponse> SendQuery<TQuery, TResponse>(
    TQuery query, 
    CancellationToken cancellationToken = default)
    where TQuery : IQuery<TResponse>;
```

- **Parameters**:
  - `query`: The query instance implementing `IQuery<TResponse>`.
  - `cancellationToken`: Optional cancellation token.
- **Return**: `ValueTask<TResponse>` producing the query result.
- **Exceptions**:
  - `ArgumentNullException`: Thrown when `query` is `null`.
  - `OperationCanceledException`: Thrown when the token is cancelled.
- **Remarks**: Enforces CQRS semantics. Guarantees that only read-only handlers handle the request.
- **Basic Example**:
  ```csharp
  var user = await sender.SendQuery<GetUserByIdQuery, UserDto?>(new GetUserByIdQuery(userId));
  ```
- **Advanced Example**:
  ```csharp
  var catalog = await sender.SendQuery<GetProductCatalogQuery, IReadOnlyList<ProductDto>>(
      new GetProductCatalogQuery("Electronics"), cancellationToken);
  ```
- **Best Practices**: Keep query handlers free from write transactions and side-effect mutations.
- **Performance**: 100% compatible with EF Core read optimizations (`AsNoTracking`) and Dapper.
- **Common Pitfalls**: Modifying domain entities inside a query handler.
- **When to Use**: All read, search, or projection operations.
- **When NOT to Use**: Operations modifying state in persistent storage.

---

##### `ISender.CreateStream<TResponse>`
Dispatches a continuous streaming request and returns a reactive asynchronous enumerable.

```csharp
IAsyncEnumerable<TResponse> CreateStream<TResponse>(
    IStreamRequest<TResponse> request, 
    CancellationToken cancellationToken = default);
```

- **Parameters**:
  - `request`: Instance of the streaming request `IStreamRequest<TResponse>`.
  - `cancellationToken`: Cancellation token to abort the stream.
- **Return**: `IAsyncEnumerable<TResponse>` emitting items as they are produced.
- **Exceptions**:
  - `ArgumentNullException`: Thrown when `request` is `null`.
- **Remarks**: Consumed via `await foreach`. Does not accumulate all elements in memory simultaneously.
- **Basic Example**:
  ```csharp
  await foreach (var item in sender.CreateStream(new StreamLogsRequest(DateTime.UtcNow.AddHours(-1))))
  {
      Console.WriteLine(item);
  }
  ```
- **Advanced Example**:
  ```csharp
  using var cts = new CancellationTokenSource();
  await foreach (var batch in sender.CreateStream(new ExportLargeDatasetQuery("Sales2026"), cts.Token))
  {
      await streamWriter.WriteAsync(batch);
  }
  ```
- **Best Practices**: Manage cancellation inside the consumer loop to terminate connections promptly.
- **Performance**: Constant $O(1)$ memory consumption regardless of the total item count transmitted.
- **Common Pitfalls**: Calling `.ToList()` or `.ToArray()` on the stream, blocking threads and exhausting heap memory.
- **When to Use**: High-volume reporting, continuous telemetry, data export.
- **When NOT to Use**: Single-record queries or small paginated datasets.

---

##### `ISender.Send<TResponse>(ICommand<TResponse>)` and `ISender.Send<TResponse>(IQuery<TResponse>)`
Polymorphic dispatch overloads for compatibility with generic frameworks or decoupled interfaces.

```csharp
ValueTask<TResponse> Send<TResponse>(
    ICommand<TResponse> command, 
    CancellationToken cancellationToken = default);

ValueTask<TResponse> Send<TResponse>(
    IQuery<TResponse> query, 
    CancellationToken cancellationToken = default);
```

- **Parameters**: The polymorphic command or query instance and the cancellation token.
- **Return**: `ValueTask<TResponse>` containing the response.
- **Remarks**: Allows dispatching instances when the variable is typed as `ICommand<T>` or `IQuery<T>` rather than the concrete type.
- **When to Use**: In generic controllers or proxy mediators where the concrete type is not known at compile time.
- **When NOT to Use**: In regular code where the concrete type is available (prefer `SendCommand` or `SendQuery`).

---

### `IPublisher`
Defines the contract for publishing domain events and 1-to-N notifications.

#### Methods

##### `IPublisher.Publish<TNotification>`
Publishes a notification to all registered `INotificationHandler<TNotification>` instances.

```csharp
ValueTask Publish<TNotification>(
    TNotification notification, 
    CancellationToken cancellationToken = default)
    where TNotification : INotification;
```

- **Parameters**:
  - `notification`: The event instance implementing `INotification`.
  - `cancellationToken`: Optional cancellation token.
- **Return**: `ValueTask` that completes when all subscribers have finished according to the configured strategy.
- **Exceptions**:
  - `ArgumentNullException`: Thrown when `notification` is `null`.
  - `NotificationHandlerAggregateException`: Thrown when configured with `SequentialAggregateExceptions` and one or more subscribers fail.
- **Remarks**: If decorated with `[PublishStrategy]`, adopts the specified policy (`Sequential`, `Parallel`, `SequentialAggregateExceptions`).
- **Basic Example**:
  ```csharp
  await publisher.Publish(new OrderCompletedEvent(orderId));
  ```
- **Advanced Example**:
  ```csharp
  await publisher.Publish(new TenantProvisionedEvent(tenantId, DateTime.UtcNow), cancellationToken);
  ```
- **Best Practices**: Subscribers should remain decoupled and avoid assuming execution order unless strictly necessary.
- **Performance**: Zero heap allocations during iteration in `Sequential` mode.
- **Common Pitfalls**: Expecting an error in a subscriber to roll back the database transaction of the original command without using an Outbox pattern.
- **When to Use**: Notifying secondary side-effects following successful state mutations.
- **When NOT to Use**: For RPC orchestration or requests that require an immediate synchronous return payload.

---

### `Unit`
Canonical readonly struct representing the absence of a return value in generic signatures.

```csharp
namespace EricksonLopez.Mediator;

public readonly struct Unit : IEquatable<Unit>, IComparable<Unit>, IComparable
{
    public static ref readonly Unit Value { get; }
    public int CompareTo(Unit other);
    public bool Equals(Unit other);
    public override string ToString();
    // Operators: ==, !=, <, <=, >, >=
}
```

- **Properties**:
  - `Unit.Value`: `ref readonly` reference to the static default singleton value.
- **Remarks**: Designed to provide compatibility with the MediatR pattern and enable generic `IRequest<Unit>` signatures.
- **Basic Example**:
  ```csharp
  return ValueTask.FromResult(Unit.Value);
  ```
- **Operators Example**:
  ```csharp
  var u1 = Unit.Value;
  var u2 = new Unit();
  bool isEqual = (u1 == u2); // true
  string str = u1.ToString(); // "()"
  ```
- **Best Practices**: Use `Unit` primarily in MediatR migration scenarios. In new greenfield code, prefer `ICommand<bool>` or `ICommand<Result<T>>`.
- **Performance**: 0 bytes memory size. Does not produce boxing when returning `ValueTask<Unit>`.

---

### `StaticMediator`
Static dispatch engine for Native AOT and serverless environments without runtime dependency injection.

#### Methods

##### `RegisterCommandHandler<TCommand, TResponse>` / `RegisterQueryHandler<TQuery, TResponse>` / `RegisterNotificationHandler<TNotification>`
Registers handler instances directly into thread-safe concurrent tables in memory.

```csharp
public static void RegisterCommandHandler<TCommand, TResponse>(ICommandHandler<TCommand, TResponse> handler);
public static void RegisterQueryHandler<TQuery, TResponse>(IQueryHandler<TQuery, TResponse> handler);
public static void RegisterNotificationHandler<TNotification>(INotificationHandler<TNotification> handler);
```

##### `SendCommand<TCommand, TResponse>` / `SendQuery<TQuery, TResponse>` / `Publish<TNotification>`
Executes direct dispatch by resolving the invoker in the static table.

```csharp
public static ValueTask<TResponse> SendCommand<TCommand, TResponse>(TCommand command, CancellationToken ct = default);
public static ValueTask<TResponse> SendQuery<TQuery, TResponse>(TQuery query, CancellationToken ct = default);
public static ValueTask Publish<TNotification>(TNotification notification, CancellationToken ct = default);
```

##### `Reset()`
Clears all registration tables. Essential for test isolation in unit test suites and benchmarks.

```csharp
public static void Reset();
```

- **Basic Example**:
  ```csharp
  StaticMediator.Reset();
  StaticMediator.RegisterCommandHandler(new ProcessOrderCommandHandler());
  var result = await StaticMediator.SendCommand<ProcessOrderCommand, bool>(new ProcessOrderCommand());
  ```
- **Best Practices**: Use in isolated Azure Functions, AWS Lambda, embedded microservices, and performance tests where DI container instantiation incurs measurable overhead.
- **Performance**: Sub-microsecond dispatch with zero DI scope allocations.
- **Common Pitfalls**: Forgetting to call `Reset()` between unit tests, resulting in handler registration collision.

---

## 2. Declarative Attributes and Metadata

### `[PublishStrategyAttribute]`
Defines the concurrency and error tolerance policy for publishing a notification.

- **Signature**: `[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)] public sealed class PublishStrategyAttribute(PublishStrategy strategy)`
- **Available Strategies**:
  - `PublishStrategy.Sequential`: Sequential FIFO execution. If a subscriber throws, execution stops immediately.
  - `PublishStrategy.Parallel`: Concurrent execution via `Task.WhenAll`.
  - `PublishStrategy.SequentialAggregateExceptions`: Complete sequential execution; captures all exceptions and throws `NotificationHandlerAggregateException`.

### `[ServiceLifetimeAttribute]`
Overrides the default DI lifetime assigned to a handler or pipeline behavior.

- **Signature**: `public sealed class ServiceLifetimeAttribute(HandlerLifetime lifetime)`
- **Values**: `HandlerLifetime.Singleton`, `HandlerLifetime.Scoped`, `HandlerLifetime.Transient`.
- **Remarks**: Allows a handler to declare explicitly if it requires `Scoped` lifetime (e.g., when injecting a `DbContext`) or `Singleton` for memory caches or metrics.

### `[UseBehaviorAttribute]` / `[UseGlobalBehaviorAttribute]`
Binds middleware behaviors to request execution pipelines.

- **`[UseBehavior(typeof(TBehavior), order: N)]`**: Applies a behavior to a specific command or query.
- **`[assembly: UseGlobalBehavior(typeof(TBehavior), order: N)]`**: Applies a behavior globally across all requests in the assembly.

### Declarative Validation Attributes (`[ValidateRequest]` and Constraints)
Instruct the Roslyn Source Generator to synthesize static validation code at compile time.

- `[ValidateRequest]`: Enables static validation generation on the command or query.
- `[ValidateNotNull("message")]`: Asserts that a reference property cannot be null.
- `[ValidateNotEmpty("message")]`: Asserts that strings cannot be null, empty, or whitespace.
- `[ValidateLength(min, max, "message")]`: Asserts string length boundaries.
- `[ValidateRange(min, max, "message")]`: Asserts numeric boundaries (inclusive minimum and maximum).
- `[ValidateRegex("pattern", "message")]`: Asserts matching against a compiled regular expression.

---

## 3. Infrastructure and Extension Packages

### `EricksonLopez.Mediator.AspNetCore`

#### `MediatorEndpointRouteBuilderExtensions.MapCommand`
Maps a CQRS command directly as an ASP.NET Core Minimal API endpoint.

```csharp
public static RouteHandlerBuilder MapCommand<TCommand, TResponse>(
    this IEndpointRouteBuilder endpoints, 
    string pattern);

public static RouteHandlerBuilder MapCommand<TCommand, TResponse>(
    this IEndpointRouteBuilder endpoints, 
    string pattern, 
    string httpMethod);
```

- **Parameters**:
  - `endpoints`: ASP.NET Core route builder (`app`).
  - `pattern`: URL route pattern (e.g., `"/api/orders"`).
  - `httpMethod`: Explicit HTTP verb (defaults to `"POST"`; can specify `"PUT"`, `"DELETE"`, etc.).
- **Return**: `RouteHandlerBuilder` to chain authorization, endpoint filters, or OpenAPI metadata.
- **Example**:
  ```csharp
  app.MapCommand<CreateCustomerCommand, Guid>("/api/customers");
  app.MapCommand<UpdateCustomerCommand, bool>("/api/customers/{id}", "PUT");
  ```

#### `MediatorEndpointRouteBuilderExtensions.MapQuery`
Maps a CQRS query as an HTTP GET endpoint with automatic query-string and route parameter binding.

```csharp
public static RouteHandlerBuilder MapQuery<TQuery, TResponse>(
    this IEndpointRouteBuilder endpoints, 
    string pattern);
```

- **Example**:
  ```csharp
  app.MapQuery<GetCustomerByIdQuery, CustomerDto>("/api/customers/{id}");
  ```

---

### `EricksonLopez.Mediator.Caching`

#### `ICacheableRequest` and `[Cacheable]`
Contract and attribute for requests whose responses are transparently cached.

- `CacheKey`: Unique key identifying the cached response.
- `Expiration`: Optional absolute expiration duration.

#### `IInvalidateCacheRequest`
Contract for commands that invalidate cached entries by prefix upon successful completion.

- `CachePrefix`: Key prefix to purge in the configured cache provider.

#### `CachingPipelineBehavior<TRequest, TResponse>`
Pipeline behavior intercepting requests, evaluating cache hits/misses, and orchestrating prefix invalidation.

#### `CachingMediatorExtensions.AddMediatorCaching`
Registers `CachingPipelineBehavior` into the DI container:
```csharp
services.AddMediatorCaching();
```

---

### `EricksonLopez.Mediator.FluentValidation`

#### `ValidationPipelineBehavior<TRequest, TResponse>`
Executes all registered `IValidator<TRequest>` instances concurrently via `Task.WhenAll`. If validation failures occur and an `IResultFactory<TResponse>` is available, returns a typed failure result without throwing exceptions; otherwise throws `ValidationException`.

#### `MediatorFluentValidationExtensions`
- `services.AddMediatorFluentValidation()`: Registers the validation pipeline behavior into DI.
- `services.AddMediatorFluentValidationValidator<TValidator, TRequest>()`: Explicit AOT-safe validator registration.
- `services.AddMediatorFluentValidatorsFromAssembly(assembly)`: Dynamic assembly-scanning validator registration.

---

### `EricksonLopez.Mediator.OpenTelemetry`

#### `OpenTelemetryBehavior<TRequest, TResponse>`
Middleware starting an `Internal` `Activity`, propagating standard tags, and recording metrics and exceptions.

#### `MediatorMetrics`
Exposes standard BCL metrics based on `System.Diagnostics.Metrics.Meter`:
- `mediator.requests.total`: Total count of dispatched requests.
- `mediator.notifications.total`: Total count of published notifications.
- `mediator.requests.failures`: Total count of failed requests.
- `mediator.request.duration`: Histogram tracking request execution latency in milliseconds.

#### `ServiceCollectionExtensions.AddMediatorOpenTelemetry`
Registers OpenTelemetry instrumentation and `ActivitySource` into DI:
```csharp
services.AddMediatorOpenTelemetry(options =>
{
    options.ActivitySourceName = "MyApp.Mediator";
});
```

---

### `EricksonLopez.Mediator.Polly` *(Deprecated per ADR-036)*

#### `UseResiliencePipelineAttribute`
Attribute declaring which Polly v8 resilience pipeline applies to a request.

#### `PollyResilienceBehavior<TRequest, TResponse>`
Pipeline behavior executing requests within a configured Polly `ResiliencePipeline`.

#### `MediatorPollyExtensions`
Extension methods `AddMediatorPolly` and `AddMediatorDefaultResiliencePipeline` for legacy resilience DI configuration.

---

### `EricksonLopez.Mediator.RateLimiting`

#### `RateLimitingBehavior<TRequest, TResponse>`
Middleware acquiring leases from a DI-registered `RateLimiter` prior to handler execution. If rate limits are exceeded, throws `RateLimitExceededException`.

#### `RateLimitExceededException`
Typed exception providing an optional `RetryAfter` property for HTTP 429 response headers.

#### `RateLimitingMediatorExtensions.AddMediatorRateLimiting`
Registers the rate limiting behavior in DI:
```csharp
services.AddMediatorRateLimiting();
```

---

### `EricksonLopez.Mediator.Result`

#### `IResultFactory<out TResponse>`
Factory interface to construct typed failure responses (`Result<T>`) without runtime reflection.

```csharp
public interface IResultFactory<out TResponse>
{
    TResponse CreateFailure(Error error);
}
```

- **Remarks**: Roslyn automatically emits and registers concrete implementations for every `Result<T>` type used in commands or queries. Enables pipeline behaviors to return typed failure responses with zero boxing.

---

### `EricksonLopez.Mediator.Testing`

#### `FakeMediator`
In-memory test double free from runtime reflection or dynamic proxy generation.

##### Configuration Methods
- `SetupCommand<TCommand, TResponse>(Func<TCommand, TResponse> handler)`
- `SetupCommand<TCommand, TResponse>(Func<TCommand, CancellationToken, ValueTask<TResponse>> handler)`
- `SetupQuery<TQuery, TResponse>(Func<TQuery, TResponse> handler)`
- `SetupQuery<TQuery, TResponse>(Func<TQuery, CancellationToken, ValueTask<TResponse>> handler)`
- `SetupNotification<TNotification>(Func<TNotification, CancellationToken, ValueTask> handler)`
- `SetupStream<TRequest, TResponse>(Func<TRequest, CancellationToken, IAsyncEnumerable<TResponse>> handler)`

##### Assertion and History Methods
- `ShouldHaveReceived<T>()` / `ShouldHaveReceived<T>(Func<T, bool> predicate)`
- `ShouldNotHaveReceived<T>()` / `ShouldNotHaveReceived<T>(Func<T, bool> predicate)`
- `ReceivedCount<T>()`
- `ReceivedRequests` / `ReceivedNotifications`
- `Reset()`

#### `DelegateNext<TResponse>` and `DelegateNext`
Struct continuations for unit testing behaviors in isolation without instantiating DI containers:

```csharp
// Asynchronous continuation
var asyncNext = new DelegateNext<string>(() => ValueTask.FromResult("OK"));

// Synchronous constant continuation
var syncNext = new DelegateNext<string>("OK");

// For INotificationBehavior
var notifNext = new DelegateNext(() => ValueTask.CompletedTask);
```
