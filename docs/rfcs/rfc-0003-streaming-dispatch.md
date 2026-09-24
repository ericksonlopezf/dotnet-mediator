# RFC 0003: High-Throughput IAsyncEnumerable Streaming Dispatch

- **Author**: Erickson Lopez
- **Date**: 2026-08-26
- **Status**: Implemented (Promoted to stable via ADR-034 as `IStreamRequest<TResponse>`)

## 1. Summary
Provides first-class streaming request dispatch via `IStreamRequest<TResponse>` and `IStreamRequestHandler<TRequest, TResponse>`, yielding asynchronous response sequences without buffering complete datasets in memory.

## 2. Motivation
Large queries and data feeds (e.g., historical telemetry, database cursor feeds) require reactive streaming to minimize memory footprint and enable backpressure handling.

## 3. Detailed Design
```csharp
public interface IStreamRequest<out TResponse> { }

public interface IStreamRequestHandler<in TRequest, out TResponse>
    where TRequest : IStreamRequest<TResponse>
{
    IAsyncEnumerable<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}
```
