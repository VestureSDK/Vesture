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
    /// <remarks>
    /// <para>
    /// <see cref="IInvocationHandler{TRequest, TResponse}"/> should not directly depend on or invoke 
    /// <see cref="IMediator"/> for subsequent operations, as this can lead to tightly 
    /// coupled and difficult-to-maintain code.
    /// </para>
    /// <para>
    /// Instead, a <seealso cref="IInvocationWorkflow"/> should be used to orchestrate 
    /// the flow of operations. <see cref="IInvocationWorkflow"/> provide a higher-level abstraction for 
    /// managing complex workflows, ensuring that different handlers are executed in the 
    /// correct order while maintaining a clear separation of concerns. 
    /// </para>
    /// <para>
    /// By using workflows, you ensure that each handler remains focused on its individual 
    /// responsibility, leaving orchestration and sequencing to the workflows, thus adhering 
    /// to the principles of loose coupling and maintainability.
    /// </para>
    /// </remarks>
    /// <seealso cref="IInvocationWorkflow"/>
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
