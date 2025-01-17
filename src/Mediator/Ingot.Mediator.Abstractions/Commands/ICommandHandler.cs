using Ingot.Mediator.Invocation;

namespace Ingot.Mediator.Commands
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
    /// <seealso cref="ICommand"/>
    /// <seealso cref="CommandHandler{TCommand}"/>
    /// <seealso cref="IMediator"/>
    public interface ICommandHandler<TCommand> : IInvocationHandler<TCommand, CommandResponse>
    {
    }
}
