using Crucible.Mediator.Requests;

namespace Crucible.Mediator.Events
{
    /// <summary>
    /// Defines an handler invoked by a <see cref="IMediator"/> for a <see cref="IEvent"/> (as specified by <typeparamref name="TEvent"/>).
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><see cref="IEventHandler{TEvent}"/> is a <see cref="IRequestHandler{TRequest, TResponse}"/> with <see cref="EventResponse"/> as <c>TResponse</c> implying no response is expected.</item>
    /// <item>You should inherit from <see cref="EventHandler{TEvent}"/> to implement a <see cref="IEventHandler{TEvent}"/>.</item>
    /// </list>
    /// </remarks>
    /// <typeparam name="TEvent">The <see cref="IEvent"/> type.</typeparam>
    /// <seealso cref="IEvent"/>
    /// <seealso cref="EventHandler{TEvent}"/>
    /// <seealso cref="IRequestHandler{TRequest, TResponse}"/>
    /// <seealso cref="IMediator"/>
    /// <inheritdoc cref="IEvent" path="/example"/>
    public interface IEventHandler<TEvent> : IRequestHandler<TEvent, EventResponse>
    {

    }
}
