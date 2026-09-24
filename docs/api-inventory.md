# Complete Public API Surface Inventory — EricksonLopez.Mediator

This document provides the exhaustive, verified, and normative inventory of the public API surface of `EricksonLopez.Mediator` and its infrastructure packages. It serves as the authoritative source of technical truth for repository documentation and the living Showcase project.

---

## 1. Core Library (`EricksonLopez.Mediator`)

| Name | Namespace | Responsibility | Dependencies | Use Cases | Complexity | Existing Example |
|---|---|---|---|---|---|:---:|
| `IMediator` | `EricksonLopez.Mediator` | Unified contract combining `ISender` and `IPublisher` for dispatch and publishing through a single access point. | `ISender`, `IPublisher` | Dependency injection into controllers, Minimal APIs, or application services. | Basic | Yes (Showcase Levels 1–8) |
| `ISender` | `EricksonLopez.Mediator` | Segregated contract for point-to-point dispatch of commands, queries, and reactive streams. | `ICommand`, `IQuery`, `IStreamRequest` | Services that strictly dispatch requests without publishing events (ISP). | Basic | Yes (Showcase Levels 1, 3) |
| `IPublisher` | `EricksonLopez.Mediator` | Segregated contract for 1-to-N broadcasting of domain events and notifications. | `INotification` | Decoupled event dissemination across multiple independent subscribers. | Basic | Yes (Showcase Levels 3, 5, 6, 10) |
| `ICommand<out TResponse>` | `EricksonLopez.Mediator` | Marker interface representing state-mutation operations in CQRS architectures. | None | Creating, updating, deleting, or executing transactional business operations. | Basic | Yes (Showcase Levels 1–11) |
| `ICommandHandler<in TCommand, TResponse>` | `EricksonLopez.Mediator` | Processing contract for a specific command returning `ValueTask<TResponse>`. | `ICommand<TResponse>`, `ValueTask` | Encapsulating command business logic and persistence. | Basic | Yes (Showcase Levels 1–11) |
| `IQuery<out TResponse>` | `EricksonLopez.Mediator` | Marker interface for side-effect-free, idempotent read queries. | None | Fetching, projecting, and reading data for UI or API consumption. | Basic | Yes (Showcase Levels 3, 9, 10, 11) |
| `IQueryHandler<in TQuery, TResponse>` | `EricksonLopez.Mediator` | Processing contract for a specific query returning `ValueTask<TResponse>`. | `IQuery<TResponse>`, `ValueTask` | High-efficiency data retrieval without change-tracking overhead. | Basic | Yes (Showcase Levels 3, 9, 10, 11) |
| `IStreamRequest<out TResponse>` | `EricksonLopez.Mediator` | Marker interface for continuous asynchronous stream requests producing item sequences. | None | Reading large chunked datasets, telemetry streaming, live event streams. | Intermediate | Yes (Showcase Levels 2, 11) |
| `IStreamRequestHandler<in TRequest, out TResponse>` | `EricksonLopez.Mediator` | Contract for handling streaming requests by emitting an `IAsyncEnumerable<TResponse>`. | `IStreamRequest<TResponse>`, `IAsyncEnumerable` | On-demand asynchronous sequence generation without massive in-memory buffering. | Intermediate | Yes (Showcase Levels 2, 11) |
| `INotification` | `EricksonLopez.Mediator` | Marker interface for multi-subscriber domain events and notifications. | None | System notifications, eventual consistency, workflow triggers. | Basic | Yes (Showcase Levels 3, 5, 6, 10, 11) |
| `INotificationHandler<in TNotification>` | `EricksonLopez.Mediator` | Contract for individual subscribers of a domain event returning `ValueTask`. | `INotification`, `ValueTask` | Sending emails, audit logging, replica synchronization following an event. | Basic | Yes (Showcase Levels 3, 5, 6, 10, 11) |
| `IPipelineBehavior<TRequest, TResponse>` | `EricksonLopez.Mediator` | Interceptor middleware for the request execution pipeline using struct `INext<TResponse>`. | `INext<TResponse>`, `ValueTask` | Logging, validation, observability, retries, concurrency control. | Advanced | Yes (Showcase Levels 2, 4, 6, 7, 9, 11) |
| `INotificationBehavior<TNotification>` | `EricksonLopez.Mediator` | Interceptor middleware for the notification publishing pipeline using struct `INext`. | `INext`, `ValueTask` | Cross-cutting audit logging, event correlation, global metrics. | Advanced | Yes (Showcase Level 11) |
| `INext<TResponse>` | `EricksonLopez.Mediator` | Interface for zero-allocation struct continuations in request pipelines. | `ValueTask` | Invoking the next link in the request middleware chain. | Advanced | Yes (Showcase Levels 2, 4, 6, 7, 9, 11) |
| `INext` | `EricksonLopez.Mediator` | Interface for zero-allocation struct continuations in notification pipelines. | `ValueTask` | Invoking the next handler in notification publishing. | Advanced | Yes (Showcase Level 11) |
| `IRequest<TResponse>` | `EricksonLopez.Mediator` | MediatR compatibility interface inheriting from `ICommand<TResponse>`. | `ICommand<TResponse>` | Smooth gradual migration from legacy codebases depending on MediatR. | Intermediate | Yes (Showcase Level 4) |
| `IRequest` | `EricksonLopez.Mediator` | MediatR compatibility interface for void requests (`IRequest<Unit>`). | `IRequest<Unit>`, `Unit` | Migration of legacy void MediatR commands without altering contracts. | Intermediate | Yes (Showcase Level 4) |
| `IRequestHandler<in TRequest, TResponse>` | `EricksonLopez.Mediator` | Handler for `IRequest<TResponse>` compatible with MediatR. | `ValueTask` | Direct migration of MediatR handlers preserving existing signatures. | Intermediate | Yes (Showcase Level 4) |
| `IRequestHandler<in TRequest>` | `EricksonLopez.Mediator` | Handler for void `IRequest` queries/commands returning `Unit`. | `IRequestHandler<TRequest, Unit>`, `Unit` | Migration of void MediatR handlers without signature rewrites. | Intermediate | Yes (Showcase Level 4) |
| `Unit` | `EricksonLopez.Mediator` | Canonical readonly struct representing absence of value in generic return types. | `IEquatable`, `IComparable` | Representing successful operations with no return payload. | Intermediate | Yes (Showcase Level 4) |
| `StaticMediator` | `EricksonLopez.Mediator` | Ultra-low overhead static dispatcher operating without a dependency injection container. | `ConcurrentDictionary` | Serverless (AWS Lambda, Azure Functions), AOT microservices, IoT runtimes. | Advanced | Yes (Showcase Level 10) |
| `PublishStrategy` | `EricksonLopez.Mediator` | Enum defining notification dispatch strategies for `INotification`. | None | Selecting between `Sequential`, `Parallel`, and `SequentialAggregateExceptions`. | Intermediate | Yes (Showcase Levels 5, 6) |
| `PublishStrategyAttribute` | `EricksonLopez.Mediator` | Declarative attribute associating a publish strategy with an `INotification`. | `PublishStrategy` | Forcing concurrent execution or comprehensive exception collection on key events. | Intermediate | Yes (Showcase Levels 5, 6) |
| `HandlerLifetime` | `EricksonLopez.Mediator` | Enum configuring DI service lifetimes for handlers and pipeline behaviors. | None | Declaring handlers as `Singleton`, `Scoped`, or `Transient`. | Intermediate | Yes (Showcase Level 8) |
| `ServiceLifetimeAttribute` | `EricksonLopez.Mediator` | Attribute declaring the DI service lifetime of a handler or behavior. | `HandlerLifetime` | Handlers requiring scoped dependencies (DbContext) or singleton state. | Intermediate | Yes (Showcase Level 8) |
| `UseBehaviorAttribute` | `EricksonLopez.Mediator` | Attribute associating a specific pipeline behavior with a request with explicit ordering. | `Type` | Granular behavior application (selective rate limiting, per-request caching). | Advanced | Yes (Showcase Levels 2, 4, 6) |
| `UseGlobalBehaviorAttribute` | `EricksonLopez.Mediator` | Assembly-level attribute binding a global pipeline behavior system-wide. | `Type` | Cross-cutting logging, global authentication, unified telemetry. | Advanced | Yes (Showcase Level 2) |
| `DiscoverHandlersAttribute` | `EricksonLopez.Mediator` | Assembly-level attribute instructing the generator to scan external assemblies. | `Type` | Multi-layered architectures and external infrastructure assemblies. | Advanced | Yes (Showcase Level 8) |
| `ValidateRequestAttribute` | `EricksonLopez.Mediator` | Attribute triggering static compile-time validation emission via Roslyn. | None | High-throughput validation without reflection or runtime allocations. | Basic | Yes (Showcase Levels 2, 4) |
| `ValidateNotNullAttribute` | `EricksonLopez.Mediator` | Declarative constraint asserting that a reference property must not be null. | None | Validating reference integrity in commands and queries. | Basic | Yes (Showcase Level 2) |
| `ValidateNotEmptyAttribute` | `EricksonLopez.Mediator` | Declarative constraint asserting that string properties cannot be empty or whitespace. | None | Validating mandatory names, codes, and identifiers. | Basic | Yes (Showcase Level 2) |
| `ValidateLengthAttribute` | `EricksonLopez.Mediator` | Declarative constraint asserting minimum and maximum string length boundaries. | None | Validating passwords, descriptions, and bounded text fields. | Basic | Yes (Showcase Level 2) |
| `ValidateRangeAttribute` | `EricksonLopez.Mediator` | Declarative constraint asserting numeric boundaries (minimum and maximum). | None | Validating prices, stock quantities, age limits, percentages. | Basic | Yes (Showcase Levels 2, 4) |
| `ValidateRegexAttribute` | `EricksonLopez.Mediator` | Declarative constraint asserting match against a compiled regular expression. | None | Validating email formats, phone numbers, tracking numbers. | Basic | Yes (Showcase Level 2) |
| `MediatorValidationException` | `EricksonLopez.Mediator` | Exception thrown when one or more declarative validation constraints fail in the pipeline. | `Exception`, `IReadOnlyList` | Capturing and formatting Source Generator validation errors. | Basic | Yes (Showcase Level 2) |
| `NotificationHandlerAggregateException` | `EricksonLopez.Mediator` | Aggregate exception thrown when one or more notification handlers fail in aggregate mode. | `Exception`, `IReadOnlyList` | Event resilience: all handlers execute and all errors are captured. | Intermediate | Yes (Showcase Level 6) |
| `MediatorHealthCheck` | `EricksonLopez.Mediator.HealthChecks` | `IHealthCheck` implementation verifying dispatcher readiness in the hosting environment. | `IHealthCheck`, `ISender` | `/healthz` probes in Kubernetes, Docker, and cloud orchestrators. | Basic | Yes (Showcase Level 9) |
| `MediatorHealthCheckExtensions` | `EricksonLopez.Mediator.HealthChecks` | Extension methods registering `MediatorHealthCheck` in the DI container. | `IServiceCollection` | Fluent registration of health checks during host startup. | Basic | Yes (Showcase Level 9, Program.cs) |

