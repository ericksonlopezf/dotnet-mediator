// Copyright © Erickson Lopez. MIT License.
namespace EricksonLopez.Mediator;

/// <summary>
/// Defines a handler for processing a request of type <typeparamref name="TRequest"/> that produces a response of type <see cref="Unit"/>.
/// Provided for migration compatibility with MediatR.
/// </summary>
/// <typeparam name="TRequest">The type of request to process.</typeparam>
public interface IRequestHandler<in TRequest> : IRequestHandler<TRequest, Unit>
    where TRequest : IRequest<Unit>
{
}
