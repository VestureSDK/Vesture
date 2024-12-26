using Crucible.Mediator.Requests;

namespace Crucible.Mediator.Commands
{
    /// <summary>
    /// Defines a <typeparamref name="TCommand"/> handler.
    /// </summary>
    /// <remarks>
    /// For ease of implementation, you should inherit from <see cref="CommandHandler{TRequest}"/> instead.
    /// </remarks>
    /// <typeparam name="TCommand">The <see cref="ICommand"/> type.</typeparam>
    public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, CommandResponse>
    {
    }
}
