namespace Crucible.Mediator.Requests
{
    /// <summary>
    /// Base implementation of <see cref="IRequestHandler{TRequest, TResponse}"/>.
    /// </summary>
    /// <remarks>
    /// Override <see cref="ExecuteAsync(TRequest, CancellationToken)"/> to handle the request as specified by <typeparamref name="TRequest"/>.
    /// </remarks>
    /// <typeparam name="TRequest">The <see cref="IRequest{TResponse}"/> type.</typeparam>
    /// <typeparam name="TResponse">The response type produced as specified in <typeparamref name="TRequest"/>.</typeparam>
    public abstract class RequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    {
        /// <inheritdoc/>
        public abstract Task<TResponse> ExecuteAsync(TRequest request, CancellationToken cancellationToken = default);
    }
}
