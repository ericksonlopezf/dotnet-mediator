// Copyright © Erickson Lopez. MIT License.
using System;
using System.Threading.Tasks;

namespace EricksonLopez.Mediator.Caching.Tests;

public readonly struct TestNextContinuation<TResponse> : INext<TResponse>
{
    private readonly Func<ValueTask<TResponse>> _callback;

    public TestNextContinuation(Func<ValueTask<TResponse>> callback)
    {
        _callback = callback ?? throw new ArgumentNullException(nameof(callback));
    }

    public ValueTask<TResponse> InvokeAsync() => _callback();
}
