// Copyright © Erickson Lopez. MIT License.
using System;
using Microsoft.Extensions.DependencyInjection;

namespace EricksonLopez.Mediator.Caching;

/// <summary>
/// Provides extension methods for registering caching pipeline behaviors with dependency injection.
/// </summary>
public static class CachingMediatorExtensions
{
    /// <summary>
    /// Registers the open generic <see cref="CachingPipelineBehavior{TRequest, TResponse}"/> with the service collection.
    /// </summary>
    /// <param name="services">The service collection to register the behavior into.</param>
    /// <returns>The same service collection instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null"/></exception>
    public static IServiceCollection AddMediatorCaching(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingPipelineBehavior<,>));
        return services;
    }
}
