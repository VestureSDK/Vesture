using Crucible.Mediator.Events;
using Crucible.Mediator.Requests;

namespace Crucible.Mediator.Commands
{
    /// <summary>
    /// Base implementation of <see cref="ICommandHandler{TCommand}"/> invoked by a <see cref="IMediator"/> for a <see cref="ICommand"/>.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>To handle <see cref="IEvent"/> or <see cref="IRequest{TResponse}"/>, kindly see <see cref="EventHandler{TEvent}"/> and <see cref="RequestHandler{TRequest, TResponse}"/> respectively.</item>
    /// <item>Override <see cref="InvokeAsync(TCommand, CancellationToken)"/> to implement the <see cref="ICommandHandler{TCommand}"/> logic.</item>
    /// </list>
    /// </remarks>
    /// <typeparam name="TCommand">The <see cref="ICommand"/> type.</typeparam>
    public abstract class CommandHandler<TCommand> : ICommandHandler<TCommand>
    {
        /// <summary>
        /// Handles the <see cref="ICommand"/>.
        /// </summary>
        /// <param name="command">The <see cref="ICommand"/> to handle.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the execution.</param>
        public abstract Task InvokeAsync(TCommand command, CancellationToken cancellationToken);

        async Task<CommandResponse> IRequestHandler<TCommand, CommandResponse>.ExecuteAsync(TCommand request, CancellationToken cancellationToken)
        {
            await InvokeAsync(request, cancellationToken).ConfigureAwait(false);
            return default!;
        }
    }
}
