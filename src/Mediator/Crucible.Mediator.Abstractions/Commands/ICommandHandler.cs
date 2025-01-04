using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Commands
{
    /// <summary>
    /// <para>
    /// A <see cref="ICommandHandler{TCommand}"/> is responsible for the actual 
    /// logic of processing a specific <see cref="ICommand"/> contract.
    /// </para>
    /// <para>
    /// When an <see cref="ICommand"/> contract is sent to the mediator, the mediator 
    /// routes it to the appropriate handler, which then processes the command.
    /// It helps decouple command processing logic from the core application logic, enabling 
    /// cleaner, more modular code.
    /// </para>
    /// </summary>
    /// <typeparam name="TCommand">
    /// The <see cref="ICommand"/> contract type handled by this handler.
    /// </typeparam>
    /// <remarks>
    /// <para>
    /// <see cref="ICommandHandler{TCommand}"/> should not directly depend on or invoke 
    /// <see cref="IMediator"/> for subsequent operations, as this can lead to tightly 
    /// coupled and difficult-to-maintain code.
    /// </para>
    /// <para>
    /// Instead, a <seealso cref="CommandWorkflow{TCommand}"/> should be used to orchestrate 
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
    /// <seealso cref="ICommand"/>
    /// <seealso cref="CommandHandler{TCommand}"/>
    /// <seealso cref="CommandWorkflow{TCommand}"/>
    /// <seealso cref="IMediator"/>
    public interface ICommandHandler<TCommand> : IInvocationHandler<TCommand, CommandResponse>
    {
    }
}
