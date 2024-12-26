using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Events
{
    /// <summary>
    /// Base implementation of <see cref="IEventListener{TEvent}"/>.
    /// </summary>
    /// <remarks>
    /// Override <see cref="NoResponseInvocationHandler{TEvent, EventResponse}.ExecuteAsync(TEvent, CancellationToken)"/> 
    /// to handle the event as specified by <typeparamref name="TEvent"/>.
    /// </remarks>
    /// <typeparam name="TEvent">The <see cref="IEvent"/> type.</typeparam>
    public abstract class EventListener<TEvent> : NoResponseInvocationHandler<TEvent, EventResponse>, IEventListener<TEvent>
    {

    }
}
