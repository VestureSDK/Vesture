using Vesture.Mediator.Invocation;

namespace Vesture.Mediator.Events
{
    /// <summary>
    /// <para>
    /// An <see cref="IEventHandler{TEvent}"/> is responsible for the actual
    /// logic of processing a specific <see cref="IEvent"/> contract.
    /// </para>
    /// <para>
    /// When an <see cref="IEvent"/> contract is sent to the mediator, the mediator
    /// routes it to the appropriate handler, which then processes the event.
    /// It helps decouple event processing logic from the core application logic, enabling
    /// cleaner, more modular code.
    /// </para>
    /// </summary>
    /// <typeparam name="TEvent">
    /// The <see cref="IEvent"/> contract type handled by this handler.
    /// </typeparam>
    /// <seealso cref="IEvent"/>
    /// <seealso cref="EventHandler{TEvent}"/>
    /// <seealso cref="IMediator"/>
    public interface IEventHandler<TEvent> : IInvocationHandler<TEvent, EventResponse> { }
}
