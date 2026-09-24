// Copyright © Erickson Lopez. MIT License.
namespace EricksonLopez.Mediator;

/// <summary>
/// Defines a marker interface for a request that produces a response of type <see cref="Unit"/>.
/// Provided for migration compatibility with MediatR.
/// </summary>
public interface IRequest : IRequest<Unit>
{
}
