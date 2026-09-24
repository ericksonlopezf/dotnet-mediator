// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.Mediator.Caching;

/// <summary>
/// Specifies caching metadata for a query or request type.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
public sealed class CacheableAttribute : Attribute
{
    /// <summary>
    /// Gets the cache expiration duration in seconds.
    /// </summary>
    public int DurationSeconds { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CacheableAttribute"/> class.
    /// </summary>
    /// <param name="durationSeconds">Expiration duration in seconds (default is 300s = 5 minutes).</param>
    public CacheableAttribute(int durationSeconds = 300)
    {
        DurationSeconds = durationSeconds;
    }
}
