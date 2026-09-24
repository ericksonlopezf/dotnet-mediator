// Copyright © Erickson Lopez. MIT License.
using System.Threading;
using System.Threading.Tasks;

namespace EricksonLopez.Mediator;

/// <summary>
/// Defines a handler for processing a request of type <typeparamref name="TRequest"/> that produces a response of type <typeparamref name="TResponse"/>.
/// Provided for migration compatibility with MediatR.
/// </summary>
/// <typeparam name="TRequest">The type of request to process.</typeparam>
/// <typeparam name="TResponse">The type of response produced by the handler.</typeparam>
public interface IRequestHandler<in TRequest, TResponse>
{
    /// <summary>
    /// Processes the specified request.
    /// </summary>
    /// <param name="request">The request instance to process.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>
    /// A value task representing the asynchronous operation that yields the response produced by the request.
    /// </returns>
    ValueTask<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}
