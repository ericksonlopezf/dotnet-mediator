# Project Roadmap

This document outlines the strategic milestones, completed features, and architectural plans for `EricksonLopez.Mediator`.

---

## Current Release: v2.0.0 (General Availability - 2026-09-24)

Version `2.0.0` GA is released and delivers the fastest zero-allocation, reflection-free in-process CQRS mediator for modern .NET (`net8.0`, `net9.0`, `net10.0`), featuring scoped mediator by default (ADR-037), MediatR migration contracts, dedicated query caching (`EricksonLopez.Mediator.Caching`), and zero `[Obsolete]` APIs.

### Implemented Core Capabilities:
- [x] **Strict CQRS Segregation**: Dedicated contracts (`IMediator`, `ISender`, `IPublisher`, `ICommand<T>`, `IQuery<T>`, `INotification`, `IStreamRequest<T>`).
- [x] **Zero-Allocation Pipeline Execution**: Compile-time monomorphized `struct INext<TResponse>` and `struct INext` continuations (0 B heap allocation on hot path).
- [x] **Roslyn Incremental Source Generator**: Compile-time dispatch tables, monomorphized pipelines, and scoped DI wiring (`services.AddEricksonLopezMediator(ServiceLifetime.Scoped)`).
- [x] **Rich IDE Diagnostics**: Compile-time diagnostics `ELM001` through `ELM011` preventing architectural defects at build time.
- [x] **Flexible Notification Strategies**: Sequential (default), Parallel fan-out, and Exception-Aggregating execution via `[PublishStrategy]`.
- [x] **Zero-DI Serverless Execution**: Static direct-dispatch engine (`StaticMediator`) with type-safe `SendCommand` and `SendQuery`.
- [x] **Cross-Assembly Discovery**: Compile-time discovery via `[assembly: DiscoverHandlers(typeof(Marker))]`.
- [x] **MediatR Migration Compatibility**: Optional migration contracts (`IRequest<T>`, `IRequest`, `IRequestHandler<T, R>`, `Unit`) adhering to one-type-per-file.
- [x] **Removed Deprecated Overloads (ADR-032)**: Removed reflection-based polymorphic `StaticMediator.Send` overloads in favor of strongly-typed `SendCommand` and `SendQuery`.

### Active Ecosystem Packages (v2.0):
- [x] `EricksonLopez.Mediator` — Core CQRS abstractions, struct `INext`, and `StaticMediator`.
- [x] `EricksonLopez.Mediator.Generator` — Roslyn Source Generator and IDE diagnostics (`ELM001`–`ELM011`).
- [x] `EricksonLopez.Mediator.AspNetCore` — Minimal API endpoint mapping extensions (`MapCommand`, `MapQuery`).
- [x] `EricksonLopez.Mediator.Caching` — Response caching and cache invalidation pipeline behavior (ADR-038).
- [x] `EricksonLopez.Mediator.FluentValidation` — High-performance FluentValidation pipeline integration.
- [x] `EricksonLopez.Mediator.OpenTelemetry` — Distributed tracing (`ActivitySource`) and runtime metrics (`Meter`).
- [x] `EricksonLopez.Mediator.Polly` — Polly v8 resilience pipeline integration.
- [x] `EricksonLopez.Mediator.RateLimiting` — `System.Threading.RateLimiting` pipeline behavior.
- [x] `EricksonLopez.Mediator.Result` — Result pattern short-circuiting integration via `IResultFactory<TResponse>`.
- [x] `EricksonLopez.Mediator.Testing` — Official in-memory `FakeMediator` and `DelegateNext` test doubles.

---

## Strategic Future Milestones

- [ ] **Strict Native AOT Default in Templates**: Enable `PublishAot=true` by default in all new project templates and sample applications.
- [ ] **Ecosystem Resilience Migration**: Provide automatic code-fixers for migrating Polly behaviors to `EricksonLopez.Resilience.Mediator`.

---

## Compatibility Policy

- **Target Frameworks**: `net8.0` (LTS), `net9.0` (STS), `net10.0` (LTS) for runtime packages; `netstandard2.0` for Roslyn Source Generator.
- **Native AOT & Trimming**: 100% Native AOT compatibility is an inviolable architectural requirement across all core runtime packages.
- **Semantic Versioning**: Strict adherence to SemVer 2.0. No breaking API changes in `1.x` releases.
