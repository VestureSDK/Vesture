using Crucible.Mediator.Commands;
using Crucible.Mediator.Requests;

namespace Crucible.Mediator.Events
{
    /// <summary>
    /// Base implementation of <see cref="IEventHandler{TEvent}"/> invoked by a <see cref="IMediator"/> for an <see cref="IEvent"/>.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>To handle <see cref="ICommand"/> or <see cref="IRequest{TResponse}"/>, kindly see <see cref="CommandHandler{TEvent}"/> and <see cref="RequestHandler{TRequest, TResponse}"/> respectively.</item>
    /// <item>Override <see cref="HandleAsync(TEvent, CancellationToken)"/> to implement the <see cref="IEventHandler{TCommand}"/> logic.</item>
    /// </list>
    /// </remarks>
    /// <typeparam name="TEvent">The <see cref="IEvent"/> type.</typeparam>
    public abstract class EventHandler<TEvent> : IEventHandler<TEvent>
    {
        /// <summary>
        /// Handles the <see cref="IEvent"/>.
        /// </summary>
        /// <param name="event">The <see cref="IEvent"/> to handle.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the execution.</param>
        public abstract Task HandleAsync(TEvent @event, CancellationToken cancellationToken);

        async Task<EventResponse> IRequestHandler<TEvent, EventResponse>.ExecuteAsync(TEvent request, CancellationToken cancellationToken)
        {
            await HandleAsync(request, cancellationToken).ConfigureAwait(false);
            return default!;
        }
    }
}
