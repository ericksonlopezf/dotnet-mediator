// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.Result;

namespace EricksonLopez.Mediator.Caching.Tests;

public sealed record TestCacheableQuery(string QueryId) : IQuery<Result<string>>, ICacheableRequest
{
    public string CacheKey => $"query:{QueryId}";
    public TimeSpan? Expiration => TimeSpan.FromMinutes(5);
}
