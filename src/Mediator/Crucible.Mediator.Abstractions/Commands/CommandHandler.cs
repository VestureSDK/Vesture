using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Commands
{
    /// <summary>
    /// <para>
    /// The <see cref="CommandHandler{TCommand}"/> provides a base implementation of the <see cref="ICommandHandler{TCommand}"/>.
    /// You should inherit from this class and override the <see cref="HandleAsync"/> method 
    /// to define the logic for processing a specific <see cref="ICommand"/> contract.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <inheritdoc cref="ICommandHandler{TCommand}" path="/summary"/>
    /// <inheritdoc cref="ICommandHandler{TCommand}" path="/remarks"/>
    /// </remarks>
    /// <inheritdoc cref="ICommandHandler{TCommand}"/>
    /// <seealso cref="ICommand"/>
    /// <seealso cref="ICommandHandler{TCommand}"/>
    /// <seealso cref="CommandWorkflow{TCommand}"/>
    /// <seealso cref="IMediator"/>
    public abstract class CommandHandler<TCommand> : ICommandHandler<TCommand>
    {
        /// <summary>
        /// Processes the specified <see cref="ICommand"/> contract.
        /// </summary>
        /// <param name="command">
        /// The <see cref="ICommand"/> contract instance to process.
        /// </param>
        /// <param name="cancellationToken">
        /// <inheritdoc cref="IInvocationHandler{TRequest, TResponse}.HandleAsync(TRequest, CancellationToken)" path="/param[@name='cancellationToken']"/>
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        public abstract Task HandleAsync(TCommand command, CancellationToken cancellationToken);

        async Task<CommandResponse> IInvocationHandler<TCommand, CommandResponse>.HandleAsync(TCommand request, CancellationToken cancellationToken)
        {
            await HandleAsync(request, cancellationToken).ConfigureAwait(false);
            return default!;
        }
    }
}
