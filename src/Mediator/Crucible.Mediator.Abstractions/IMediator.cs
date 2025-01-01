using Crucible.Mediator.Commands;
using Crucible.Mediator.Events;
using Crucible.Mediator.Invocation;
using Crucible.Mediator.Requests;

namespace Crucible.Mediator
{
    /// <summary>
    /// Defines a mediator to handle <see cref="IRequest{TResponse}"/>, <see cref="ICommand"/> and <see cref="IEvent"/>.
    /// </summary>
    /// <remarks>
    /// This is a facade for <see cref="IRequestExecutor"/>, <see cref="ICommandInvoker"/> and <see cref="IEventPublisher"/>.
    /// </remarks>
    public interface IMediator
    {
        /// <summary>
        /// Handles the specified <paramref name="request"/> (as a <see cref="IRequest{TResponse}"/>, <see cref="ICommand"/> or <see cref="IEvent"/>)
        /// and returns the expected <typeparamref name="TResponse"/>.
        /// </summary>
        /// <typeparam name="TResponse">The expected response from <paramref name="request"/>.</typeparam>
        /// <param name="request">The <see cref="IRequest{TResponse}"/>, <see cref="ICommand"/> or <see cref="IEvent"/> to handle.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the execution.</param>
        /// <returns>The expected <typeparamref name="TResponse"/> value.</returns>
        Task<TResponse> HandleAsync<TResponse>(object request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the specified <paramref name="request"/> (as a <see cref="IRequest{TResponse}"/>, <see cref="ICommand"/> or <see cref="IEvent"/>)
        /// and returns the <see cref="IInvocationContext{TResponse}"/> containing the expected 
        /// <typeparamref name="TResponse"/> or any <see cref="Exception"/> that might have occured.
        /// </summary>
        /// <typeparam name="TResponse">The expected response from <paramref name="request"/>.</typeparam>
        /// <param name="request">The <see cref="IRequest{TResponse}"/>, <see cref="ICommand"/> or <see cref="IEvent"/> to handle.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the execution.</param>
        /// <returns>
        /// The <see cref="IInvocationContext{TResponse}"/> containing the expected 
        /// <typeparamref name="TResponse"/> or any <see cref="Exception"/> that might have occured.
        /// </returns>
        Task<IInvocationContext<TResponse>> HandleAndCaptureAsync<TResponse>(object request, CancellationToken cancellationToken = default);

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

        /// <summary>
        /// Executes the specified <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the execution.</param>
        /// <returns>The executing process.</returns>
        Task InvokeAsync(ICommand command, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes the specified <paramref name="command"/> and returns the 
        /// <see cref="IInvocationContext"/> containing any <see cref="Exception"/> that might have occured.
        /// </summary>
        /// <param name="command">The <see cref="ICommand"/> to execute.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the execution.</param>
        /// <returns>
        /// The <see cref="IInvocationContext"/> containing any <see cref="Exception"/> that might have occured.
        /// </returns>
        Task<IInvocationContext> InvokeAndCaptureAsync(ICommand command, CancellationToken cancellationToken = default);

        /// <summary>
        /// Publishes the specified <paramref name="event"/>.
        /// </summary>
        /// <param name="event">The <see cref="IEvent"/> to publish.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the publication and related handlings.</param>
        /// <returns>The publication and handling process.</returns>
        Task PublishAsync(IEvent @event, CancellationToken cancellationToken = default);

        /// <summary>
        /// Publishes the specified <paramref name="event"/> and returns the <see cref="IInvocationContext"/> 
        /// containing any <see cref="Exception"/> that might have occured.
        /// </summary>
        /// <param name="event">The <see cref="IEvent"/> to publish.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the publication and related handlings.</param>
        /// <returns>
        /// The <see cref="IInvocationContext"/> containing any <see cref="Exception"/> that might have occured.
        /// </returns>
        Task<IInvocationContext> PublishAndCaptureAsync(IEvent @event, CancellationToken cancellationToken = default);
    }
}
