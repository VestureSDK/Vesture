namespace Ingot.Mediator.Events
{
    /// <summary>
    /// <para>
    /// An <see cref="IEvent"/> defines a contract for events in your application. 
    /// </para>
    /// <para>
    /// Events represent occurrences or actions that have already taken place, and they typically notify other parts 
    /// of the system to react. For instance, events may be used to trigger notifications, logging, or other system-wide changes.
    /// </para>
    /// <para>
    /// When published via the <c>IMediator</c>, the event is processed by one or more <c>IEventHandler</c>, 
    /// enabling the system to respond to the event accordingly. This separation of concerns allows for cleaner and more flexible code, 
    /// as event handling logic is decoupled from the components that trigger events.
    /// </para>
    /// <para>
    /// By using the mediator pattern to publish events, you centralize the event-handling logic and facilitate easy 
    /// extensibility and maintainability without tightly coupling the source of the event to the consumers.
    /// </para>
    /// </summary>
    public interface IEvent
    {
        // Marker interface
    }
}
