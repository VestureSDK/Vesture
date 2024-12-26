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
    public interface IMediator : IRequestExecutor, ICommandInvoker, IEventPublisher
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
    }
}
