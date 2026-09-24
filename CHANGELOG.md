# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [2.0.0] - 2026-09-24

### Added

- **`EricksonLopez.Mediator.Caching` (ADR-038):** Added dedicated package providing high-performance query caching and invalidation pipeline behaviors (`CachingPipelineBehavior<TRequest, TResponse>`, `CacheableAttribute`, `ICacheableRequest`, `IInvalidateCacheRequest`, `AddMediatorCaching()`). Features multi-tier caching and 100% Native AOT compatibility.
- **MediatR Compatibility Primitives:** Added optional migration contracts (`IRequest<TResponse>`, `IRequest`, `IRequestHandler<TRequest, TResponse>`, `IRequestHandler<TRequest>`, `Unit`) to enable seamless, incremental migration from legacy MediatR codebases.

### Deprecated

- **`EricksonLopez.Mediator.Polly` (ADR-036):** Marked as deprecated. All public types (`PollyResilienceBehavior<TRequest, TResponse>`, `UseResiliencePipelineAttribute`, `MediatorPollyExtensions`) are marked with `[Obsolete(error: false)]`. Package retained for backward compatibility until v2.0. Migration: replace with `EricksonLopez.Resilience.Mediator` from the `EricksonLopez.Resilience` ecosystem, which provides Clean Architecture-compliant resilience pipeline integration without leaking Polly types into the Application layer.
- **`StaticMediator.Send` (ADR-032):** Marked polymorphic `Send<TResponse>(ICommand<TResponse>, CancellationToken)` and `Send<TResponse>(IQuery<TResponse>, CancellationToken)` with `[Obsolete(error: false)]` in favor of type-safe, AOT-compatible `SendCommand<TCommand, TResponse>()` and `SendQuery<TQuery, TResponse>()`. Retained for backwards compatibility in v1.x; scheduled for removal in v2.0.

### Breaking Changes

- **Scoped Mediator by Default (BC-UNREL-001 / ADR-037):** Changed default service lifetime of `IMediator`, `ISender`, and `IPublisher` in `AddEricksonLopezMediator(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)` from `ServiceLifetime.Singleton` to `ServiceLifetime.Scoped`.
  - *Impact:* Resolving `IMediator`, `ISender`, or `IPublisher` from the root `IServiceProvider` (e.g. at startup in `Program.cs`) or from within a `Singleton` service (e.g. hosted services, background workers) will throw `InvalidOperationException` under ASP.NET Core default scope validation (`ValidateScopes=true`). In addition, the compiled binary signature changed from 1 parameter to 2 parameters with a default value.
  - *Migration:* For applications requiring a singleton mediator (e.g. CLI daemons or workers with purely stateless handlers), pass `ServiceLifetime.Singleton` explicitly: `services.AddEricksonLopezMediator(ServiceLifetime.Singleton);`. In ASP.NET Core, resolve `IMediator` from `HttpContext.RequestServices` or an active scope (`IServiceScopeFactory.CreateScope()`). Recompile projects referencing `AddEricksonLopezMediator`.
- **Sealed Source Generator Class (BC-UNREL-002):** Added `sealed` modifier to `MediatorSourceGenerator` (`public sealed class MediatorSourceGenerator : IIncrementalGenerator`).
  - *Impact:* Consumers, test suites, or tooling attempting to inherit from `MediatorSourceGenerator` will fail compilation with error `CS0509: cannot derive from sealed type`.
  - *Migration:* Use composition rather than inheritance. Direct subclassing of the incremental generator is unsupported.
- **Result Pattern Dependency Major Upgrade (BC-UNREL-003):** Upgraded `EricksonLopez.Result` and `EricksonLopez.Result.FluentValidation` package dependencies from `2.0.0` to `3.0.0` in `Directory.Packages.props`.
  - *Impact:* Consuming projects referencing `EricksonLopez.Mediator.Result` that directly depend on `EricksonLopez.Result` 2.x will encounter package downgrade/conflict warnings (NU1605) or breaking API changes from the `EricksonLopez.Result` 3.x major bump.
  - *Migration:* Upgrade consuming projects to `EricksonLopez.Result` 3.0.0 or higher.
- **DI Registration Idempotency with TryAdd (BC-UNREL-004):** Replaced `services.Add{Lifetime}<T>()` with `services.TryAdd{Lifetime}<T>()` in generated DI code for handlers, pipeline behaviors, and `IResultFactory<T>` registrations.
  - *Impact:* Handlers or behaviors registered prior to calling `AddEricksonLopezMediator()` will now take precedence, and subsequent generated registrations will be skipped rather than appending duplicate descriptors.
  - *Migration:* If your application relied on `services.Add...` registering multiple instances for multi-cast resolution of the same handler type, register them explicitly after `AddEricksonLopezMediator()`.
