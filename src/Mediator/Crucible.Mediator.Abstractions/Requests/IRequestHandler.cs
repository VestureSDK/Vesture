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
        /// <param name="request">The request to handle.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the execution.</param>
        /// <returns>The response as expected by the <paramref name="request"/>.</returns>
        Task<TResponse> ExecuteAsync(TRequest request, CancellationToken cancellationToken = default);
    }
}
