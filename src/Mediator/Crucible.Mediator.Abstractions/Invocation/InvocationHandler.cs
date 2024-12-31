using Crucible.Mediator.Requests;

namespace Crucible.Mediator.Invocation
{
    /// <summary>
    /// Base implementation of an invocation handler.
    /// </summary>
    /// <typeparam name="TRequest">The request type.</typeparam>
    /// <typeparam name="TNoResponse">The <see cref="NoResponse"/> produced as specified in <typeparamref name="TRequest"/>.</typeparam>
    public abstract class NoResponseInvocationHandler<TRequest, TNoResponse> : IRequestHandler<TRequest, TNoResponse>
        where TNoResponse : NoResponse
    {
        /// <summary>
        /// Executes the handling process for the related <typeparamref name="TRequest"/>.
        /// </summary>
        /// <remarks>
        /// This is the internal implementation of <see cref="ExecuteAsync(TRequest, CancellationToken)"/>.
        /// It is implicitely surrounded by <c>try { } catch</c> and can safely <c>throw</c>.
        /// </remarks>
        /// <param name="request">The <typeparamref name="TRequest"/> to handle.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the execution.</param>
        /// <returns>The executing process.</returns>
        public abstract Task ExecuteAsync(TRequest request, CancellationToken cancellationToken);

        async Task<TNoResponse> IRequestHandler<TRequest, TNoResponse>.ExecuteAsync(TRequest request, CancellationToken cancellationToken)
        {
            await ExecuteAsync(request, cancellationToken).ConfigureAwait(false);
            return default!;
        }
    }
}
