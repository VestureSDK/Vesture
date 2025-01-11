namespace Crucible.Mediator.Requests
{
    /// <summary>
    /// <para>
    /// The <see cref="CommandHandler{TRequest, TResponse}"/> provides a base implementation of the <see cref="IRequestHandler{TRequest, TResponse}"/>.
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
    public abstract class CommandHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        /// <summary>
        /// Processes the specified <see cref="IRequest{TResponse}"/> contract and returns the expected 
        /// <typeparamref name="TResponse"/>.
        /// </summary>
        /// <param name="request">
        /// The <see cref="IRequest{TResponse}"/> contract instance to process.
        /// </param>
        /// <param name="cancellationToken">
        /// A token that propagates notification that the operation should be canceled.
        /// </param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation, with 
        /// a result of type <typeparamref name="TResponse"/>.
        /// </returns>
        public abstract Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken = default);
    }
}
