# ADR-037: Scoped Mediator Lifetime by Default (Supersedes ADR-009)

## Status
Approved

## Date
2026-09-05

## Context
In ADR-009, `GeneratedMediator` (implementing `IMediator`, `ISender`, and `IPublisher`) was registered into the dependency injection container as a `Singleton`. The initial rationale was that `GeneratedMediator` does not hold internal mutable state and only references `IServiceProvider`.

However, comprehensive adversarial testing (Finding `F-001` / `BREAK-001`) revealed two severe defects with the `Singleton` registration:
1. **Scope Validation Failure in Development (`ValidateScopes = true`)**:
   In ASP.NET Core development mode, `ValidateScopes` is enabled by default. When a request or background task dispatches a command/query handled by a `Scoped` handler (e.g., holding a database `DbContext` or tenant context), `GeneratedMediator` attempts to resolve `_serviceProvider.GetRequiredService<TScopedHandler>()` from the root provider, throwing:
   `InvalidOperationException: Cannot resolve scoped service '...' from root provider`.
2. **Captive Dependency & Multitenant Bleed in Production (`ValidateScopes = false`)**:
   When scope validation is disabled (standard in production), the singleton mediator captures the first resolved `Scoped` handler instance permanently across all subsequent HTTP requests, leading to data leaks between tenants, race conditions on non-thread-safe `DbContext` instances, and stale cache state.

## Decision
1. **Supersede ADR-009**: Register `IMediator`, `ISender`, and `IPublisher` as **`Scoped`** by default in `AddEricksonLopezMediator`.
2. **Configurable Lifetime Parameter**: Provide an optional `ServiceLifetime lifetime = ServiceLifetime.Scoped` parameter in `AddEricksonLopezMediator(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)`.
3. In ASP.NET Core, `IMediator` is resolved from `HttpContext.RequestServices` per request, cleanly managing the lifecycle of scoped handlers and database contexts.
4. For standalone console applications, daemon workers, or test doubles with only singleton handlers, consumers can explicitly opt into `ServiceLifetime.Singleton`.

## Consequences

### Positive
- **Guaranteed Isolation**: Scoped handlers (e.g. EF Core `DbContext`, Unit of Work, multitenant credentials) are properly created and disposed per HTTP request/scope.
- **Zero Captive Dependencies**: Eliminates captive dependencies without requiring manual `IServiceScopeFactory` boilerplate in consumers.
- **Full ASP.NET Core Compatibility**: Passes ASP.NET Core default scope validation (`ValidateScopes = true`) seamlessly.
- **Opt-in Flexibility**: Non-web workers can still select `ServiceLifetime.Singleton` via optional parameter.

### Negative
- Resolving `IMediator` per request adds ~5 ns of DI lookup overhead per HTTP request scope (negligible compared to network I/O).
