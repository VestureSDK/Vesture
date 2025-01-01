using Crucible.Mediator.Commands;
using Crucible.Mediator.Events;

namespace Crucible.Mediator.Requests
{
    /// <summary>
    /// Base implementation of <see cref="IRequestHandler{TRequest, TResponse}"/> invoked by a <see cref="IMediator"/> for a <see cref="IRequest{TResponse}"/> producing a <typeparamref name="TResponse"/> as expected by the specified <typeparamref name="TRequest"/>.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>To handle <see cref="ICommand"/> or <see cref="IEvent"/>, kindly see <see cref="CommandHandler{TCommand}"/> and <see cref="EventHandler{TEvent}"/> respectively.</item>
    /// <item>Override <see cref="ExecuteAsync(TRequest, CancellationToken)"/> to implement the <see cref="IRequestHandler{TRequest, TResponse}"/> logic.</item>
    /// </list>
    /// </remarks>
    /// <typeparam name="TRequest">The <see cref="IRequest{TResponse}"/> type.</typeparam>
    /// <typeparam name="TResponse">The <c>TResponse</c> type as expected by the <see cref="IRequest{TResponse}"/> specified by <typeparamref name="TRequest"/>.</typeparam>
    public abstract class RequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    {
        /// <inheritdoc/>
        public abstract Task<TResponse> ExecuteAsync(TRequest request, CancellationToken cancellationToken = default);
    }
}
