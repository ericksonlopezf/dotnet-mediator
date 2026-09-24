// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EricksonLopez.Mediator;

/// <summary>
/// Provides static mediator dispatching capabilities without requiring a dependency injection container.
/// </summary>
/// <remarks>
/// <para>This type is thread-safe, <strong>reflection-minimal</strong>, Native AOT compatible, and intended for high-performance,
/// embedded, or constrained runtime environments. The primary dispatch path (<see cref="SendCommand{TCommand, TResponse}"/>
/// and <see cref="SendQuery{TQuery, TResponse}"/>) uses compile-time type keys and performs zero reflection.
/// The polymorphic fallback path (activated when the runtime type of the argument differs from the compile-time type)
/// uses <c>object.GetType()</c> for dictionary lookup — it does NOT use <c>MethodInfo.Invoke</c> or emit dynamic code.
/// See ADR-032 for full details and rationale.</para>
/// <para><strong>Notification Handler Order:</strong> Notification handlers are invoked in the order they were
/// registered (FIFO). This guarantee relies on <see cref="ConcurrentQueue{T}"/> semantics.</para>
/// <para><strong>Cancellation:</strong> All dispatch methods honour a pre-cancelled
/// <see cref="CancellationToken"/> by throwing <see cref="OperationCanceledException"/> before invoking
/// any handler, consistent with the behaviour of the DI-based <c>GeneratedMediator</c>.</para>
/// <para><strong>Scoped handlers:</strong> Unlike the DI-based mediator, <see cref="StaticMediator"/>
/// holds direct references to handler instances. Handlers that hold scoped resources (e.g. a
/// <c>DbContext</c>) must be managed by the caller. Call <see cref="Reset"/> between test cases or
/// application restarts to avoid stale handler references.</para>
/// </remarks>
public static class StaticMediator
{
    private static readonly ConcurrentDictionary<Type, object> CommandHandlers = new();
    private static readonly ConcurrentDictionary<Type, object> QueryHandlers = new();

    // PERF-001: Changed from ConcurrentQueue<object> to object[] snapshot array.
    // ConcurrentQueue.GetEnumerator() allocates a 32-byte enumerator on every Publish call.
    // An immutable array snapshot preserves FIFO ordering, is thread-safe without locks during reads,
    // and produces 0 bytes of heap allocation during Publish loops.
    private static readonly ConcurrentDictionary<Type, object[]> NotificationHandlers = new();

    private interface ICommandInvoker<TResponse>
    {
        ValueTask<TResponse> Invoke(object command, CancellationToken cancellationToken);
    }

    private sealed class CommandInvoker<TCommand, TResponse> : ICommandInvoker<TResponse>
        where TCommand : ICommand<TResponse>
    {
        private readonly ICommandHandler<TCommand, TResponse> _handler;
        public CommandInvoker(ICommandHandler<TCommand, TResponse> handler) => _handler = handler;
        public ValueTask<TResponse> Invoke(object command, CancellationToken cancellationToken) =>
            _handler.Handle((TCommand)command, cancellationToken);
    }

    private interface IQueryInvoker<TResponse>
    {
        ValueTask<TResponse> Invoke(object query, CancellationToken cancellationToken);
    }

    private sealed class QueryInvoker<TQuery, TResponse> : IQueryInvoker<TResponse>
        where TQuery : IQuery<TResponse>
    {
        private readonly IQueryHandler<TQuery, TResponse> _handler;
        public QueryInvoker(IQueryHandler<TQuery, TResponse> handler) => _handler = handler;
        public ValueTask<TResponse> Invoke(object query, CancellationToken cancellationToken) =>
            _handler.Handle((TQuery)query, cancellationToken);
    }

