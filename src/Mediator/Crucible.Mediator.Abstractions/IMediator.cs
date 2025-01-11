using Crucible.Mediator.Commands;
using Crucible.Mediator.Events;
using Crucible.Mediator.Invocation;
using Crucible.Mediator.Requests;

namespace Crucible.Mediator
{
    /// <summary>
    /// <para>
    /// An <see cref="IMediator"/> coordinates the execution of different types of contracts 
    /// such as requests, commands, and events by invoking the appropriate handlers and middlewares.
    /// </para>
    /// <para>
    /// The mediator acts as a central point of communication in your application, 
    /// decoupling the components that send requests, commands, or events from those that handle them.
    /// </para>
    /// <para>
    /// This interface provides methods for processing:
    /// <list type="bullet">
    ///     <item><term><see cref="IRequest{TResponse}"/> contracts</term><description> which are typically used for operations that return a response.</description></item>
    ///     <item><term><see cref="ICommand"/> contracts</term><description>typically used for operations that trigger actions but don't expect a return value.</description></item>
    ///     <item><term><see cref="IEvent"/> contracts</term><description>typically used to notify other components of something that has occurred in the system.</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// The mediator ensures that the right handler for each type of contract is invoked, allowing for a 
    /// cleaner and more modular codebase by promoting loose coupling between components.
    /// </para>
    /// </summary>
    /// <seealso cref="IRequest{TResponse}"/>
    /// <seealso cref="CommandHandler{TRequest, TResponse}"/>
    /// <seealso cref="ICommand"/>
    /// <seealso cref="CommandHandler{TCommand}"/>
    /// <seealso cref="IEvent"/>
    /// <seealso cref="Crucible.Mediator.Events.EventHandler{TEvent}"/>
    public interface IMediator
    {
        /// <summary>
        /// Processes the specified unmarked contract and returns the expected 
        /// <typeparamref name="TResponse"/>.
        /// </summary>
        /// <typeparam name="TResponse">
        /// The response type produced by processing the specified unmarked contract.
        /// </typeparam>
        /// <param name="contract">
        /// The unmarked contract instance to process.
        /// </param>
        /// <param name="cancellationToken">
        /// <inheritdoc cref="IInvocationHandler{TRequest, TResponse}.HandleAsync(TRequest, CancellationToken)" path="/param[@name='cancellationToken']"/>
        /// </param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation, with 
        /// a result of type <typeparamref name="TResponse"/>.
        /// </returns>
        Task<TResponse> HandleAsync<TResponse>(object contract, CancellationToken cancellationToken = default);

        /// <summary>
        /// Processes the specified unmarked contract and returns 
        /// the <see cref="IInvocationContext{TResponse}"/> containing the expected <typeparamref name="TResponse"/> 
        /// or any <see cref="Exception"/> that might have occurred.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation, with 
        /// a result of type <see cref="IInvocationContext{TResponse}"/> containing the expected 
        /// <typeparamref name="TResponse"/> or any <see cref="Exception"/> that might have occurred.
        /// </returns>
        /// <inheritdoc cref="HandleAsync{TResponse}(Object, CancellationToken)"/>
        Task<IInvocationContext<TResponse>> HandleAndCaptureAsync<TResponse>(object contract, CancellationToken cancellationToken = default);

        /// <summary>
        /// Processes the specified <see cref="IRequest{TResponse}"/> contract and returns the expected 
        /// <typeparamref name="TResponse"/>.
        /// </summary>
        /// <typeparam name="TResponse">
        /// The response type produced by processing the specified <see cref="IRequest{TResponse}"/> contract.
        /// </typeparam>
        /// <param name="request">
        /// <inheritdoc cref="IInvocationHandler{TRequest, TResponse}.HandleAsync(TRequest, CancellationToken)" path="/param[@name='request']"/>
        /// </param>
        /// <param name="cancellationToken">
        /// <inheritdoc cref="IInvocationHandler{TRequest, TResponse}.HandleAsync(TRequest, CancellationToken)" path="/param[@name='cancellationToken']"/>
        /// </param>
        /// <returns>
        /// <inheritdoc cref="IInvocationHandler{TRequest, TResponse}.HandleAsync(TRequest, CancellationToken)" path="/returns"/>
        /// </returns>
        Task<TResponse> ExecuteAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Processes the specified <see cref="IRequest{TResponse}"/> contract and returns 
        /// the <see cref="IInvocationContext{TResponse}"/> containing the expected <typeparamref name="TResponse"/> 
        /// or any <see cref="Exception"/> that might have occurred.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation, with 
        /// a result of type <see cref="IInvocationContext{TResponse}"/> containing the expected 
        /// <typeparamref name="TResponse"/> or any <see cref="Exception"/> that might have occurred.
        /// </returns>
        /// <inheritdoc cref="ExecuteAsync{TResponse}(IRequest{TResponse}, CancellationToken)"/>
        Task<IInvocationContext<TResponse>> ExecuteAndCaptureAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);

        /// <inheritdoc cref="CommandHandler{TCommand}.HandleAsync(TCommand, CancellationToken)"/>
        Task InvokeAsync(ICommand command, CancellationToken cancellationToken = default);

        /// <summary>
        /// Processes the specified <see cref="ICommand"/> contract and returns the 
        /// <see cref="IInvocationContext{TResponse}"/> containing any <see cref="Exception"/> that might have occurred.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation, with 
        /// a result of type <see cref="IInvocationContext{TResponse}"/> containing any 
        /// <see cref="Exception"/> that might have occurred.
        /// </returns>
        /// <inheritdoc cref="InvokeAsync(ICommand, CancellationToken)"/>
        Task<IInvocationContext> InvokeAndCaptureAsync(ICommand command, CancellationToken cancellationToken = default);

        /// <inheritdoc cref="Crucible.Mediator.Events.EventHandler{TEvent}.HandleAsync(TEvent, CancellationToken)"/>
        Task PublishAsync(IEvent @event, CancellationToken cancellationToken = default);

        /// <summary>
        /// Processes the specified <see cref="IEvent"/> contract and returns the 
        /// <see cref="IInvocationContext{TResponse}"/> containing any <see cref="Exception"/> that might have occurred.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation, with 
        /// a result of type <see cref="IInvocationContext{TResponse}"/> containing any 
        /// <see cref="Exception"/> that might have occurred.
        /// </returns>
        /// <inheritdoc cref="PublishAsync(IEvent, CancellationToken)"/>
        Task<IInvocationContext> PublishAndCaptureAsync(IEvent @event, CancellationToken cancellationToken = default);
    }
}
