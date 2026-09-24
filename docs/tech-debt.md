# Technical Debt & Codebase Health Registry — EricksonLopez.Mediator

This registry tracks verified architectural, pipeline, and structural technical debt identified during comprehensive codebase audits. Items are prioritized from P0 (critical/blocking) to P3 (minor/housekeeping).

---

## 1. Summary Metrics

| Category | Open Items | Status | Primary Impact Area |
|---|:---:|:---:|---|
| **CI/CD Pipeline Integrity** | 2 | Actionable | Package release completeness and CI run duration |
| **Architectural Deprecations** | 2 | Scheduled v2.0 | API surface modernization (ADR-036, StaticMediator) |
| **Code Style & Structure** | 2 | Non-Breaking | Single Responsibility Principle per file |
| **Total Tracked Items** | **6** | — | — |

---

## 2. Technical Debt Registry

### Item DEBT-001: Omission of `Caching` Package in `publish.yml` Pipeline
- **Severity**: **P1 (High)**
- **Category**: CI/CD / Release Automation
- **Location**: `.github/workflows/publish.yml` (lines 144–152)
- **Description**: While `src/EricksonLopez.Mediator.Caching` exists with `<IsPackable>true</IsPackable>`, the `publish.yml` workflow's `dotnet pack` step only packs 9 packages and omits `src/EricksonLopez.Mediator.Caching/EricksonLopez.Mediator.Caching.csproj`. The ecosystem summary table in the same workflow also lists 9 packages instead of the 10 packable packages.
- **Risk**: Automated release workflows will not publish `EricksonLopez.Mediator.Caching` to NuGet unless manually executed or updated.
- **Remediation Plan**: Add `dotnet pack src/EricksonLopez.Mediator.Caching/EricksonLopez.Mediator.Caching.csproj -c Release --no-build -o packages` to `publish.yml` step and update summary table.

---

### Item DEBT-002: Unused Database and Broker Containers in Benchmark Workflows
- **Severity**: **P2 (Medium)**
- **Category**: CI/CD Optimization
- **Location**: `.github/workflows/benchmarks.yml` (lines 37–51), `.github/workflows/weekly-benchmarks.yml` (lines 35–49)
- **Description**: The benchmark workflows declare service containers for PostgreSQL (`postgres:16-alpine`) and RabbitMQ (`rabbitmq:3-management-alpine`). However, `EricksonLopez.Mediator` is a pure in-memory CQRS mediator engine without database or message broker dependencies.
- **Risk**: Unnecessary container startup adds ~30–45 seconds of runner provisioning latency and wastes compute resources.
- **Remediation Plan**: Remove unused `services:` container declarations from both benchmark workflows.

---

### Item DEBT-003: Deprecated `EricksonLopez.Mediator.Polly` Package (ADR-036)
- **Severity**: **P2 (Medium)**
- **Category**: Architectural Deprecation
- **Location**: `src/EricksonLopez.Mediator.Polly/`
- **Description**: In accordance with ADR-036, direct coupling of Polly v8 resilience policies inside the mediator pipeline violates Clean Architecture separation of concerns. The package is superseded by `EricksonLopez.Resilience.Mediator`.
- **Risk**: Maintenance burden of legacy Polly v8 wrappers.
- **Remediation Plan**: Maintain backward compatibility with `[Obsolete]` attributes through v1.x; fully remove the package in v2.0.0.

---

### Item DEBT-004: Deprecated Signatures in `StaticMediator`
- **Severity**: **P3 (Low)**
- **Category**: API Hygiene
- **Location**: `src/EricksonLopez.Mediator/StaticMediator.cs`
- **Description**: Four methods in `StaticMediator` carry `[Obsolete]` attributes flagged by `verify-compliance.ps1` Gate 2:
  - `RegisterCommandHandler<TCommand, TResponse>(Func<TCommand, CancellationToken, ValueTask<TResponse>>)`
  - `RegisterQueryHandler<TQuery, TResponse>(Func<TQuery, CancellationToken, ValueTask<TResponse>>)`
  - Untyped `Send(object)` overloads superseded by strongly-typed `SendCommand` and `SendQuery`.
- **Risk**: Minimal; callers receive compiler warnings guiding them to strongly-typed overloads.
- **Remediation Plan**: Remove obsolete overloads in v2.0.0 breaking change cycle.

---

### Item DEBT-005: Compiler Warning Suppressions (`CS0618`/`CS0619`) in Test Projects
- **Severity**: **P3 (Low)**
- **Category**: Compiler / Quality Gates
- **Location**: `tests/EricksonLopez.Mediator.Tests/`, `tests/EricksonLopez.Mediator.Polly.Tests/`
- **Description**: Test projects define `<NoWarn>$(NoWarn);CS0618;CS0619</NoWarn>` in their `.csproj` to allow unit testing of deprecated APIs (`PollyResilienceBehavior`, obsolete `StaticMediator` methods) without triggering `TreatWarningsAsErrors=true`.
- **Risk**: Legitimate unintended usages of obsolete APIs in tests could be masked.
- **Remediation Plan**: Isolate deprecated API tests into dedicated test fixtures with local `#pragma warning disable CS0618` blocks instead of project-wide `<NoWarn>`.

---

### Item DEBT-006: Multiple Top-Level Types in Single Interface Files
- **Severity**: **P3 (Low)**
- **Category**: Code Structure Convention
- **Location**:
  - `src/EricksonLopez.Mediator/IRequest.cs` (contains `IRequest<TResponse>` and `IRequest`)
  - `src/EricksonLopez.Mediator/IRequestHandler.cs` (contains `IRequestHandler<TRequest, TResponse>` and `IRequestHandler<TRequest>`)
- **Description**: Roslyn style analyzers and .NET naming guidelines recommend one top-level type per file matching the file name.
- **Risk**: Purely aesthetic; zero runtime impact.
- **Remediation Plan**: In v2.0 refactoring, split into `IRequestOfT.cs` / `IRequest.cs` and `IRequestHandlerOfT.cs` / `IRequestHandler.cs`, or preserve with explicit analyzer suppression comments.
