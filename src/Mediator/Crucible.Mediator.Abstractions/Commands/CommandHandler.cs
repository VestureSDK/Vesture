using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Commands
{
    /// <summary>
    /// Base implementation of <see cref="ICommandHandler{TCommand}"/>.
    /// </summary>
    /// <remarks>
    /// Override <see cref="NoResponseInvocationHandler{TCommand, CommandResponse}.ExecuteAsync(TCommand, CancellationToken)"/> to handle the command as 
    /// specified by <typeparamref name="TCommand"/>.
    /// </remarks>
    /// <typeparam name="TCommand">The <see cref="ICommand"/> type.</typeparam>
    public abstract class CommandHandler<TCommand> : NoResponseInvocationHandler<TCommand, CommandResponse>, ICommandHandler<TCommand>
    {

    }
}
