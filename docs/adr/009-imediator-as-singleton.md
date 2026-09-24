# ADR-009: IMediator as Singleton

## Status
Superseded by [ADR-037](037-scoped-mediator-by-default.md)

## Date
2026-09-04

**Status**: Superseded by [ADR-037](037-scoped-mediator-by-default.md)

### Decision
`GeneratedMediator` (implementing `IMediator`) is registered as Singleton.

### Why
- `GeneratedMediator` holds only an `IServiceProvider` reference (thread-safe)
- No mutable state in the mediator itself
- Singleton reduces DI resolution overhead
- Handlers are resolved per-call from IServiceProvider (correct lifetime management)

### Consequences
+ Zero DI resolution overhead for IMediator
+ Thread-safe by construction
- Developer cannot have per-request state in the mediator (correct — use handlers instead)
