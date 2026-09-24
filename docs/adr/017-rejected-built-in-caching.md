# ADR-017: REJECTED — Built-in Caching

## Status
Rejected

## Date
2026-09-04

**Status**: Accepted (Rejection)

### Context
Some users want automatic caching of query results via the mediator.

### Decision
**REJECTED from core**. Caching is highly domain-specific (TTL, cache keys, invalidation) and belongs to application logic or an extension behavior.

### Why
- Cache key generation requires domain knowledge (which fields matter?)
- TTL is business logic, not infrastructure concern
- Cache invalidation is one of the hardest problems in computer science; the mediator should not attempt it
- A `CachingBehavior<TRequest, TResponse>` using `IMemoryCache` or `IDistributedCache` can be written in 30 lines

---

> [!NOTE]
> While caching remains excluded from the core dispatcher package, an official opt-in extension package `EricksonLopez.Mediator.Caching` was introduced via [ADR-038](038-response-caching-pipeline-behavior-package.md) to provide declarative response caching and stampede protection.
