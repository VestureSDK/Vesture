using Crucible.Mediator.Requests;

namespace Crucible.Mediator.Commands
{
    /// <summary>
    /// Defines an handler invoked by a <see cref="IMediator"/> for a <see cref="ICommand"/> (as specified by <typeparamref name="TCommand"/>).
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><see cref="ICommandHandler{TCommand}"/> is a <see cref="IRequestHandler{TRequest, TResponse}"/> with <see cref="CommandResponse"/> as <c>TResponse</c> implying no response is expected.</item>
    /// <item>You should inherit from <see cref="CommandHandler{TCommand}"/> to implement a <see cref="ICommandHandler{TCommand}"/>.</item>
    /// </list>
    /// </remarks>
    /// <typeparam name="TCommand">The <see cref="ICommand"/> type.</typeparam>
    /// <seealso cref="ICommand"/>
    /// <seealso cref="CommandHandler{TCommand}"/>
    /// <seealso cref="IRequestHandler{TRequest, TResponse}"/>
    /// <seealso cref="IMediator"/>
    public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, CommandResponse>
    {
    }
}
