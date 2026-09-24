// Copyright © Erickson Lopez. MIT License.
using AwesomeAssertions;
using Xunit;

namespace EricksonLopez.Mediator.Caching.Tests;

public sealed class CacheableAttributeTests
{
    [Fact]
    public void Constructor_DefaultDuration_Sets300Seconds()
    {
        var attr = new CacheableAttribute();
        attr.DurationSeconds.Should().Be(300);
    }

    [Fact]
    public void Constructor_CustomDuration_SetsConfiguredSeconds()
    {
        var attr = new CacheableAttribute(60);
        attr.DurationSeconds.Should().Be(60);
    }
}
