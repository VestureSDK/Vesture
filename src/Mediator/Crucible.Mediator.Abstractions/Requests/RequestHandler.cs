namespace Crucible.Mediator.Requests
{
    /// <summary>
    /// <para>
    /// Provides a base implementation of the <see cref="IRequestHandler{TRequest, TResponse}"/>.
    /// You should inherit from this class and override the <see cref="HandleAsync"/> method 
    /// to define the logic for processing a specific <see cref="IRequest{TResponse}"/> contract
    /// and producing a <typeparamref name="TResponse"/> result, as expected by the corresponding 
    /// <typeparamref name="TRequest"/>.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <inheritdoc cref="IRequestHandler{TRequest, TResponse}" path="/summary"/>
    /// <inheritdoc cref="IRequestHandler{TRequest, TResponse}" path="/remarks"/>
    /// </remarks>
    /// <inheritdoc cref="IRequestHandler{TRequest, TResponse}"/>
    /// <seealso cref="IRequest{TResponse}"/>
    /// <seealso cref="IRequestHandler{TRequest, TResponse}"/>
    /// <seealso cref="RequestWorkflow{TRequest, TResponse}"/>
    /// <seealso cref="IMediator"/>
    public abstract class RequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    {
        /// <inheritdoc/>
        public abstract Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken = default);
    }
}
