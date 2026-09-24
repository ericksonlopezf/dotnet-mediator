# ADR-038: Response Caching Pipeline Behavior via Dedicated Integration Package

## Status
Approved

## Date
2026-09-08

## Context
In ADR-017, built-in response caching was explicitly rejected from the core `EricksonLopez.Mediator` package to uphold the zero-dependency, ultra-lightweight architectural invariant of the core dispatcher. Caching involves key generation policies, time-to-live (TTL) calculations, cache invalidation rules, and backend storage providers, which are outside the responsibility of an in-process mediator kernel.

However, enterprise CQRS applications frequently require deterministic, stampede-protected caching on read queries (`IQuery<T>`) and automated cache invalidation on write commands (`ICommand<T>`). Providing this capability through unstandardized, ad-hoc behaviors leads to code duplication, improper handling of asynchronous continuation tokens, and potential memory leaks under Native AOT.

## Decision
1. **Dedicated Extension Package**: Introduce `EricksonLopez.Mediator.Caching` as a separate, opt-in integration package. The core `EricksonLopez.Mediator` package remains completely decoupled from caching abstractions.
2. **Ecosystem Integration**: Integrate directly with `EricksonLopez.Caching` to leverage multi-tier in-memory and distributed caching with cache-stampede protection.
3. **Pipeline Behavior Contracts**:
   - `CachingPipelineBehavior<TRequest, TResponse>`: Implements `IPipelineBehavior<TRequest, TResponse>` with `where TNext : struct, INext<TResponse>` to maintain zero-allocation struct continuation chains.
   - `ICacheableRequest`: Interface defining cache keys, TTL, and cache options.
   - `[Cacheable]`: Declarative attribute for query types defining automatic key generation and expiration.
   - `IInvalidateCacheRequest`: Contract for commands that evict single keys or wildcard key prefixes upon successful completion.
4. **Native AOT Guarantee**: Ensure that all serialization and cache key evaluation pathways are fully trimmable and 100% Native AOT compliant without reflection.
5. **Fluent DI Registration**: Provide `AddMediatorCaching(this IServiceCollection services)` for seamless registration.

## Consequences

### Positive
- **Core Invariant Preserved**: The core `EricksonLopez.Mediator` engine remains free of caching and serialization dependencies (consistent with ADR-017).
- **Zero-Allocation Pipeline**: The caching behavior uses `struct INext<TResponse>` continuations, avoiding heap closures on cache misses.
- **Native AOT Compatible**: Zero trimmer warnings under aggressive ILC compilation (`PublishAot=true`).
- **Declarative & Programmatic**: Supports both attribute-driven (`[Cacheable]`) and contract-driven (`ICacheableRequest`) caching models.

### Negative
- Adds one additional project and package to maintain within the repository ecosystem.