---

## 2. Infrastructure & Extension Packages

### `EricksonLopez.Mediator.AspNetCore`
| Name | Namespace | Responsibility | Dependencies | Use Cases | Complexity | Existing Example |
|---|---|---|---|---|---|:---:|
| `MediatorEndpointRouteBuilderExtensions` | `EricksonLopez.Mediator.AspNetCore` | Extension methods on `IEndpointRouteBuilder` to map commands and queries as Minimal APIs. | `Microsoft.AspNetCore.Routing`, `ISender` | Exposing CQRS endpoints directly as HTTP POST/PUT/DELETE (`MapCommand`) and GET (`MapQuery`). | Basic | Yes (Showcase Level 9) |

### `EricksonLopez.Mediator.Caching`
| Name | Namespace | Responsibility | Dependencies | Use Cases | Complexity | Existing Example |
|---|---|---|---|---|---|:---:|
| `CacheableAttribute` | `EricksonLopez.Mediator.Caching` | Attribute defining cache retention duration for requests. | None | Declarative time-to-live (TTL) configuration per request. | Intermediate | Yes (Showcase Level 9) |
| `ICacheableRequest` | `EricksonLopez.Mediator.Caching` | Contract for requests whose responses are cached transparently via `CacheKey` and `Expiration`. | None | Catalog queries, configuration data, and high-frequency read scenarios. | Intermediate | Yes (Showcase Level 9) |
| `IInvalidateCacheRequest` | `EricksonLopez.Mediator.Caching` | Contract for commands invalidating cache entries by prefix upon successful completion. | None | Data mutations invalidating lists or aggregate records in distributed cache. | Intermediate | Yes (Showcase Level 9) |
| `CachingPipelineBehavior<TRequest, TResponse>` | `EricksonLopez.Mediator.Caching` | Pipeline behavior managing response caching, hit/miss resolution, and prefix invalidation. | `IPipelineBehavior`, `ICacheProvider` | Transparent read acceleration with `EricksonLopez.Result` support. | Advanced | Yes (Showcase Level 9) |
| `CachingMediatorExtensions` | `EricksonLopez.Mediator.Caching` | Extension method `AddMediatorCaching` registering the caching pipeline behavior into DI. | `IServiceCollection` | Caching pipeline configuration in `Program.cs`. | Basic | Yes (Showcase Level 9) |

