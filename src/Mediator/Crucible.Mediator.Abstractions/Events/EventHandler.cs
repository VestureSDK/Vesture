using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Events
{
    /// <summary>
    /// <para>
    /// The <see cref="EventHandler{TEvent}"/> provides a base implementation of the <see cref="IEventHandler{TEvent}"/>.
    /// You should inherit from this class and override the <see cref="HandleAsync"/> method 
    /// to define the logic for processing a specific <see cref="IEvent"/> contract.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <inheritdoc cref="IEventHandler{TEvent}" path="/summary"/>
    /// <inheritdoc cref="IEventHandler{TEvent}" path="/remarks"/>
    /// </remarks>
    /// <inheritdoc cref="IEventHandler{TEvent}"/>
    /// <seealso cref="IEvent"/>
    /// <seealso cref="IEventHandler{TEvent}"/>
    /// <seealso cref="EventWorkflow{TEvent}"/>
    /// <seealso cref="IMediator"/>
    public abstract class EventHandler<TEvent> : IEventHandler<TEvent>
    {
        /// <summary>
        /// Processes the specified <see cref="IEvent"/> contract.
        /// </summary>
        /// <param name="event">
        /// The <see cref="IEvent"/> contract instance to process.
        /// </param>
        /// <param name="cancellationToken">
        /// <inheritdoc cref="IInvocationHandler{TRequest, TResponse}.HandleAsync(TRequest, CancellationToken)" path="/param[@name='cancellationToken']"/>
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        public abstract Task HandleAsync(TEvent @event, CancellationToken cancellationToken);

        async Task<EventResponse> IInvocationHandler<TEvent, EventResponse>.HandleAsync(TEvent request, CancellationToken cancellationToken)
        {
            await HandleAsync(request, cancellationToken).ConfigureAwait(false);
            return default!;
        }
    }
}
