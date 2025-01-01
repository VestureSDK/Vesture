using Crucible.Mediator.Commands;
using Crucible.Mediator.Events;

namespace Crucible.Mediator.Requests
{
    /// <summary>
    /// Defines an handler invoked by a <see cref="IMediator"/> for a <see cref="IRequest{TResponse}"/> producing a <typeparamref name="TResponse"/> as expected by the specified <typeparamref name="TRequest"/>.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>To handle <see cref="ICommand"/> or <see cref="IEvent"/>, kindly see <see cref="ICommandHandler{TCommand}"/> and <see cref="IEventHandler{TEvent}"/> respectively.</item>
    /// <item>You should inherit from <see cref="RequestHandler{TRequest, TResponse}"/> to implement a <see cref="IRequestHandler{TRequest, TResponse}"/>.</item>
    /// </list>
    /// </remarks>
    /// <typeparam name="TRequest">The <see cref="IRequest{TResponse}"/> type.</typeparam>
    /// <typeparam name="TResponse">The <c>TResponse</c> type as expected by the <see cref="IRequest{TResponse}"/> specified by <typeparamref name="TRequest"/>.</typeparam>
    /// <seealso cref="IRequest{TResponse}"/>
    /// <seealso cref="RequestHandler{TRequest, TResponse}"/>
    /// <seealso cref="IMediator"/>
    /// <seealso cref="ICommandHandler{TCommand}"/>
    /// <seealso cref="IEventHandler{TEvent}"/>
    public interface IRequestHandler<TRequest, TResponse>
    {
        /// <summary>
        /// Handles the <see cref="IRequest{TResponse}"/> and produces a <typeparamref name="TResponse"/> as expected by the specified <typeparamref name="TRequest"/>.
        /// </summary>
        /// <param name="request">The <see cref="IRequest{TResponse}"/> to handle.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the execution.</param>
        /// <returns>The <typeparamref name="TResponse"/> as expected by the specified <typeparamref name="TRequest"/>.</returns>
        Task<TResponse> ExecuteAsync(TRequest request, CancellationToken cancellationToken = default);
    }
}
