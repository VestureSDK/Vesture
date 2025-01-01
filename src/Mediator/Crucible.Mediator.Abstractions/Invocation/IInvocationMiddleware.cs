using Crucible.Mediator.Commands;
using Crucible.Mediator.Events;
using Crucible.Mediator.Requests;

namespace Crucible.Mediator.Invocation
{
    /// <summary>
    /// Defines a middleware to intercept <see cref="IRequest{TResponse}"/>, <see cref="ICommand"/> or <see cref="IEvent"/> processing.
    /// </summary>
    public interface IInvocationMiddleware<in TRequest, in TResponse>
    {
        /// <summary>
        /// Executes the middleware process for the related <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The invocation context related to the <see cref="IRequest{TResponse}"/>, <see cref="ICommand"/> or <see cref="IEvent"/>.</param>
        /// <param name="next">The next <see cref="IInvocationMiddleware{TRequest, TResponse}"/> or the underlying handler.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the execution.</param>
        /// <returns>The executing process.</returns>
        Task ExecuteAsync(IInvocationContext<TRequest, TResponse> context, Func<CancellationToken, Task> next, CancellationToken cancellationToken);
    }
}
