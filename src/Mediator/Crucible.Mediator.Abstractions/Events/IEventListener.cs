using Crucible.Mediator.Requests;

namespace Crucible.Mediator.Events
{
    /// <summary>
    /// Defines a <typeparamref name="TEvent"/> publication handler.
    /// </summary>
    /// <remarks>
    /// For ease of implementation, you should inherit from <see cref="EventListener{TEvent}"/> instead.
    /// </remarks>
    /// <typeparam name="TEvent">The <see cref="IEvent"/> type.</typeparam>
    public interface IEventListener<TEvent> : IRequestHandler<TEvent, EventResponse>
    {

    }
}
