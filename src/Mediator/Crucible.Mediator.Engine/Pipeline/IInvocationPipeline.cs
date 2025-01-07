using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Engine.Pipeline
{
    /// <exclude />
    /// <inheritdoc cref="IInvocationPipeline{TResponse}" />
    public interface IInvocationPipeline
    {
        /// <summary>
        /// Gets the <see cref="Type"/> of the contract being processed by this pipeline.
        /// </summary>
        Type Request { get; }

        /// <summary>
        /// Gets the <see cref="Type"/> of the response produced by this pipeline.
        /// </summary>
        Type Response { get; }
    }

    /// <summary>
    /// <para>
    /// An <see cref="IInvocationPipeline{TResponse}"/> represents the orchestrated sequence of execution 
    /// that processes a specific contract through a series of <see cref="IInvocationMiddleware{TRequest, TResponse}"/> 
    /// and ultimately reaches an <see cref="IInvocationHandler{TRequest, TResponse}"/>.
    /// </para>
    /// <para>
    /// Invocation pipelines are a core component in the mediator pattern that enhances 
    /// modularity and separation of concerns by encapsulating cross-cutting logic such as logging, 
    /// validation, authorization, and more within middlewares.
    /// </para>
    /// <para>
    /// This pipeline embodies a chain of responsibility, where each middleware can perform actions 
    /// before or after invoking the next item in the chain, culminating with the handler.
    /// </para>
    /// </summary>
    /// <typeparam name="TResponse">The type of the response produced by this pipeline.</typeparam>
    /// <seealso cref="IMediator"/>
    /// <seealso cref="IInvocationMiddleware{TRequest, TResponse}"/>
    /// <seealso cref="IInvocationHandler{TRequest, TResponse}"/>
    public interface IInvocationPipeline<TResponse> : IInvocationPipeline
    {
        /// <summary>
        /// Processes the specified contract and returns 
        /// the <see cref="IInvocationContext{TResponse}"/> containing the expected <typeparamref name="TResponse"/> 
        /// or any <see cref="Exception"/> that might have occurred.
        /// </summary>
        /// <param name="request">The contract instance to process.</param>
        /// <param name="cancellationToken">
        /// <inheritdoc cref="IInvocationHandler{TRequest, TResponse}.HandleAsync(TRequest, CancellationToken)" path="/param[@name='cancellationToken']"/>
        /// </param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation, with 
        /// a result of type <see cref="IInvocationContext{TResponse}"/> containing the expected 
        /// <typeparamref name="TResponse"/> or any <see cref="Exception"/> that might have occurred.
        /// </returns>
        Task<IInvocationContext<TResponse>> HandleAsync(object request, CancellationToken cancellationToken);
    }
}
