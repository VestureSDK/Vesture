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
        /// Defines the order when to execute this <see cref="IInvocationMiddleware{TRequest, TResponse}"/> compare to the other <see cref="IInvocationMiddleware{TRequest, TResponse}"/>.
        /// </summary>
        /// <remarks>
        /// A greater <see cref="Order"/> will make this <see cref="IInvocationMiddleware{TRequest, TResponse}"/> execute closer to the handler (aka later).
        /// </remarks>
        public int? Order { get; set; }

        /// <summary>
        /// Executes the middleware process for the related <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The invocation context related to the <see cref="IRequest{TResponse}"/>, <see cref="ICommand"/> or <see cref="IEvent"/>.</param>
        /// <param name="next">The next <see cref="IInvocationMiddleware{TRequest, TResponse}"/> or the underlying handler.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the execution.</param>
        /// <returns>The executing process.</returns>
        ValueTask ExecuteAsync(IInvocationContext<TRequest, TResponse> context, Func<CancellationToken, ValueTask> next, CancellationToken cancellationToken);
    }
}
