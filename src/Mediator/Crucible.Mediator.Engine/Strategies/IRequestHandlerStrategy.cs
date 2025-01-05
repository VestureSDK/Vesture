using Crucible.Mediator.Invocation;
using Crucible.Mediator.Requests;

namespace Crucible.Mediator.Engine.Strategies
{
    /// <summary>
    /// Defines a <see cref="IRequestHandler{TRequest, TResponse}"/> execution strategy.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public interface IRequestHandlerStrategy<TRequest, TResponse>
    {
        /// <summary>
        /// Executes the handling process for the related <typeparamref name="TRequest"/>.
        /// </summary>
        /// <param name="context">The invocation context related to the <typeparamref name="TRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the execution.</param>
        /// <returns>The executing process.</returns>
        Task ExecuteAsync(IInvocationContext<TRequest, TResponse> context, CancellationToken cancellationToken = default);
    }
}
