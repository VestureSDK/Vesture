using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Requests
{
    /// <summary>
    /// Defines an invoker for <see cref="IRequest{TResponse}"/>.
    /// </summary>
    /// <remarks>
    /// For simplicity, you should rather use <see cref="IMediator"/> directly.
    /// </remarks>
    public interface IRequestExecutor
    {
        /// <summary>
        /// Executes the specified <paramref name="request"/> and returns the expected <typeparamref name="TResponse"/>.
        /// </summary>
        /// <typeparam name="TResponse">The expected response from <paramref name="request"/>.</typeparam>
        /// <param name="request">The <see cref="IRequest{TResponse}"/> to execute.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the execution.</param>
        /// <returns>The expected <typeparamref name="TResponse"/> value.</returns>
        Task<TResponse> ExecuteAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the specified <paramref name="request"/> and returns the <see cref="IInvocationContext{TResponse}"/> containing
        /// the expected <typeparamref name="TResponse"/> or any <see cref="Exception"/> that might have occured.
        /// </summary>
        /// <typeparam name="TResponse">The expected response from <paramref name="request"/>.</typeparam>
        /// <param name="request">The <see cref="IRequest{TResponse}"/> to execute.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the execution.</param>
        /// <returns>
        /// The <see cref="IInvocationContext{TResponse}"/> containing the expected 
        /// <typeparamref name="TResponse"/> or any <see cref="Exception"/> that might have occured.
        /// </returns>
        Task<IInvocationContext<TResponse>> ExecuteAndCaptureAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
    }
}