- **FakeMediator Pre-flight Cancellation and Null Validation (BC-UNREL-005):** All dispatch methods in `FakeMediator` (`Send`, `SendCommand`, `SendQuery`, `Publish`, `CreateStream`) now immediately invoke `cancellationToken.ThrowIfCancellationRequested()` and `ArgumentNullException.ThrowIfNull(request)`.
  - *Impact:* Test suites that pass pre-canceled `CancellationToken` instances or `null` requests to `FakeMediator` will now throw `OperationCanceledException` or `ArgumentNullException` instead of returning mock responses.
  - *Migration:* Ensure test setups pass valid, non-canceled tokens (`CancellationToken.None` or `default`) and non-null request instances. To test cancellation behavior, assert `OperationCanceledException`.
- **StaticMediator Pre-flight Cancellation Validation (BC-UNREL-006):** `StaticMediator.SendCommand`, `SendQuery`, and `Publish` now invoke `cancellationToken.ThrowIfCancellationRequested()` before attempting handler lookup.
  - *Impact:* Dispatches with a canceled token throw `OperationCanceledException` immediately, prior to checking whether a handler is registered.
  - *Migration:* Ensure callers pass active tokens. Expect `OperationCanceledException` when cancellation is signaled.
- **Deprecation of EricksonLopez.Mediator.Polly (BC-UNREL-007 / ADR-036):** All public types in `EricksonLopez.Mediator.Polly` (`PollyResilienceBehavior`, `UseResiliencePipelineAttribute`, `MediatorPollyExtensions`) are marked with `[Obsolete(error: false)]`.
  - *Impact:* Projects built with `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` will fail compilation with error CS0618.
  - *Migration:* Add `<NoWarn>$(NoWarn);CS0618</NoWarn>` to consuming project files as a temporary mitigation, and migrate to `EricksonLopez.Resilience.Mediator` from the `EricksonLopez.Resilience` ecosystem.

### Changed

- **`EricksonLopez.Mediator.Generator` (ADR-037):** Updated `AddEricksonLopezMediator(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)` to default to `ServiceLifetime.Scoped` instead of `Singleton` (superseding ADR-009). Eliminates captive dependencies when handlers inject scoped services (e.g., `DbContext` or multi-tenant context) and resolves scope validation failures under ASP.NET Core `ValidateScopes=true`. Consumers requiring singleton mediator (e.g., CLI daemons with purely stateless handlers) can pass `ServiceLifetime.Singleton` explicitly.
- **Showcase (`EricksonLopez.Mediator.Samples`):** Removed `EricksonLopez.Mediator.Polly` dependency from the reference Showcase. `Level6_ErrorHandling` now demonstrates `EricksonLopez.Mediator.RateLimiting` (`RateLimitingBehavior<TRequest, TResponse>`, `RateLimitExceededException`, `ConcurrencyLimiter` pre-saturation pattern) as the canonical non-deprecated resilience-adjacent behavior. `PublishStrategy.SequentialAggregateExceptions` demo retained.
- **`docs/showcase-specification.md`:** Updated `Mediator.Polly` row to reflect DEPRECATED status per ADR-036. Fixed `IStreamQuery<T>` type name references to correct public type `IStreamRequest<T>` and `IStreamRequestHandler` in all four occurrences throughout the document.

---

## [1.0.0] - 2026-08-26

### Added
- **Core Interfaces & CQRS:** `IMediator`, `ISender`, `IPublisher`, `ICommand<T>`, `IQuery<T>`, `INotification`, `IStreamRequest<T>`, `ICommandHandler<T, R>`, `IQueryHandler<T, R>`, `INotificationHandler<T>`, `IStreamRequestHandler<T, R>`, and `StaticMediator`.
- **Zero-Allocation Struct Pipelines:** Compile-time monomorphized execution using `IPipelineBehavior<TRequest, TResponse>`, `INotificationBehavior<TNotification>`, and nested `struct` continuations (`INext<T>`, `INext`).
- **Multi-Targeting Ecosystem:** Full multi-targeting for `.NET 8.0` (LTS), `.NET 9.0` (STS), and `.NET 10.0` across all runtime and extension packages, with `netstandard2.0` support for the Roslyn Generator.
- **Roslyn Incremental Source Generator:** Compile-time dispatch tables, monomorphized pipelines, and DI registration (`services.AddEricksonLopezMediator()`) without runtime reflection or dynamic code emit.
- **Notification Strategies:** Sequential execution (default), parallel fan-out (`[PublishStrategy(PublishStrategy.Parallel)]`), and exception-aggregating sequential dispatch (`[PublishStrategy(PublishStrategy.SequentialAggregateExceptions)]`).
- **IDE Roslyn Diagnostics (ELM001–ELM011):** Real-time compiler diagnostics detecting missing handlers, duplicate handlers, invalid signatures, generic ambiguities, missing notifications, and behavior ordering conflicts.
- **ASP.NET Core Integration (`EricksonLopez.Mediator.AspNetCore`):** Native Minimal API endpoint mappings via `MapCommand` and `MapQuery`.
- **OpenTelemetry Observability (`EricksonLopez.Mediator.OpenTelemetry`):** Out-of-the-box distributed tracing with `ActivitySource` and execution metrics via `Meter`.
- **Polly Resilience Integration (`EricksonLopez.Mediator.Polly`):** Resilience pipelines (Retry, Circuit Breaker, Timeout, Hedging) powered by Polly v8 via `[UseResiliencePipeline]` and `AddMediatorPolly()`.
- **Rate Limiting (`EricksonLopez.Mediator.RateLimiting`):** Concurrency and throughput control with `System.Threading.RateLimiting`, emitting `RateLimitExceededException` with `RetryAfter` metadata.
- **Result Pattern Integration (`EricksonLopez.Mediator.Result`):** Dedicated AOT-safe short-circuiting integration package with `IResultFactory<TResponse>`.
- **FluentValidation Integration (`EricksonLopez.Mediator.FluentValidation`):** High-performance pipeline validation short-circuiting via `ValidationPipelineBehavior<TRequest, TResponse>` and `AddMediatorFluentValidation()`.
- **Health Checks (`EricksonLopez.Mediator`):** Built-in mediator readiness health checks via `AddMediatorHealthCheck()` and `MediatorHealthCheck`.
- **Testing Utilities (`EricksonLopez.Mediator.Testing`):** Test doubles including `FakeMediator`, assertion helpers (`ShouldHaveReceived<T>`, `ReceivedRequestsOf<T>`), and `DelegateNext` continuations.

