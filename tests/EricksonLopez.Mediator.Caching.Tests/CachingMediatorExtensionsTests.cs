// Copyright © Erickson Lopez. MIT License.
using System;
using System.Linq;
using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EricksonLopez.Mediator.Caching.Tests;

public sealed class CachingMediatorExtensionsTests
{
    [Fact]
    public void AddMediatorCaching_NullServices_ThrowsArgumentNullException()
    {
        IServiceCollection services = null!;
        var act = () => services.AddMediatorCaching();
        act.Should().Throw<ArgumentNullException>().WithParameterName("services");
    }

    [Fact]
    public void AddMediatorCaching_ValidServices_RegistersTransientPipelineBehavior()
    {
        var services = new ServiceCollection();
        var returned = services.AddMediatorCaching();

        returned.Should().BeSameAs(services);

        var descriptor = services.FirstOrDefault(d =>
            d.ServiceType == typeof(IPipelineBehavior<,>) &&
            d.ImplementationType == typeof(CachingPipelineBehavior<,>));

        descriptor.Should().NotBeNull();
        descriptor!.Lifetime.Should().Be(ServiceLifetime.Transient);
    }
}
