// Copyright © Erickson Lopez. MIT License.
using System;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
using EricksonLopez.Mediator;
using Xunit;

namespace EricksonLopez.Mediator.Tests;

public class UnitTests
{
    private sealed record PingRequest(string Message) : IRequest<string>;
    private sealed record VoidRequest : IRequest;

    private sealed class PingRequestHandler : IRequestHandler<PingRequest, string>
    {
        public ValueTask<string> Handle(PingRequest request, CancellationToken cancellationToken)
            => ValueTask.FromResult($"Pong: {request.Message}");
    }

    private sealed class VoidRequestHandler : IRequestHandler<VoidRequest>
    {
        public ValueTask<Unit> Handle(VoidRequest request, CancellationToken cancellationToken)
            => ValueTask.FromResult(Unit.Value);
    }

    [Fact]
    public void Value_DefaultAccess_ReturnsCanonicalValueAndCorrectStringRepresentation()
    {
        var unit = Unit.Value;
        unit.ToString().Should().Be("()");
    }

    [Fact]
    public void Equals_WithUnitInstances_ReturnsTrue()
    {
        var u1 = Unit.Value;
        var u2 = new Unit();

        u1.Equals(u2).Should().BeTrue();
        u1.Equals((object)u2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithNonUnitObjectOrNull_ReturnsFalse()
    {
        var unit = Unit.Value;

        unit.Equals(null).Should().BeFalse();
        unit.Equals("string").Should().BeFalse();
        unit.Equals(42).Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_AnyInstance_ReturnsZero()
    {
        var u1 = Unit.Value;
        var u2 = new Unit();

        u1.GetHashCode().Should().Be(0);
        u2.GetHashCode().Should().Be(0);
    }

    [Fact]
    public void CompareTo_WithUnitInstance_ReturnsZero()
    {
        var u1 = Unit.Value;
        var u2 = new Unit();

        u1.CompareTo(u2).Should().Be(0);
        ((IComparable)u1).CompareTo(u2).Should().Be(0);
    }

    [Fact]
    public void CompareTo_WithNonUnitObjectOrNull_ReturnsOne()
    {
        IComparable u1 = Unit.Value;

        u1.CompareTo(null).Should().Be(1);
        u1.CompareTo("not-unit").Should().Be(1);
    }

    [Fact]
    public void Operators_EqualityAndRelational_ReturnExpectedBooleans()
    {
        var u1 = Unit.Value;
        var u2 = new Unit();

        (u1 == u2).Should().BeTrue();
        (u1 != u2).Should().BeFalse();
        (u1 < u2).Should().BeFalse();
        (u1 <= u2).Should().BeTrue();
        (u1 > u2).Should().BeFalse();
        (u1 >= u2).Should().BeTrue();
    }

    [Fact]
    public async Task IRequestHandler_TypedRequest_DispatchesSuccessfully()
    {
        var handler = new PingRequestHandler();
        var result = await handler.Handle(new PingRequest("Hello"), CancellationToken.None);

        result.Should().Be("Pong: Hello");
    }

    [Fact]
    public async Task IRequestHandler_VoidRequest_ReturnsUnitValue()
    {
        var handler = new VoidRequestHandler();
        var result = await handler.Handle(new VoidRequest(), CancellationToken.None);

        result.Should().Be(Unit.Value);
    }
}
