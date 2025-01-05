using Crucible.Mediator.Commands;
using Crucible.Mediator.Events;
using Crucible.Mediator.Requests;

namespace Crucible.Mediator.Engine
{
    /// <summary>
    /// Defines a <see cref="InvocationPipeline{TResponse}"/> provider for a <see cref="IRequest{TResponse}"/>, <see cref="ICommand"/> or <see cref="IEvent"/>.
    /// </summary>
    public interface IInvocationPipelineProvider
    {
        /// <summary>
        /// Gets the <see cref="InvocationPipeline{TResponse}"/> for the specified <paramref name="request"/>.
        /// </summary>
        /// <typeparam name="TResponse">The reponse type expected by <paramref name="request"/>.</typeparam>
        /// <param name="request">The <see cref="IRequest{TResponse}"/>, <see cref="ICommand"/> or <see cref="IEvent"/> to get the <see cref="InvocationPipeline{TResponse}"/> for.</param>
        /// <returns>The <see cref="InvocationPipeline{TResponse}"/> for <paramref name="request"/>.</returns>
        InvocationPipeline<TResponse> GetInvocationPipeline<TResponse>(object request);
    }
}
