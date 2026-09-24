// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.Mediator;

/// <summary>
/// Specifies the service lifetime of the decorated handler within the dependency injection container.
/// </summary>
/// <remarks>
/// <para>
/// When this attribute is omitted, the source generator registers the handler with
/// <see cref="HandlerLifetime.Transient"/> lifetime by default.
/// </para>
/// <para>
/// This lifetime controls the handler registration only. The mediator interfaces
/// (<c>IMediator</c>, <c>ISender</c>, and <c>IPublisher</c>) are registered separately as
/// <c>Scoped</c> by default via <c>AddEricksonLopezMediator()</c> (ADR-037), which is
/// independent of the handler registration lifetime configured here.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class ServiceLifetimeAttribute : Attribute
{
    /// <summary>
    /// Gets the configured lifetime for the handler registration.
    /// </summary>
    public HandlerLifetime Lifetime { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceLifetimeAttribute"/> class
    /// with the specified service lifetime.
    /// </summary>
    /// <param name="lifetime">The service lifetime to apply to the handler registration.</param>
    public ServiceLifetimeAttribute(HandlerLifetime lifetime)
    {
        Lifetime = lifetime;
    }
}
