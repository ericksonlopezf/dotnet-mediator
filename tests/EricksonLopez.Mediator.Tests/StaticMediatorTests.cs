// Copyright © Erickson Lopez. MIT License.
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
using EricksonLopez.Mediator;
using EricksonLopez.Mediator.Tests.Fixtures;
using Xunit;

namespace EricksonLopez.Mediator.Tests;

[Collection("StaticMediator")]
public class StaticMediatorTests
{
    [Fact]
    public async Task SendCommand_RegisteredCommandWithoutDI_DispatchesSuccessfully()
    {
        StaticMediator.Reset();
        StaticMediator.RegisterCommandHandler(new StaticPingCommandHandler());

        var resultTyped = await StaticMediator.SendCommand<StaticPingCommand, string>(
            new StaticPingCommand("World"), CancellationToken.None);
        resultTyped.Should().Be("StaticPong: World");
    }

    [Fact]
    public async Task SendQuery_RegisteredQueryWithoutDI_DispatchesSuccessfully()
    {
        StaticMediator.Reset();
        StaticMediator.RegisterQueryHandler(new StaticGetQueryHandler());

        var resultTyped = await StaticMediator.SendQuery<StaticGetQuery, int>(
            new StaticGetQuery(7), CancellationToken.None);
        resultTyped.Should().Be(70);
    }

    [Fact]
    public async Task Publish_RegisteredNotificationWithoutDI_DispatchesSuccessfully()
    {
        StaticMediator.Reset();
        var handler = new StaticNotificationHandler();
        StaticMediator.RegisterNotificationHandler(handler);

        await StaticMediator.Publish(new StaticNotification("OrderCreated"), CancellationToken.None);
        handler.HandledCount.Should().Be(1);
    }

    [Fact]
    public async Task Publish_MultipleNotificationHandlers_DispatchesToAll()
    {
        StaticMediator.Reset();
        var handler1 = new StaticNotificationHandler();
        var handler2 = new StaticNotificationHandler();

        StaticMediator.RegisterNotificationHandler(handler1);
        StaticMediator.RegisterNotificationHandler(handler2);

        await StaticMediator.Publish(new StaticNotification("MultiEvent"), CancellationToken.None);
        handler1.HandledCount.Should().Be(1);
        handler2.HandledCount.Should().Be(1);
    }

