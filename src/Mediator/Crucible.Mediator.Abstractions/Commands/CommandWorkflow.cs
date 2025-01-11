using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Commands
{
    /// <summary>
    /// <para>
    /// The <see cref="CommandWorkflow{TCommand}"/> provides a base implementation of the <see cref="IInvocationWorkflow"/>.
    /// You should inherit from this class and override the 
    /// <see cref="CommandHandler{TCommand}.HandleAsync(TCommand, CancellationToken)"/> method 
    /// to manage and coordinate the flow of multiple operations across different handlers for a 
    /// specific <see cref="ICommand"/> contract.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <inheritdoc cref="IInvocationWorkflow" path="/summary"/>
    /// </remarks>
    /// <typeparam name="TCommand">
    /// The <see cref="ICommand"/> contract type handled by this handler.
    /// </typeparam>
    /// <seealso cref="ICommand"/>
    /// <seealso cref="CommandHandler{TCommand}"/>
    /// <seealso cref="IInvocationWorkflow"/>
    /// <seealso cref="IWorkflowMediator"/>
    public abstract class CommandWorkflow<TCommand> : CommandHandler<TCommand>, IInvocationWorkflow
        where TCommand : ICommand
    {
        private IWorkflowMediator? _workflowMediator;

        private IWorkflowMediator WorkflowMediator
        {
            get => _workflowMediator ?? throw new EntryPointNotFoundException("The Workflow mediator has not yet been initialized");
            set => _workflowMediator = value;
        }

        IWorkflowMediator IInvocationWorkflow.Mediator { set => WorkflowMediator = value; }
    }
}
