# Release Notes

## Version 2.0.0 (General Availability) - 2026-09-24

We are thrilled to announce the official General Availability (GA) release of **EricksonLopez.Mediator 2.0.0**, featuring scoped mediator lifetimes by default, dedicated high-performance query caching, MediatR migration primitives, and full Native AOT compatibility across .NET 10, .NET 9, and .NET 8.

### Highlights
- **Scoped Mediator by Default (ADR-037):** `AddEricksonLopezMediator()` now defaults to `ServiceLifetime.Scoped`, eliminating captive dependencies and multitenant context leaks when handlers inject scoped dependencies (e.g., database contexts).
- **`EricksonLopez.Mediator.Caching` Package (ADR-038):** High-performance response caching and cache invalidation pipeline behaviors (`CachingPipelineBehavior<TRequest, TResponse>`, `[Cacheable]`, `ICacheableRequest`, `IInvalidateCacheRequest`).
- **MediatR Migration Primitives:** Added optional migration contracts (`IRequest<TResponse>`, `IRequest`, `IRequestHandler<TRequest, TResponse>`, `IRequestHandler<TRequest>`, `Unit`) enabling seamless transition from legacy MediatR codebases.
- **Deprecation of `EricksonLopez.Mediator.Polly` (ADR-036):** Public types marked obsolete in favor of Clean Architecture-compliant `EricksonLopez.Resilience.Mediator`.
- **AOT Hardening & Pre-flight Validation:** Enforced immediate pre-flight cancellation checks across `StaticMediator` and `FakeMediator`, with sealed source generator classes.
- **Result Ecosystem 3.0.0 Upgrade:** Upgraded dependency on `EricksonLopez.Result` and `EricksonLopez.Result.FluentValidation` to 3.0.0.

Available on NuGet today!

---

## Version 1.0.0 (General Availability) - 2026-08-26

We are thrilled to announce the official General Availability (GA) release of **EricksonLopez.Mediator 1.0.0**, a high-performance, Native AOT compatible, zero-allocation CQRS dispatching framework for .NET 10, .NET 9, and .NET 8.

### Highlights
- **Zero-Allocation Pipelines:** Compile-time monomorphized pipeline execution using nested `struct` continuations (`INext<T>`, `INext`), resulting in 0 memory allocations on the heap for `Send` calls.
- **Full Multi-Targeting:** Built and tested across `.NET 8.0` (LTS), `.NET 9.0` (STS), and `.NET 10.0`, with `netstandard2.0` support for the Roslyn Source Generator.
- **Roslyn Incremental Generator:** Automatically generates monomorphized dispatch tables and `services.AddEricksonLopezMediator()` DI registration at compile-time with zero runtime reflection.
- **Modular Ecosystem:** Dedicated packages for `AspNetCore`, `FluentValidation`, `OpenTelemetry`, `Polly`, `RateLimiting`, `Result`, and `Testing`.
- **Advanced Notification Strategies:** Opt-in Parallel Dispatch (`[PublishStrategy(PublishStrategy.Parallel)]`) and Aggregate Exception handling (`[PublishStrategy(PublishStrategy.SequentialAggregateExceptions)]`).
- **Rich IDE Diagnostics:** Real-time compile-time diagnostics (`ELM001`–`ELM011`) directly within Visual Studio and VS Code.

Available on NuGet today!

---