// Copyright © Erickson Lopez. MIT License.
// NOTE: This file is named INextVoid.cs by convention to distinguish it from INext.cs
// which declares the generic INext<TResponse> interface. The public interface declared
// here is INext (non-generic), used exclusively for INotificationBehavior<TNotification> pipelines.
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EricksonLopez.Mediator;

/// <summary>
/// Defines a callback delegate representing the next step in a notification processing pipeline.
/// </summary>
/// <remarks>
/// <para>
/// This is the <strong>non-generic</strong> variant of <c>INext</c>, used as the continuation
/// type for <see cref="INotificationBehavior{TNotification}"/>. It is distinct from
/// <see cref="INext{TResponse}"/>, which is used for command and query pipeline behaviors.
/// </para>
/// <para>
/// This abstraction enables zero-allocation pipeline execution across notification behaviors
/// and handlers by requiring implementations to be <c>readonly struct</c> types, satisfying
/// the <c>where TNext : struct, INext</c> constraint on
/// <see cref="INotificationBehavior{TNotification}"/>.
/// </para>
/// </remarks>
public interface INext
{
    /// <summary>
    /// Invokes the next behavior or handler in the pipeline.
    /// </summary>
    /// <returns>A value task representing the asynchronous operation.</returns>
    ValueTask InvokeAsync();
}
