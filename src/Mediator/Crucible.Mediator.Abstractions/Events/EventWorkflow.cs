using Crucible.Mediator.Commands;
using Crucible.Mediator.Invocation;
using Crucible.Mediator.Requests;

namespace Crucible.Mediator.Events
{
    /// <summary>
    /// <para>
    /// The <see cref="EventWorkflow{TEvent}"/> provides a base implementation of the <see cref="IInvocationWorkflow"/>.
    /// You should inherit from this class and override the 
    /// <see cref="EventHandler{TEvent}.HandleAsync(TEvent, CancellationToken)"/> method 
    /// to manage and coordinate the flow of multiple operations across different handlers for a 
    /// specific <see cref="IEvent"/> contract.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <inheritdoc cref="IInvocationWorkflow" path="/summary"/>
    /// </remarks>
    /// <typeparam name="TEvent">
    /// The <see cref="IEvent"/> contract type handled by this handler.
    /// </typeparam>
    /// <seealso cref="IEvent"/>
    /// <seealso cref="EventHandler{TEvent}"/>
    /// <seealso cref="IInvocationWorkflow"/>
    /// <seealso cref="IWorkflowMediator"/>
    public abstract class EventWorkflow<TEvent> : EventHandler<TEvent>, IInvocationWorkflow
        where TEvent : IEvent
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
