# Repository Inventory — EricksonLopez.Mediator

This document provides a comprehensive, verified inventory of all source projects, test suites, sample applications, dependencies, and build configurations in the `EricksonLopez.Mediator` repository.

---

## 1. Source Projects (10 Packages)

All 10 source projects are packable and produce independent NuGet packages:

| Project | Target Framework(s) | Packable | Description |
|---|---|:---:|---|
| [`src/EricksonLopez.Mediator`](../src/EricksonLopez.Mediator) | `net8.0;net9.0;net10.0` | Yes | Core mediator contracts (`IMediator`, `ISender`, `IPublisher`, `ICommand`, `IQuery`, `INotification`, `IStreamRequest`), struct continuations (`INext<T>`, `INext`), attributes, and `StaticMediator`. |
| [`src/EricksonLopez.Mediator.Generator`](../src/EricksonLopez.Mediator.Generator) | `netstandard2.0` | Yes | Incremental Roslyn Source Generator weaving compile-time switch dispatch, handler registration, and IDE diagnostics (`ELM001`–`ELM011`). |
| [`src/EricksonLopez.Mediator.AspNetCore`](../src/EricksonLopez.Mediator.AspNetCore) | `net8.0;net9.0;net10.0` | Yes | ASP.NET Core Minimal API endpoint route extensions (`MapCommand`, `MapQuery`). |
| [`src/EricksonLopez.Mediator.Caching`](../src/EricksonLopez.Mediator.Caching) | `net8.0;net9.0;net10.0` | Yes | Transparent query caching and invalidation pipeline behaviors with stampede protection (ADR-038). |
| [`src/EricksonLopez.Mediator.FluentValidation`](../src/EricksonLopez.Mediator.FluentValidation) | `net8.0;net9.0;net10.0` | Yes | FluentValidation pipeline behavior and fluent DI registration helpers. Replaced legacy `Validation` package (ADR-033). |
| [`src/EricksonLopez.Mediator.OpenTelemetry`](../src/EricksonLopez.Mediator.OpenTelemetry) | `net8.0;net9.0;net10.0` | Yes | OpenTelemetry distributed tracing (`ActivitySource`) and runtime performance metrics (`Meter`) with zero reflection. |
| [`src/EricksonLopez.Mediator.Polly`](../src/EricksonLopez.Mediator.Polly) | `net8.0;net9.0;net10.0` | Yes | Polly v8 resilience pipeline behavior. Deprecated per ADR-036 in favor of `EricksonLopez.Resilience.Mediator`. |
| [`src/EricksonLopez.Mediator.RateLimiting`](../src/EricksonLopez.Mediator.RateLimiting) | `net8.0;net9.0;net10.0` | Yes | Concurrency and rate limiting pipeline behavior using `System.Threading.RateLimiting`. |
| [`src/EricksonLopez.Mediator.Result`](../src/EricksonLopez.Mediator.Result) | `net8.0;net9.0;net10.0` | Yes | Decoupled result factory layer (`IResultFactory<TResponse>`) bridging mediator pipelines with `EricksonLopez.Result`. |
| [`src/EricksonLopez.Mediator.Testing`](../src/EricksonLopez.Mediator.Testing) | `net8.0;net9.0;net10.0` | Yes | Official in-memory `FakeMediator` and `DelegateNext` test doubles for unit testing without container setup. |

---

## 2. Test, Benchmark & Sample Projects

| Project | Target Framework(s) | Type | Purpose |
|---|---|:---:|---|
| [`tests/EricksonLopez.Mediator.AotSmokeTest`](../tests/EricksonLopez.Mediator.AotSmokeTest) | `net10.0` | Native AOT Smoke | Dedicated `PublishAot=true` console smoke test enforcing zero trimmer warnings and native execution. |
| [`tests/EricksonLopez.Mediator.Tests`](../tests/EricksonLopez.Mediator.Tests) | `net10.0` | Unit Tests | Core dispatcher, static mediator, behavior chains, and notification test suite. |
| [`tests/EricksonLopez.Mediator.AspNetCore.Tests`](../tests/EricksonLopez.Mediator.AspNetCore.Tests) | `net10.0` | Unit Tests | Tests for `MapCommand` and `MapQuery` Minimal API route binding extensions. |
| [`tests/EricksonLopez.Mediator.Caching.Tests`](../tests/EricksonLopez.Mediator.Caching.Tests) | `net10.0` | Unit Tests | Tests for query caching, eviction commands, and struct continuations in caching pipeline. |
| [`tests/EricksonLopez.Mediator.FluentValidation.Tests`](../tests/EricksonLopez.Mediator.FluentValidation.Tests) | `net10.0` | Unit Tests | Tests for FluentValidation pipeline behavior and DI registration extensions. |
| [`tests/EricksonLopez.Mediator.Generator.Tests`](../tests/EricksonLopez.Mediator.Generator.Tests) | `net10.0` | Compiler Tests | Roslyn code generator compilation tests verifying dispatch emission, DI registration, and diagnostics (`ELM001`–`ELM011`). |
| [`tests/EricksonLopez.Mediator.IntegrationTests`](../tests/EricksonLopez.Mediator.IntegrationTests) | `net10.0` | Integration Tests | End-to-end ASP.NET Core Minimal API integration tests via `WebApplicationFactory`. |
| [`tests/EricksonLopez.Mediator.OpenTelemetry.Tests`](../tests/EricksonLopez.Mediator.OpenTelemetry.Tests) | `net10.0` | Unit Tests | ActivitySource distributed tracing and Meter metrics verification. |
| [`tests/EricksonLopez.Mediator.Polly.Tests`](../tests/EricksonLopez.Mediator.Polly.Tests) | `net10.0` | Unit Tests | Polly v8 resilience pipeline behavior tests. |
| [`tests/EricksonLopez.Mediator.RateLimiting.Tests`](../tests/EricksonLopez.Mediator.RateLimiting.Tests) | `net10.0` | Unit Tests | Concurrency and token bucket rate limiter tests. |
| [`tests/EricksonLopez.Mediator.Result.Tests`](../tests/EricksonLopez.Mediator.Result.Tests) | `net8.0;net9.0;net10.0` | Multi-TFM Tests | Result pattern short-circuiting integration tests across all supported .NET versions. |
| [`tests/EricksonLopez.Mediator.Testing.Tests`](../tests/EricksonLopez.Mediator.Testing.Tests) | `net10.0` | Unit Tests | Tests for `FakeMediator` assertions, setups, and `DelegateNext` continuations. |
| [`benchmarks/EricksonLopez.Mediator.Benchmarks`](../benchmarks/EricksonLopez.Mediator.Benchmarks) | `net8.0;net9.0;net10.0` | Benchmarks | BenchmarkDotNet throughput and memory allocation performance suite comparing against MediatR. |
| [`samples/EricksonLopez.Mediator.Samples`](../samples/EricksonLopez.Mediator.Samples) | `net10.0` | Sample App | 12-level progressive executable living showcase application demonstrating all mediator capabilities. |

