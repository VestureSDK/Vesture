namespace Crucible.Mediator.Invocation
{
    /// <summary>
    /// <para>
    /// An <see cref="IInvocationHandler{TRequest, TResponse}"/> is responsible for the actual 
    /// logic of processing a specific contract.
    /// </para>
    /// <para>
    /// When a contract is sent to the mediator, the mediator routes it to the appropriate handler, 
    /// which then processes the request and returns a <typeparamref name="TResponse"/>.
    /// It helps decouple processing logic from the core application logic, enabling cleaner, more modular code.
    /// </para>
    /// </summary>
    /// <typeparam name="TRequest">
    /// The contract type handled by this handler.
    /// </typeparam>
    /// <typeparam name="TResponse">
    /// The response type produced by processing the <typeparamref name="TRequest"/>.
    /// </typeparam>
    /// <seealso cref="IMediator"/>
    public interface IInvocationHandler<TRequest, TResponse>
    {
        /// <summary>
        /// Processes the specified <typeparamref name="TRequest"/> contract and returns the expected 
        /// <typeparamref name="TResponse"/>.
        /// </summary>
        /// <param name="request">
        /// The contract instance to process.
        /// </param>
        /// <param name="cancellationToken">
        /// A token that propagates notification that the operation should be canceled.
        /// </param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation, with 
        /// a result of type <typeparamref name="TResponse"/>.
        /// </returns>
        Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken = default);
    }
}