### `EricksonLopez.Mediator.FluentValidation`
| Name | Namespace | Responsibility | Dependencies | Use Cases | Complexity | Existing Example |
|---|---|---|---|---|---|:---:|
| `ValidationPipelineBehavior<TRequest, TResponse>` | `EricksonLopez.Mediator.FluentValidation` | Pipeline behavior executing FluentValidation validators concurrently with `IResultFactory` support. | `IPipelineBehavior`, `IValidator<T>`, `IResultFactory` | Complex enterprise validation before executing commands or queries. | Advanced | Yes (Showcase Level 9) |
| `MediatorFluentValidationExtensions` | `Microsoft.Extensions.DependencyInjection` | Extension methods registering validation behaviors and validators in AOT or assembly-scanning mode. | `IServiceCollection`, `IValidator<T>` | FluentValidation registration with or without runtime reflection (AOT-safe). | Intermediate | Yes (Showcase Level 9) |

### `EricksonLopez.Mediator.OpenTelemetry`
| Name | Namespace | Responsibility | Dependencies | Use Cases | Complexity | Existing Example |
|---|---|---|---|---|---|:---:|
| `OpenTelemetryBehavior<TRequest, TResponse>` | `EricksonLopez.Mediator.OpenTelemetry` | Pipeline behavior creating distributed tracing activities and recording metrics with zero overhead when unlistened. | `IPipelineBehavior`, `ActivitySource`, `Meter` | Distributed tracing and metrics integration with OpenTelemetry, Jaeger, and Prometheus. | Advanced | Yes (Showcase Level 9) |
| `MediatorOpenTelemetryOptions` | `EricksonLopez.Mediator.OpenTelemetry` | Options class configuring `ActivitySource` naming and trace enrichment callbacks. | `Activity` | Attaching contextual telemetry tags (tenant, environment, user) to every mediator trace. | Intermediate | Yes (Showcase Level 9) |
| `MediatorMetrics` | `EricksonLopez.Mediator.OpenTelemetry` | Standard BCL metrics (`System.Diagnostics.Metrics.Meter`) for throughput, failures, and execution latency. | `Meter` | Exporting dispatch metrics to Prometheus, Datadog, or Grafana Agent. | Advanced | Yes (Showcase Level 9) |
| `ServiceCollectionExtensions` | `EricksonLopez.Mediator.OpenTelemetry` | Extension method `AddMediatorOpenTelemetry` registering instrumentation into DI. | `IServiceCollection` | Fluent observability setup in `Program.cs`. | Basic | Yes (Showcase Level 9) |