    /// <summary>
    /// Registers a command handler instance for static dispatching.
    /// </summary>
    /// <typeparam name="TCommand">The type of command handled.</typeparam>
    /// <typeparam name="TResponse">The type of response produced by the command.</typeparam>
    /// <param name="handler">The command handler instance to register.</param>
    /// <exception cref="ArgumentNullException"><paramref name="handler"/> is <see langword="null"/></exception>
    /// <remarks>
    /// If a handler for <typeparamref name="TCommand"/> is already registered, it is silently replaced.
    /// This is intentional to support re-registration scenarios in tests; call <see cref="Reset"/> between
    /// test cases for clean isolation.
    /// </remarks>
    public static void RegisterCommandHandler<TCommand, TResponse>(ICommandHandler<TCommand, TResponse> handler)
        where TCommand : ICommand<TResponse>
    {
        ArgumentNullException.ThrowIfNull(handler);
        CommandHandlers[typeof(TCommand)] = new CommandInvoker<TCommand, TResponse>(handler);
    }

    /// <summary>
    /// Registers a query handler instance for static dispatching.
    /// </summary>
    /// <typeparam name="TQuery">The type of query handled.</typeparam>
    /// <typeparam name="TResponse">The type of response produced by the query.</typeparam>
    /// <param name="handler">The query handler instance to register.</param>
    /// <exception cref="ArgumentNullException"><paramref name="handler"/> is <see langword="null"/></exception>
    public static void RegisterQueryHandler<TQuery, TResponse>(IQueryHandler<TQuery, TResponse> handler)
        where TQuery : IQuery<TResponse>
    {
        ArgumentNullException.ThrowIfNull(handler);
        QueryHandlers[typeof(TQuery)] = new QueryInvoker<TQuery, TResponse>(handler);
    }

    /// <summary>
    /// Registers a notification handler instance for static dispatching.
    /// </summary>
    /// <typeparam name="TNotification">The type of notification handled.</typeparam>
    /// <param name="handler">The notification handler instance to register.</param>
    /// <exception cref="ArgumentNullException"><paramref name="handler"/> is <see langword="null"/></exception>
    /// <remarks>
    /// Multiple handlers may be registered for the same notification type. They are invoked in
    /// <strong>registration order</strong> (FIFO) when <see cref="Publish{TNotification}"/> is called.
    /// </remarks>
    public static void RegisterNotificationHandler<TNotification>(INotificationHandler<TNotification> handler)
        where TNotification : INotification
    {
        ArgumentNullException.ThrowIfNull(handler);
        NotificationHandlers.AddOrUpdate(
            typeof(TNotification),
            _ => new object[] { handler },
            (_, existing) =>
            {
                var updated = new object[existing.Length + 1];
                Array.Copy(existing, updated, existing.Length);
                updated[existing.Length] = handler;
                return updated;
            });
    }

    /// <summary>
    /// Dispatches a strongly typed command directly to its registered static handler without reflection.
    /// </summary>
    /// <typeparam name="TCommand">The concrete type of the command.</typeparam>
    /// <typeparam name="TResponse">The type of response returned by the command handler.</typeparam>
    /// <param name="command">The command instance to dispatch.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>
    /// A value task representing the asynchronous operation that yields the response from the handler.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="command"/> is <see langword="null"/></exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> is already cancelled</exception>
    /// <exception cref="InvalidOperationException">No static command handler is registered for <typeparamref name="TCommand"/></exception>
    /// <remarks>
    /// <para><strong>Polymorphic Dispatch:</strong> Dispatch is keyed by the compile-time type <typeparamref name="TCommand"/> (ADR-032).
    /// If invoking via an interface reference (e.g. <c>ICommand&lt;TResponse&gt;</c>), the type must match the registration
    /// or be dispatched via the DI-based <c>IMediator</c> which generates exhaustive runtime type switches.</para>
    /// </remarks>
    public static ValueTask<TResponse> SendCommand<TCommand, TResponse>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TResponse>
    {
        // FIX CONC-002: Check cancellation before dispatch, consistent with GeneratedMediator behaviour.
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(command);

        if (CommandHandlers.TryGetValue(typeof(TCommand), out var handlerObj) &&
            handlerObj is ICommandInvoker<TResponse> invoker)
        {
            return invoker.Invoke(command, cancellationToken);
        }

        var runtimeCommandType = command.GetType();
        if (runtimeCommandType != typeof(TCommand) &&
            CommandHandlers.TryGetValue(runtimeCommandType, out handlerObj) &&
            handlerObj is ICommandInvoker<TResponse> runtimeInvoker)
        {
            return runtimeInvoker.Invoke(command, cancellationToken);
        }

        throw new InvalidOperationException($"No static command handler registered for {typeof(TCommand).FullName}");
    }

