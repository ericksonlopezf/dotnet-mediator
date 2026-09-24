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

public sealed record TestCacheableQueryNoExpiration(string QueryId) : IQuery<Result<string>>, ICacheableRequest
{
    public string CacheKey => $"query-no-exp:{QueryId}";
    public TimeSpan? Expiration => null;
}

public sealed record TestCacheablePlainQuery(string QueryId) : IQuery<string>, ICacheableRequest
{
    public string CacheKey => $"plain-query:{QueryId}";
    public TimeSpan? Expiration => TimeSpan.FromMinutes(2);
}

public sealed record TestPlainRequest : ICommand<string>;

public sealed record TestInvalidatePlainCommand(string CachePrefix) : ICommand<string>, IInvalidateCacheRequest;

public sealed class CachingPipelineBehaviorTests
{
    private readonly ICacheProvider _cacheProvider = Substitute.For<ICacheProvider>();

    [Fact]
    public async Task Handle_NullCacheProvider_InvokesNextDirectly()
    {
        var behavior = new CachingPipelineBehavior<TestCacheableQuery, Result<string>>(null);
        var query = new TestCacheableQuery("123");
        var nextExecuted = false;
        var next = new TestNextContinuation<Result<string>>(() =>
        {
            nextExecuted = true;
            return ValueTask.FromResult(Result<string>.Success("DIRECT"));
        });

        var result = await behavior.Handle(query, next, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("DIRECT");
        nextExecuted.Should().BeTrue();
    }

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
    public async Task Handle_CacheableRequestWithoutExpiration_SetsNullOptions()
    {
        var behavior = new CachingPipelineBehavior<TestCacheableQueryNoExpiration, Result<string>>(_cacheProvider);
        var query = new TestCacheableQueryNoExpiration("no-exp");
        var next = new TestNextContinuation<Result<string>>(() => ValueTask.FromResult(Result<string>.Success("NO_EXP_DATA")));

        _cacheProvider.GetAsync<Result<string>>("query-no-exp:no-exp", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<Result<string>>.Success(default)));

        var result = await behavior.Handle(query, next, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("NO_EXP_DATA");
        await _cacheProvider.Received(1).SetAsync(
            "query-no-exp:no-exp",
            Arg.Is<Result<string>>(r => r.Value == "NO_EXP_DATA"),
            null,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CacheGetFailure_ExecutesNextAndCaches()
    {
        var behavior = new CachingPipelineBehavior<TestCacheableQuery, Result<string>>(_cacheProvider);
        var query = new TestCacheableQuery("err-key");
        var next = new TestNextContinuation<Result<string>>(() => ValueTask.FromResult(Result<string>.Success("FALLBACK_DATA")));

        // GetAsync returns a failure result
        _cacheProvider.GetAsync<Result<string>>("query:err-key", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<Result<string>>.Failure(Error.Failure("CacheError", "Failed"))));

        var result = await behavior.Handle(query, next, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("FALLBACK_DATA");
        await _cacheProvider.Received(1).SetAsync(
            "query:err-key",
            Arg.Is<Result<string>>(r => r.Value == "FALLBACK_DATA"),
            Arg.Any<CacheEntryOptions?>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CachedOutcomeIsUninitialized_ExecutesNext()
    {
        var behavior = new CachingPipelineBehavior<TestCacheableQuery, Result<string>>(_cacheProvider);
        var query = new TestCacheableQuery("uninit");
        var next = new TestNextContinuation<Result<string>>(() => ValueTask.FromResult(Result<string>.Success("FRESH")));

        // Default uninitialized Result struct has IsUninitialized == true
        Result<string> uninitializedResult = default;
        _cacheProvider.GetAsync<Result<string>>("query:uninit", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<Result<string>>.Success(uninitializedResult)));

        var result = await behavior.Handle(query, next, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("FRESH");
    }

    [Fact]
    public async Task Handle_PlainCacheableQuery_CachesResponse()
    {
        var behavior = new CachingPipelineBehavior<TestCacheablePlainQuery, string>(_cacheProvider);
        var query = new TestCacheablePlainQuery("plain-1");
        var next = new TestNextContinuation<string>(() => ValueTask.FromResult("PLAIN_VALUE"));

        _cacheProvider.GetAsync<string>("plain-query:plain-1", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<string?>.Failure(Error.NotFound("KeyNotFound", "Not found"))));

        var result = await behavior.Handle(query, next, CancellationToken.None);

        result.Should().Be("PLAIN_VALUE");
        await _cacheProvider.Received(1).SetAsync(
            "plain-query:plain-1",
            "PLAIN_VALUE",
            Arg.Is<CacheEntryOptions>(o => o.AbsoluteExpirationRelativeToNow == TimeSpan.FromMinutes(2)),
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

    [Fact]
    public async Task Handle_InvalidateCacheRequest_WhenFailedResult_DoesNotRemovePrefix()
    {
        var behavior = new CachingPipelineBehavior<TestInvalidateCommand, Result<bool>>(_cacheProvider);
        var command = new TestInvalidateCommand("B");
        var next = new TestNextContinuation<Result<bool>>(() => ValueTask.FromResult(Result<bool>.Failure(Error.Conflict("Conflict", "Error"))));

        var result = await behavior.Handle(command, next, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        await _cacheProvider.DidNotReceive().RemoveByPrefixAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_InvalidateCacheRequest_WhenNonResultOutcome_RemovesPrefix()
    {
        var behavior = new CachingPipelineBehavior<TestInvalidatePlainCommand, string>(_cacheProvider);
        var command = new TestInvalidatePlainCommand("plain-prefix:");
        var next = new TestNextContinuation<string>(() => ValueTask.FromResult("DONE"));

        var result = await behavior.Handle(command, next, CancellationToken.None);

        result.Should().Be("DONE");
        await _cacheProvider.Received(1).RemoveByPrefixAsync("plain-prefix:", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NonCacheableNonInvalidateRequest_CallsNextDirectly()
    {
        var behavior = new CachingPipelineBehavior<TestPlainRequest, string>(_cacheProvider);
        var command = new TestPlainRequest();
        var nextCalled = false;
        var next = new TestNextContinuation<string>(() =>
        {
            nextCalled = true;
            return ValueTask.FromResult("PLAIN_RESPONSE");
        });

        var result = await behavior.Handle(command, next, CancellationToken.None);

        result.Should().Be("PLAIN_RESPONSE");
        nextCalled.Should().BeTrue();
        await _cacheProvider.DidNotReceive().GetAsync<string>(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _cacheProvider.DidNotReceive().RemoveByPrefixAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }
}