### `EricksonLopez.Mediator.Polly` *(Deprecated per ADR-036)*
| Name | Namespace | Responsibility | Dependencies | Use Cases | Complexity | Existing Example |
|---|---|---|---|---|---|:---:|
| `UseResiliencePipelineAttribute` | `EricksonLopez.Mediator.Polly` | Attribute declaring which Polly v8 resilience pipeline applies to a request. | None | Retry, circuit breaker, or timeout policies for network operations. | Intermediate | Yes (Showcase Level 9) |
| `PollyResilienceBehavior<TRequest, TResponse>` | `EricksonLopez.Mediator.Polly` | Pipeline behavior executing requests within a configured Polly `ResiliencePipeline`. | `IPipelineBehavior`, `Polly.Core` | Wrapping request dispatch in resilience strategies for transient failures. | Advanced | Yes (Showcase Level 9) |
| `MediatorPollyExtensions` | `EricksonLopez.Mediator.Polly` | Extension methods `AddMediatorPolly` and `AddMediatorDefaultResiliencePipeline`. | `IServiceCollection`, `Polly.Core` | Legacy resilience policy configuration in DI. | Intermediate | Yes (Showcase Level 9) |

### `EricksonLopez.Mediator.RateLimiting`
| Name | Namespace | Responsibility | Dependencies | Use Cases | Complexity | Existing Example |
|---|---|---|---|---|---|:---:|
| `RateLimitingBehavior<TRequest, TResponse>` | `EricksonLopez.Mediator.RateLimiting` | Pipeline behavior enforcing throughput limits via BCL `RateLimiter`. | `IPipelineBehavior`, `RateLimiter` | Overload protection, tenant quota enforcement, and maximum concurrency control. | Advanced | Yes (Showcase Levels 6, 7) |
| `RateLimitExceededException` | `EricksonLopez.Mediator.RateLimiting` | Exception thrown when a request is rejected for exceeding rate limits, providing `RetryAfter`. | `Exception`, `TimeSpan` | Notifying HTTP callers or middleware of wait times before retrying. | Basic | Yes (Showcase Levels 6, 7) |
| `RateLimitingMediatorExtensions` | `EricksonLopez.Mediator.RateLimiting` | Extension method `AddMediatorRateLimiting` registering the behavior into DI. | `IServiceCollection` | Rate limiting configuration in `Program.cs`. | Basic | Yes (Showcase Levels 6, 7, Program.cs) |

