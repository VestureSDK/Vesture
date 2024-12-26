using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Requests
{
    /// <summary>
    /// Defines a <see cref="IRequest{TResponse}"/> handler.
    /// </summary>
    /// <remarks>
    /// For ease of implementation, you should inherit from <see cref="RequestHandler{TRequest, TResponse}"/> instead.
    /// </remarks>
    /// <typeparam name="TRequest">The <see cref="IRequest{TResponse}"/> type.</typeparam>
    /// <typeparam name="TResponse">The response type produced as specified in <typeparamref name="TRequest"/>.</typeparam>
    public interface IRequestHandler<TRequest, TResponse>
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