    /// <summary>
    /// Dispatches a strongly typed query directly to its registered static handler without reflection.
    /// </summary>
    /// <typeparam name="TQuery">The concrete type of the query.</typeparam>
    /// <typeparam name="TResponse">The type of response returned by the query handler.</typeparam>
    /// <param name="query">The query instance to dispatch.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>
    /// A value task representing the asynchronous operation that yields the response from the handler.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="query"/> is <see langword="null"/></exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> is already cancelled</exception>
    /// <exception cref="InvalidOperationException">No static query handler is registered for <typeparamref name="TQuery"/></exception>
    public static ValueTask<TResponse> SendQuery<TQuery, TResponse>(TQuery query, CancellationToken cancellationToken = default)
        where TQuery : IQuery<TResponse>
    {
        // FIX CONC-002: Check cancellation before dispatch, consistent with GeneratedMediator behaviour.
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(query);

        if (QueryHandlers.TryGetValue(typeof(TQuery), out var handlerObj) &&
            handlerObj is IQueryInvoker<TResponse> invoker)
        {
            return invoker.Invoke(query, cancellationToken);
        }

        var runtimeQueryType = query.GetType();
        if (runtimeQueryType != typeof(TQuery) &&
            QueryHandlers.TryGetValue(runtimeQueryType, out handlerObj) &&
            handlerObj is IQueryInvoker<TResponse> runtimeInvoker)
        {
            return runtimeInvoker.Invoke(query, cancellationToken);
        }

        throw new InvalidOperationException($"No static query handler registered for {typeof(TQuery).FullName}");
    }


    /// <summary>
    /// Publishes a notification to all registered static handlers in registration order (FIFO).
    /// </summary>
    /// <typeparam name="TNotification">The type of notification being published.</typeparam>
    /// <param name="notification">The notification instance to publish.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A value task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="notification"/> is <see langword="null"/></exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> is already cancelled</exception>
    /// <remarks>
    /// Handlers are invoked sequentially in the order they were registered. If a handler throws an
    /// exception, subsequent handlers will <strong>not</strong> be invoked and the exception propagates
    /// to the caller. To invoke all handlers regardless of individual failures and collect all exceptions,
    /// use the DI-based <c>GeneratedMediator</c> with the
    /// <c>[PublishStrategy(PublishStrategy.SequentialAggregateExceptions)]</c> attribute.
    /// </remarks>
    public static async ValueTask Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        // FIX CONC-002: Check cancellation before dispatch, consistent with GeneratedMediator behaviour.
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(notification);

        if (NotificationHandlers.TryGetValue(typeof(TNotification), out var handlers))
        {
            // PERF-001: Zero-allocation indexed loop over array snapshot in strict FIFO order
            for (int i = 0; i < handlers.Length; i++)
            {
                if (handlers[i] is INotificationHandler<TNotification> handler)
                {
                    await handler.Handle(notification, cancellationToken).ConfigureAwait(false);
                }
            }
        }
    }

    /// <summary>
    /// Removes all statically registered command, query, and notification handlers.
    /// </summary>
    /// <remarks>
    /// Call this method between test cases or application restarts to ensure clean handler state.
    /// </remarks>
    public static void Reset()
    {
        CommandHandlers.Clear();
        QueryHandlers.Clear();
        NotificationHandlers.Clear();
    }
}
