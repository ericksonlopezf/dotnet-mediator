# Migration Guide: MediatR → EricksonLopez.Mediator

This guide helps you migrate an existing codebase from `MediatR` to `EricksonLopez.Mediator`.

---

## 1. Package References

Replace `MediatR` and `MediatR.Contracts` packages:

```xml
<!-- Before -->
<PackageReference Include="MediatR" Version="12.4.1" />

<!-- After -->
<PackageReference Include="EricksonLopez.Mediator" Version="2.0.0" />
<PackageReference Include="EricksonLopez.Mediator.Generator" Version="2.0.0" OutputItemType="Analyzer" ReferenceOutputAssembly="false" />
```

---

## 2. Request Interfaces & Compatibility Layer

`EricksonLopez.Mediator` supports two migration pathways:

### Option A: Zero-Friction Drop-In Migration (Compatibility Types)
To minimize migration friction in large codebases, `EricksonLopez.Mediator` provides native compatibility abstractions in the root namespace:
- `IRequest<TResponse>` (treated as command request)
- `IRequest` (equivalent to `IRequest<Unit>`)
- `IRequestHandler<TRequest, TResponse>`
- `IRequestHandler<TRequest>`
- `Unit` (zero-size readonly struct with `Unit.Value`)

The Roslyn Incremental Source Generator natively recognizes `IRequestHandler<TRequest, TResponse>` and generates static compile-time dispatch routes for them automatically!

### Option B: Strict CQRS Segregation (Recommended Target)
For clean architectural boundaries, segregate requests into explicit commands and queries:

| MediatR | EricksonLopez.Mediator (Target) |
|---|---|
| `IRequest<TResponse>` (Command) | `ICommand<TResponse>` |
| `IRequest<TResponse>` (Query) | `IQuery<TResponse>` |
| `IRequest` (Void Command) | `ICommand<Unit>` or `ICommand<Result>` |
| `INotification` | `INotification` |

---

## 3. Handler Signatures

Handlers in `EricksonLopez.Mediator` return `ValueTask<TResponse>` instead of `Task<TResponse>` to enable zero heap allocations for synchronously completed operations:

```csharp
// Before (MediatR)
public class PingHandler : IRequestHandler<PingCommand, string>
{
    public Task<string> Handle(PingCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult("Pong");
    }
}

// After (Option A - Compatibility Handler)
public class PingHandler : IRequestHandler<PingCommand, string>
{
    public ValueTask<string> Handle(PingCommand request, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult("Pong");
    }
}

// After (Option B - Strict CQRS Handler)
public class PingHandler : ICommandHandler<PingCommand, string>
{
    public ValueTask<string> Handle(PingCommand command, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult("Pong");
    }
}
```

---

## 4. Pipeline Behaviors

Replace `RequestHandlerDelegate<TResponse>` with the generic struct `TNext` constraint:

```csharp
// Before (MediatR)
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        return await next();
    }
}

// After (EricksonLopez.Mediator)
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    public async ValueTask<TResponse> Handle<TNext>(TRequest request, TNext next, CancellationToken ct)
        where TNext : struct, INext<TResponse>
    {
        return await next.InvokeAsync().ConfigureAwait(false);
    }
}
```

---

## 5. Dependency Injection Registration

Replace `services.AddMediatR(cfg => ...)` with the source-generated extension:

```csharp
// Before (MediatR)
services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// After (EricksonLopez.Mediator)
services.AddEricksonLopezMediator();
```
