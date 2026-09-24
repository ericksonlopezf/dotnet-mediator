# ADR-036: Deprecation of EricksonLopez.Mediator.Polly in Favor of EricksonLopez.Resilience.Mediator

## Status
Accepted

## Date
2026-08-28

**Status**: Approved (Implemented in v1.0)

**Date**: 2026-08-28

**Supersedes**: —

**Superseded by**: —

---

## Context

`EricksonLopez.Mediator.Polly` was introduced as an integration package to apply Polly v8 resilience pipelines
to Mediator request handling via `PollyResilienceBehavior<TRequest, TResponse>`. The package exposes Polly's
`ResiliencePipelineProvider<string>` and `ResiliencePipeline` types directly in its registration API
(`AddMediatorPolly`, `AddMediatorDefaultResiliencePipeline`).

This creates a **Clean Architecture violation**: the Application layer's pipeline behavior depends directly on
`Polly.Core`, making Polly a first-class citizen of the application tier rather than an infrastructure detail.

The ecosystem now provides `EricksonLopez.Resilience` — a dedicated resilience framework that:

1. Exposes its own `IResiliencePipeline` / `IResilienceExecutor` abstractions (BCL-only contracts) in `EricksonLopez.Resilience.Abstractions`.
2. Wraps Polly v8 exclusively in `EricksonLopez.Resilience.Polly` (L4 infrastructure adapter), invisible to Application.
3. Provides `EricksonLopez.Resilience.Mediator` — a zero-allocation struct-continuation `ResiliencePipelineBehavior` for Mediator that depends only on `EricksonLopez.Resilience` contracts, not Polly directly.
4. Provides domain-aware retry classification via `ResultRetryClassifier` (`ErrorType.Infrastructure` → retry, `ErrorType.Validation` → no retry).

The co-existence of `Mediator.Polly` and `Resilience.Mediator` violates the `ONE CAPABILITY → ONE OWNER` principle
for "Mediator resilience pipeline integration".

## Decision

Mark all public types in `EricksonLopez.Mediator.Polly` with `[Obsolete(error: false)]` directing consumers to
`EricksonLopez.Resilience.Mediator`. The package is retained for backward compatibility but will not receive
new features. Removal is planned for the next major version (v2.0).

Affected types:
- `PollyResilienceBehavior<TRequest, TResponse>` — replaced by `ResiliencePipelineBehavior<TRequest, TResponse>` in `EricksonLopez.Resilience.Mediator`
- `UseResiliencePipelineAttribute` — replaced by `[IResilientRequest]` marker interface or policy registration in `EricksonLopez.Resilience`
- `MediatorPollyExtensions.AddMediatorPolly` — replaced by `services.AddEricksonLopezResilience(...)` + `services.AddResilienceMediatorBehavior()`
- `MediatorPollyExtensions.AddMediatorDefaultResiliencePipeline` — replaced by named policy registration in `EricksonLopez.Resilience.DependencyInjection`

## Canonical Authority

| Capability | Owner |
|---|---|
| Mediator resilience pipeline behavior | `EricksonLopez.Resilience.Mediator` |
| Resilience policy registration | `EricksonLopez.Resilience.DependencyInjection` |
| Polly execution engine | `EricksonLopez.Resilience.Polly` (L4, internal adapter) |

## Consequences

### Positive
- Eliminates Polly as a first-class dependency of the Application layer pipeline.
- Single ownership of Mediator resilience concern (`EricksonLopez.Resilience.Mediator`).
- Domain-aware retry classification (`ErrorType` integration) available to all Mediator behaviors.
- Enables multi-tenant context (`TenantId`) in resilience execution without extra coupling.

### Negative
- Consumers of `Mediator.Polly` receive `CS0618` warnings and must migrate.
- Migration requires adding a new package reference to `EricksonLopez.Resilience.DependencyInjection`.

### Neutral
- `Mediator.Polly` continues to function correctly (`error: false`). No runtime behavior change.
- Hard removal deferred to v2.0 (semver major) to provide migration window.

## Migration Guide

**Before (deprecated):**
```csharp
// Program.cs
services.AddMediatorDefaultResiliencePipeline(builder =>
{
    builder.AddRetry(new RetryStrategyOptions { MaxRetryAttempts = 3 });
    builder.AddTimeout(TimeSpan.FromSeconds(30));
});
```

**After (canonical):**
```csharp
// Program.cs
services.AddEricksonLopezResilience(options =>
{
    options.AddPolicy("default", builder =>
    {
        builder
            .AddTimeout(TimeSpan.FromSeconds(30))
            .AddResultRetry(opt => { opt.MaxRetryAttempts = 3; });
    });
});
services.AddResilienceMediatorBehavior();
```

## References

- ADR-014: Rejected built-in auto-retry in Mediator core.
- ADR-026: Decoupling Result pattern into `EricksonLopez.Mediator.Result`.
- `EricksonLopez.Resilience` README — Architecture & Design Principles.
- Ecosystem Convergence Plan — Section 2.1 (Resilience Authority).
