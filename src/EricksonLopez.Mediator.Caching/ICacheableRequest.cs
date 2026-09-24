// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.Mediator.Caching;

/// <summary>
/// Defines a request contract that enables transparent response caching within the mediator pipeline.
/// </summary>
public interface ICacheableRequest
{
    /// <summary>
    /// Gets the unique cache key for this request and its parameters.
    /// </summary>
    string CacheKey { get; }

    /// <summary>
    /// Gets the optional custom expiration duration for the cached response.
    /// </summary>
    TimeSpan? Expiration => null;
}
