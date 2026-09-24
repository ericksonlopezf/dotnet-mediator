// Copyright © Erickson Lopez. MIT License.

namespace EricksonLopez.Mediator.Caching;

/// <summary>
/// Defines a request contract that triggers cache prefix invalidation upon successful completion.
/// </summary>
public interface IInvalidateCacheRequest
{
    /// <summary>
    /// Gets the cache key prefix to invalidate upon successful execution.
    /// </summary>
    string CachePrefix { get; }
}