### `EricksonLopez.Mediator.Result`
| Name | Namespace | Responsibility | Dependencies | Use Cases | Complexity | Existing Example |
|---|---|---|---|---|---|:---:|
| `IResultFactory<out TResponse>` | `EricksonLopez.Mediator.Result` | Factory interface for constructing typed failure responses (`Result<T>`) without exceptions or reflection. | `EricksonLopez.Result.Error` | Validation and business rule short-circuiting in Native AOT pipelines. | Advanced | Yes (Showcase Level 4) |

### `EricksonLopez.Mediator.Testing`
| Name | Namespace | Responsibility | Dependencies | Use Cases | Complexity | Existing Example |
|---|---|---|---|---|---|:---:|
| `FakeMediator` | `EricksonLopez.Mediator.Testing` | In-memory test double for `IMediator`, `ISender`, and `IPublisher` free from dynamic mock generation. | `IMediator` | Unit testing application services in reflection-free AOT environments. | Intermediate | Yes (Showcase Levels 9, 11) |
| `DelegateNext<TResponse>` | `EricksonLopez.Mediator.Testing` | Struct continuation for executing and evaluating `IPipelineBehavior` synchronously or asynchronously in unit tests. | `INext<TResponse>` | Isolated unit testing of pipeline behaviors without instantiating DI containers. | Intermediate | Yes (Showcase Levels 7, 9, 11) |
| `DelegateNext` | `EricksonLopez.Mediator.Testing` | Struct continuation for executing and evaluating `INotificationBehavior` in unit tests. | `INext` | Isolated unit testing of notification behaviors without event bus infrastructure. | Intermediate | Yes (Showcase Level 11) |
| `FakeAssertionException` | `EricksonLopez.Mediator.Testing` | Descriptive exception thrown when `ShouldHaveReceived` test assertions fail. | `Exception` | Interaction verification and traceability in unit tests. | Basic | Yes (Showcase Level 11) |

### Source Generator (`EricksonLopez.Mediator.Generator` - Emitted in Consumer Project)
| Name | Namespace | Responsibility | Dependencies | Use Cases | Complexity | Existing Example |
|---|---|---|---|---|---|:---:|
| `AddEricksonLopezMediator` | `Microsoft.Extensions.DependencyInjection` | Roslyn-emitted extension method registering the static dispatcher and discovered handlers into DI. | `IServiceCollection`, `ServiceLifetime` | Centralized library bootstrap in `Program.cs`. | Basic | Yes (Showcase Program.cs, Level 1) |