---

## 3. Central Package Management (CPM)

Dependencies are pinned centrally in `Directory.Packages.props`:

| Package | Pinned Version | Scope |
|---|---|---|
| `Microsoft.Extensions.DependencyInjection.Abstractions` | `10.0.11` | Core & Integrations |
| `Microsoft.Extensions.DependencyInjection` | `10.0.11` | Core, Samples & Tests |
| `Microsoft.Extensions.Diagnostics.HealthChecks.Abstractions` | `10.0.11` | Core Health Checks |
| `System.Threading.RateLimiting` | `10.0.11` | Rate Limiting Extension |
| `OpenTelemetry.Api` | `1.18.0` | Observability Extension |
| `Polly.Core` | `8.7.0` | Resilience Extension |
| `FluentValidation` | `12.1.1` | Validation Extension |
| `EricksonLopez.Result` | `2.0.0` | Result Extension |
| `EricksonLopez.Result.FluentValidation` | `2.0.0` | Validation Extension |
| `EricksonLopez.Caching` | `1.0.0` | Caching Extension |
| `Microsoft.SourceLink.GitHub` | `10.0.400` | SourceLink Git Integration |
| `Microsoft.CodeAnalysis.CSharp` | `5.9.0` | Roslyn Generator |
| `Microsoft.CodeAnalysis.Analyzers` | `5.9.0` | Roslyn Generator |
| `Microsoft.CodeAnalysis.PublicApiAnalyzers` | `5.6.0` | Public API Guardrails |
| `Microsoft.NET.Test.Sdk` | `18.9.0` | Test SDK |
| `xunit` | `2.9.3` | Test Runner |
| `xunit.v3` | `4.0.0` | Test Runner (v3 Engine) |
| `xunit.runner.visualstudio` | `4.0.0` | Test Runner Adapter |
| `AwesomeAssertions` | `9.6.0` | Test Assertions |
| `NSubstitute` | `6.2.0` | Mocking |
| `AutoFixture.Xunit2` | `4.18.1` | Test Data Fixtures |
| `coverlet.collector` | `10.0.1` | Code Coverage Collector |
| `coverlet.msbuild` | `10.0.1` | Code Coverage MSBuild Integration |
| `coverlet.MTP` | `10.0.1` | Code Coverage Platform |
| `BenchmarkDotNet` | `0.15.8` | Performance Benchmarks |
| `Microsoft.AspNetCore.Mvc.Testing` | `10.0.11` | Integration Tests |

---

## 4. Quality & Build Tooling

- **.NET SDK**: `10.0.x` (enables C# 14 / latest language features and multi-targeting down to .NET 8.0).
- **Static Analysis**: `TreatWarningsAsErrors=true`, `AnalysisLevel=latest-recommended`, `EnforceCodeStyleInBuild=true`.
- **Trimming & AOT**: `EnableTrimAnalyzer=true`, `IsAotCompatible=true` across all applicable runtime packages.
- **Strong Name Signing**: Centralized RSA signing via `Directory.Build.props` using `EricksonLopez.snk`.
- **Mutation Testing**: Stryker.NET 4.16.0 with 10 dedicated package profiles (Thresholds: High=100%, Low=98%, Break=95%, Concurrency=2).
- **Public API Tracking**: `PublicAPI.Shipped.txt` and `PublicAPI.Unshipped.txt` in core.

---

## 5. GitHub Repository Metadata & Topics

Official GitHub repository metadata configuration for `ericksonlopezf/dotnet-mediator`:

- **Description**: `High-throughput, zero-allocation CQRS mediator ecosystem for modern .NET (8, 9, 10). Features Roslyn compile-time dispatch, 100% Native AOT compatibility, and zero-overhead pipeline extensions for caching, telemetry, and validation.`
- **Website**: `https://ericksonlopez.dev/mediator`
- **Official Topics (20 / 20 Maximum)**:
  1. `dotnet`
  2. `csharp`
  3. `mediator`
  4. `cqrs`
  5. `source-generator`
  6. `roslyn`
  7. `native-aot`
  8. `zero-allocation`
  9. `pipeline`
  10. `domain-events`
  11. `streaming`
  12. `aspnetcore`
  13. `minimal-apis`
  14. `caching`
  15. `fluentvalidation`
  16. `opentelemetry`
  17. `resilience`
  18. `rate-limiting`
  19. `result-pattern`
  20. `unit-testing`

