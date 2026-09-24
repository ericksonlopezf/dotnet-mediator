// Copyright © Erickson Lopez. MIT License.
namespace EricksonLopez.Mediator;

/// <summary>
/// Defines a marker interface for a request that produces a response of type <typeparamref name="TResponse"/>.
/// Provided for migration compatibility with MediatR; prefer <see cref="ICommand{TResponse}"/> or <see cref="IQuery{TResponse}"/> in new code.
/// </summary>
/// <typeparam name="TResponse">The type of response produced by the request.</typeparam>
public interface IRequest<TResponse> : ICommand<TResponse>
{
}
