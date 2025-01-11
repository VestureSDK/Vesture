using Crucible.Mediator.Commands;
using Crucible.Mediator.Events;
using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Requests
{
    /// <summary>
    /// <para>
    /// The <see cref="RequestWorkflow{TRequest, TResponse}"/> provides a base implementation of the <see cref="IInvocationWorkflow"/>.
    /// You should inherit from this class and override the 
    /// <see cref="RequestHandler{TRequest, TResponse}.HandleAsync(TRequest, CancellationToken)"/> method 
    /// to manage and coordinate the flow of multiple operations across different handlers for a 
    /// specific <see cref="IRequest{TResponse}"/> contract and producing a <typeparamref name="TResponse"/> 
    /// result, as expected by the corresponding <typeparamref name="TRequest"/>.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <inheritdoc cref="IInvocationWorkflow" path="/summary"/>
    /// </remarks>
    /// <typeparam name="TRequest">
    /// The <see cref="IRequest{TResponse}"/> contract type handled by this handler.
    /// </typeparam>
    /// <typeparam name="TResponse">
    /// The response type produced by processing the <typeparamref name="TRequest"/>.
    /// </typeparam>
    /// <seealso cref="IRequest{TResponse}"/>
    /// <seealso cref="RequestHandler{TRequest, TResponse}"/>
    /// <seealso cref="IInvocationWorkflow"/>
    /// <seealso cref="IWorkflowMediator"/>
    public abstract class RequestWorkflow<TRequest, TResponse> : RequestHandler<TRequest, TResponse>, IInvocationWorkflow
        where TRequest : IRequest<TResponse>
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