    [Fact]
    public async Task Publish_NoHandlersRegistered_DoesNotThrow()
    {
        StaticMediator.Reset();
        var act = async () => await StaticMediator.Publish(new StaticNotification("NoHandlerEvent"), CancellationToken.None);
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public void RegisterCommandHandler_NullHandler_ThrowsArgumentNullException()
    {
        var act = () => StaticMediator.RegisterCommandHandler<StaticPingCommand, string>(null!);
        act.Should().Throw<ArgumentNullException>().WithParameterName("handler");
    }

    [Fact]
    public void RegisterQueryHandler_NullHandler_ThrowsArgumentNullException()
    {
        var act = () => StaticMediator.RegisterQueryHandler<StaticGetQuery, int>(null!);
        act.Should().Throw<ArgumentNullException>().WithParameterName("handler");
    }

    [Fact]
    public void RegisterNotificationHandler_NullHandler_ThrowsArgumentNullException()
    {
        var act = () => StaticMediator.RegisterNotificationHandler<StaticNotification>(null!);
        act.Should().Throw<ArgumentNullException>().WithParameterName("handler");
    }

    [Fact]
    public async Task Publish_WithCancellationToken_PropagatesTokenToHandler()
    {
        StaticMediator.Reset();
        var handler = new StaticCancellableNotificationHandler();
        StaticMediator.RegisterNotificationHandler(handler);
        using var cts = new CancellationTokenSource();

        await StaticMediator.Publish(new StaticNotification("CancelCheck"), cts.Token);

        handler.ReceivedToken.Should().Be(cts.Token);
    }

    // FIX CONC-002: A pre-cancelled token now throws OperationCanceledException BEFORE handler invocation.
    // This test verifies the new behaviour matches GeneratedMediator's ThrowIfCancellationRequested contract.
    [Fact]
    public async Task Publish_WithPreCancelledCancellationToken_ThrowsOperationCanceledException()
    {
        StaticMediator.Reset();
        var handler = new StaticCancellableNotificationHandler();
        StaticMediator.RegisterNotificationHandler(handler);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // CONC-002: Should throw before reaching the handler
        var act = async () => await StaticMediator.Publish(new StaticNotification("PreCancelledCheck"), cts.Token).AsTask();
        await act.Should().ThrowAsync<OperationCanceledException>();

        // Handler was never invoked — if it had been called with the pre-cancelled token,
        // IsCancellationRequested would be true. We verify this property because CancellationToken
        // struct equality is unreliable in assertions (WaitHandle access creates OS handles that
        // change structural equality even for logically equivalent tokens).
        handler.ReceivedToken.IsCancellationRequested.Should().BeFalse(
            "handler should not be called when token is pre-cancelled (CONC-002)");
    }

    [Fact]
    public async Task Send_NullRequests_ThrowsArgumentNullException()
    {
        var act1 = async () => await StaticMediator.SendCommand<StaticPingCommand, string>(null!).AsTask();
        await act1.Should().ThrowAsync<ArgumentNullException>();

        var act2 = async () => await StaticMediator.SendQuery<StaticGetQuery, int>(null!).AsTask();
        await act2.Should().ThrowAsync<ArgumentNullException>();

        var act3 = async () => await StaticMediator.Publish<StaticNotification>(null!).AsTask();
        await act3.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task SendCommand_UnregisteredCommand_ThrowsInvalidOperationException()
    {
        StaticMediator.Reset();
        var act = async () => await StaticMediator.SendCommand<StaticPingCommand, string>(new StaticPingCommand("Fail")).AsTask();
        var ex = await act.Should().ThrowAsync<InvalidOperationException>();
        ex.Which.Message.Should().Contain(typeof(StaticPingCommand).FullName!);
        ex.Which.Message.Should().Contain("No static command handler registered for");
    }

    [Fact]
    public async Task SendQuery_UnregisteredQuery_ThrowsInvalidOperationException()
    {
        StaticMediator.Reset();
        var act = async () => await StaticMediator.SendQuery<StaticGetQuery, int>(new StaticGetQuery(99)).AsTask();
        var ex = await act.Should().ThrowAsync<InvalidOperationException>();
        ex.Which.Message.Should().Contain(typeof(StaticGetQuery).FullName!);
        ex.Which.Message.Should().Contain("No static query handler registered for");
    }

    [Fact]
    public async Task Reset_WhenInvoked_ClearsAllHandlers()
    {
        var handler = new StaticNotificationHandler();
        StaticMediator.RegisterCommandHandler(new StaticPingCommandHandler());
        StaticMediator.RegisterQueryHandler(new StaticGetQueryHandler());
        StaticMediator.RegisterNotificationHandler(handler);

        // Reset
        StaticMediator.Reset();

        // Verify cleared
        var actCmd = async () => await StaticMediator.SendCommand<StaticPingCommand, string>(new StaticPingCommand("PostReset")).AsTask();
        await actCmd.Should().ThrowAsync<InvalidOperationException>();

        var actQry = async () => await StaticMediator.SendQuery<StaticGetQuery, int>(new StaticGetQuery(3)).AsTask();
        await actQry.Should().ThrowAsync<InvalidOperationException>();

        await StaticMediator.Publish(new StaticNotification("PostReset"));
        handler.HandledCount.Should().Be(0); // not incremented
    }

    [Fact]
    public async Task Send_ConcurrentDispatch_IsThreadSafe()
    {
        StaticMediator.Reset();
        StaticMediator.RegisterCommandHandler(new StaticPingCommandHandler());
        StaticMediator.RegisterQueryHandler(new StaticGetQueryHandler());

        var timeout = TimeSpan.FromSeconds(10);
        using var cts = new CancellationTokenSource(timeout);

        var tasks = Enumerable.Range(0, 100).Select(async i =>
        {
            var cmdResult = await StaticMediator.SendCommand<StaticPingCommand, string>(
                new StaticPingCommand($"Msg_{i}"), cts.Token);
            var qryResult = await StaticMediator.SendQuery<StaticGetQuery, int>(
                new StaticGetQuery(i), cts.Token);

            cmdResult.Should().Be($"StaticPong: Msg_{i}");
            qryResult.Should().Be(i * 10);
        });
        var allTasks = Task.WhenAll(tasks);
        var completedTask = await Task.WhenAny(allTasks, Task.Delay(timeout, CancellationToken.None));

        completedTask.Should().Be(allTasks, "concurrent dispatch should complete within 10 seconds without deadlocks");
        await allTasks; // propagate any exceptions
    }

    // ────────────────────────────────────────────────────────────────────────────────
    // REGRESSION TESTS for CONC-001, CONC-002, CONC-003 audit fixes
    // ────────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// CONC-001: Notification handlers must execute in FIFO registration order.
    /// The fix changed ConcurrentBag (non-deterministic) to ConcurrentQueue (strict FIFO).
    /// </summary>
    [Fact]
    public async Task Publish_MultipleHandlers_ExecutedInFifoRegistrationOrder()
    {
        StaticMediator.Reset();
        var order = new System.Collections.Concurrent.ConcurrentQueue<int>();

        StaticMediator.RegisterNotificationHandler(new OrderedNotificationHandler(1, order));
        StaticMediator.RegisterNotificationHandler(new OrderedNotificationHandler(2, order));
        StaticMediator.RegisterNotificationHandler(new OrderedNotificationHandler(3, order));

        await StaticMediator.Publish(new StaticNotification("OrderTest"), CancellationToken.None);

        var executionOrder = order.ToArray();
        executionOrder.Should().Equal([1, 2, 3], "handlers must execute in FIFO registration order (CONC-001 fix)");
        StaticMediator.Reset();
    }

    /// <summary>
    /// CONC-002: SendCommand with pre-cancelled token throws OperationCanceledException before dispatch.
    /// </summary>
    [Fact]
    public async Task SendCommand_PreCancelledToken_ThrowsBeforeHandlerInvocation()
    {
        StaticMediator.Reset();
        // Register the standard handler — it WOULD return a value if reached.
        // With pre-cancellation, it must NOT be reached at all.
        StaticMediator.RegisterCommandHandler(new StaticPingCommandHandler());
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var act = async () => await StaticMediator.SendCommand<StaticPingCommand, string>(
            new StaticPingCommand("Test"), cts.Token).AsTask();

        // CONC-002: The exception proves ThrowIfCancellationRequested() fired before handler dispatch.
        await act.Should().ThrowAsync<OperationCanceledException>("CONC-002: pre-cancelled token must throw before dispatch");
        StaticMediator.Reset();
    }

    /// <summary>
    /// CONC-002: SendQuery with pre-cancelled token throws OperationCanceledException before dispatch.
    /// </summary>
    [Fact]
    public async Task SendQuery_PreCancelledToken_ThrowsBeforeHandlerInvocation()
    {
        StaticMediator.Reset();
        // Register the standard handler — it WOULD return a value if reached.
        // With pre-cancellation, it must NOT be reached at all.
        StaticMediator.RegisterQueryHandler(new StaticGetQueryHandler());
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var act = async () => await StaticMediator.SendQuery<StaticGetQuery, int>(
            new StaticGetQuery(42), cts.Token).AsTask();

        // CONC-002: The exception proves ThrowIfCancellationRequested() fired before handler dispatch.
        await act.Should().ThrowAsync<OperationCanceledException>("CONC-002: pre-cancelled token must throw before dispatch");
        StaticMediator.Reset();
    }

    /// <summary>
    /// CONC-003: If the first notification handler throws, subsequent handlers are not called.
    /// This is documented behaviour for StaticMediator (unlike DI-based SequentialAggregateExceptions strategy).
    /// </summary>
    [Fact]
    public async Task Publish_FirstHandlerThrows_SubsequentHandlersNotInvoked()
    {
        StaticMediator.Reset();
        var handler2 = new StaticNotificationHandler();
        StaticMediator.RegisterNotificationHandler(new ThrowingNotificationHandler());
        StaticMediator.RegisterNotificationHandler(handler2);

        var act = async () => await StaticMediator.Publish(new StaticNotification("ExceptionTest"), CancellationToken.None).AsTask();
        await act.Should().ThrowAsync<InvalidOperationException>("first handler throws");

        // CONC-003: Second handler was NOT called — this is documented behaviour
        handler2.HandledCount.Should().Be(0, "CONC-003: exception in first handler stops propagation to subsequent handlers");
        StaticMediator.Reset();
    }
}

// ─────────────────────────────────────────────────────────────────────────────────
// Test helpers for regression tests added during MEGA-AUDIT
// ─────────────────────────────────────────────────────────────────────────────────
// IMPORTANT: These helpers must NOT implement ICommandHandler<StaticPingCommand,string>
// or IQueryHandler<StaticGetQuery,int> directly — that would register a second handler
// for those types, causing ELM002/ELM003 (duplicate handler) from the source generator.
// See: the CONC-002 tests use StaticPingCommandHandler/StaticGetQueryHandler directly.

internal sealed class OrderedNotificationHandler(int index, System.Collections.Concurrent.ConcurrentQueue<int> order)
    : INotificationHandler<StaticNotification>
{
    public ValueTask Handle(StaticNotification notification, CancellationToken cancellationToken)
    {
        order.Enqueue(index);
        return ValueTask.CompletedTask;
    }
}

internal sealed class ThrowingNotificationHandler : INotificationHandler<StaticNotification>
{
    public ValueTask Handle(StaticNotification notification, CancellationToken cancellationToken)
        => throw new InvalidOperationException("Deliberate failure in first handler");
}
