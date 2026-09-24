// Copyright © Erickson Lopez. MIT License.
using System;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
using EricksonLopez.Caching;
using EricksonLopez.Result;
using NSubstitute;
using Xunit;

namespace EricksonLopez.Mediator.Caching.Tests;

public sealed class CachingPipelineBehaviorTests
{
    private readonly ICacheProvider _cacheProvider = Substitute.For<ICacheProvider>();

    [Fact]
    public async Task Handle_CacheHit_ReturnsCachedResponseWithoutExecutingNext()
    {
        var behavior = new CachingPipelineBehavior<TestCacheableQuery, Result<string>>(_cacheProvider);
        var query = new TestCacheableQuery("123");
        var nextExecuted = false;
        var next = new TestNextContinuation<Result<string>>(() =>
        {
            nextExecuted = true;
            return ValueTask.FromResult(Result<string>.Success("DB_VALUE"));
        });

        _cacheProvider.GetAsync<Result<string>>("query:123", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<Result<string>>.Success(Result<string>.Success("CACHED_VALUE"))));

        var result = await behavior.Handle(query, next, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("CACHED_VALUE");
        nextExecuted.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_CacheMiss_ExecutesNextAndCachesResponse()
    {
        var behavior = new CachingPipelineBehavior<TestCacheableQuery, Result<string>>(_cacheProvider);
        var query = new TestCacheableQuery("456");
        var next = new TestNextContinuation<Result<string>>(() => ValueTask.FromResult(Result<string>.Success("NEW_DATA")));

        _cacheProvider.GetAsync<Result<string>>("query:456", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<Result<string>>.Success(default)));

        var result = await behavior.Handle(query, next, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("NEW_DATA");
        await _cacheProvider.Received(1).SetAsync(
            "query:456",
            Arg.Is<Result<string>>(r => r.Value == "NEW_DATA"),
            Arg.Is<CacheEntryOptions>(o => o.AbsoluteExpirationRelativeToNow == TimeSpan.FromMinutes(5)),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_FunctionalFailure_DoesNotCacheResponse()
    {
        var behavior = new CachingPipelineBehavior<TestCacheableQuery, Result<string>>(_cacheProvider);
        var query = new TestCacheableQuery("789");
        var next = new TestNextContinuation<Result<string>>(() => ValueTask.FromResult(Result<string>.Failure(Error.NotFound("NotFound", "User not found"))));

        _cacheProvider.GetAsync<Result<string>>("query:789", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<Result<string>>.Success(default)));

        var result = await behavior.Handle(query, next, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        await _cacheProvider.DidNotReceive().SetAsync(
            Arg.Any<string>(),
            Arg.Any<Result<string>>(),
            Arg.Any<CacheEntryOptions?>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_InvalidateCacheRequest_RemovesPrefixOnSuccess()
    {
        var behavior = new CachingPipelineBehavior<TestInvalidateCommand, Result<bool>>(_cacheProvider);
        var command = new TestInvalidateCommand("A");
        var next = new TestNextContinuation<Result<bool>>(() => ValueTask.FromResult(Result<bool>.Success(true)));

        var result = await behavior.Handle(command, next, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _cacheProvider.Received(1).RemoveByPrefixAsync("tenant:A:", Arg.Any<CancellationToken>());
    }
}
