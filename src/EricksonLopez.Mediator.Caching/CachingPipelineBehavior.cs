// Copyright © Erickson Lopez. MIT License.
using System;
using System.Threading;
using System.Threading.Tasks;
using EricksonLopez.Caching;
using EricksonLopez.Result;

namespace EricksonLopez.Mediator.Caching;

/// <summary>
/// Provides transparent response caching for requests implementing <see cref="ICacheableRequest"/> and
/// automated prefix invalidation for requests implementing <see cref="IInvalidateCacheRequest"/>.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public sealed class CachingPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ICacheProvider? _cacheProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="CachingPipelineBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="cacheProvider">The optional cache provider.</param>
    public CachingPipelineBehavior(ICacheProvider? cacheProvider = null)
    {
        _cacheProvider = cacheProvider;
    }

    /// <inheritdoc/>
    public async ValueTask<TResponse> Handle<TNext>(
        TRequest request,
        TNext next,
        CancellationToken cancellationToken) where TNext : struct, INext<TResponse>
    {
        if (_cacheProvider is null)
        {
            return await next.InvokeAsync().ConfigureAwait(false);
        }

        if (request is ICacheableRequest cacheable)
        {
            var cached = await _cacheProvider.GetAsync<TResponse>(cacheable.CacheKey, cancellationToken).ConfigureAwait(false);
            if (cached.IsSuccess && cached.Value is not null && (cached.Value is not IResultOutcome cachedOutcome || !cachedOutcome.IsUninitialized))
            {
                return cached.Value;
            }

            var response = await next.InvokeAsync().ConfigureAwait(false);

            if (response is IResultOutcome responseOutcome && responseOutcome.IsFailure)
            {
                return response;
            }

            var options = cacheable.Expiration.HasValue
                ? CacheEntryOptions.FromAbsolute(cacheable.Expiration.Value)
                : null;

            await _cacheProvider.SetAsync(cacheable.CacheKey, response, options, cancellationToken).ConfigureAwait(false);
            return response;
        }

        if (request is IInvalidateCacheRequest invalidator)
        {
            var response = await next.InvokeAsync().ConfigureAwait(false);

            if (response is not IResultOutcome outcome || outcome.IsSuccess)
            {
                await _cacheProvider.RemoveByPrefixAsync(invalidator.CachePrefix, cancellationToken).ConfigureAwait(false);
            }

            return response;
        }

        return await next.InvokeAsync().ConfigureAwait(false);
    }
}
