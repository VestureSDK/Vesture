namespace Crucible.Mediator.Commands
{
    /// <summary>
    /// <para>
    /// A <see cref="ICommand"/> defines a contract for commands in your application. 
    /// </para>
    /// <para>
    /// Commands represent actions or operations that trigger state changes in the system that do not require 
    /// a response or result, such as creating, updating, or deleting data. 
    /// </para>
    /// <para>
    /// When invoked via the <c>IMediator</c>, the appropriate <c>ICommandHandler</c> is determined and the logic to process 
    /// the command is executed. This promotes clean, decoupled code by separating the command's definition from 
    /// its handling logic, which results in a more maintainable and flexible design.
    /// </para>
    /// <para>
    /// By using the mediator pattern for invoking commands, you ensure that command handling is centralized, 
    /// and you achieve better separation of concerns between the command, the logic, and the system that consumes it.
    /// </para>
    /// </summary>
    public interface ICommand
    {
        // Marker interface
    }
}
