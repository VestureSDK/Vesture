using Vesture.Mediator.Commands;
using Vesture.Mediator.Events;
using Vesture.Mediator.Invocation;
using Vesture.Mediator.Requests;

namespace Vesture.Mediator.Engine.Pipeline.Context
{
    /// <summary>
    /// Defines a factory to create <see cref="IInvocationContext"/> for an <see cref="IInvocationPipeline{TResponse}"/>.
    /// </summary>
    public interface IInvocationContextFactory
    {
        /// <summary>
        /// Creates a new <see cref="IInvocationContext{TRequest, TResponse}"/> from the specified <paramref name="request"/>.
        /// </summary>
        /// <typeparam name="TRequest">The <paramref name="request"/> type.</typeparam>
        /// <typeparam name="TResponse">The expected response type.</typeparam>
        /// <param name="request">The <see cref="IRequest{TResponse}"/>, <see cref="ICommand"/> or <see cref="IEvent"/> to create the context from.</param>
        /// <returns>The created <see cref="IInvocationContext{TRequest, TResponse}"/></returns>
        IInvocationContext<TRequest, TResponse> CreateContextForRequest<TRequest, TResponse>(
            object request
        );
    }
}