### Breaking Changes
- **Result Pattern Decoupling (BC-001):** Removed `IResultFactory<TResponse>` and the `EricksonLopez.Result` dependency from the core `EricksonLopez.Mediator` package to ensure a zero-dependency core.
  - *Migration:* Projects using Result short-circuiting must install `EricksonLopez.Mediator.Result` and add `using EricksonLopez.Mediator.Result;`.
- **Package Replacement (BC-002):** Replaced `EricksonLopez.Mediator.Validation` with `EricksonLopez.Mediator.FluentValidation`.
  - *Migration:* In `.csproj`, replace `<PackageReference Include="EricksonLopez.Mediator.Validation" />` with `<PackageReference Include="EricksonLopez.Mediator.FluentValidation" />`.
- **Validation Pipeline Renaming (BC-003):** Renamed `ValidationBehavior<TRequest, TResponse>` to `ValidationPipelineBehavior<TRequest, TResponse>` and DI registration method `AddMediatorValidation()` to `AddMediatorFluentValidation()`.
  - *Migration:* Update DI configuration in `Program.cs` from `services.AddMediatorValidation()` to `services.AddMediatorFluentValidation()`.
- **Rate Limiting Exception Model (BC-004):** `RateLimitingBehavior` now throws `RateLimitExceededException` containing `RetryAfter` metadata instead of failing silently or throwing generic exceptions.
  - *Migration:* Exception handling middleware should handle `RateLimitExceededException` and map `RetryAfter` to HTTP 429 status codes.

### Removed
- Removed legacy package `EricksonLopez.Mediator.Validation` from solution and distribution channels.
- Removed transitive dependency on `EricksonLopez.Result` from core `EricksonLopez.Mediator`.

---

## [1.0.0-rc1] - 2026-08-13

### Added
- **Core Interfaces & CQRS**: Initial release candidate with `IMediator`, `ISender`, `IPublisher`, `ICommand<T>`, `IQuery<T>`, `INotification`, `ICommandHandler<T, R>`, `IQueryHandler<T, R>`, `INotificationHandler<T>`, and `StaticMediator`.
- **Zero-Allocation Pipelines**: Zero-allocation pipeline execution using `IPipelineBehavior<TRequest, TResponse>` and nested `struct` continuations (`INext<T>`, `INext`).
- **Behavior Ordering & Lifetimes**: Deterministic behavior ordering with `[UseGlobalBehavior]`, `[UseBehavior]`, and configurable DI lifetimes with `[ServiceLifetime]`.
- **Source Generator**: Roslyn Incremental Generator computing and weaving dispatch tables at compile-time.
- **IDE Roslyn Diagnostics**: Added compile-time diagnostics `ELM001` through `ELM011`.
- **ASP.NET Core Integration**: `EricksonLopez.Mediator.AspNetCore` package with Minimal API endpoint mappings.
- **OpenTelemetry Observability**: `EricksonLopez.Mediator.OpenTelemetry` package for automatic `ActivitySource` tracing.
- **Polly Resilience**: `EricksonLopez.Mediator.Polly` package with Polly v8 resilience policies.
- **Rate Limiting**: `EricksonLopez.Mediator.RateLimiting` package with `System.Threading.RateLimiting` pipeline behavior.
- **Testing Utilities**: `EricksonLopez.Mediator.Testing` package providing `FakeMediator` test double.
