using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Events
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
    /// <remarks>
    /// <para>
    /// <see cref="IEventHandler{TEvent}"/> should not directly depend on or invoke 
    /// <see cref="IMediator"/> for subsequent operations, as this can lead to tightly 
    /// coupled and difficult-to-maintain code.
    /// </para>
    /// <para>
    /// Instead, a <seealso cref="EventWorkflow{TEvent}"/> should be used to orchestrate 
    /// the flow of operations. <see cref="IInvocationWorkflow"/> provide a higher-level abstraction for 
    /// managing complex workflows, ensuring that different handlers are executed in the 
    /// correct order while maintaining a clear separation of concerns. 
    /// </para>
    /// <para>
    /// By using workflows, you ensure that each handler remains focused on its individual 
    /// responsibility, leaving orchestration and sequencing to the workflows, thus adhering 
    /// to the principles of loose coupling and maintainability.
    /// </para>
    /// </remarks>
    /// <seealso cref="IEvent"/>
    /// <seealso cref="EventHandler{TEvent}"/>
    /// <seealso cref="EventWorkflow{TEvent}"/>
    /// <seealso cref="IMediator"/>
    public interface IEventHandler<TEvent> : IInvocationHandler<TEvent, EventResponse>
    {

    }
}
