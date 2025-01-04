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
    /// <seealso cref="IMediator"/>
    public abstract class EventWorkflow<TEvent> : EventHandler<TEvent>, IInvocationWorkflow
        where TEvent : IEvent
    {
        private IMediator? _workflowMediator;

        private IMediator WorkflowMediator
        {
            get => _workflowMediator ?? throw new EntryPointNotFoundException("The Workflow mediator has not yet been initialized");
            set => _workflowMediator = value;
        }

        IMediator IInvocationWorkflow.Mediator { set => WorkflowMediator = value; }

        /// <exclude />
        /// <inheritdoc cref="IMediator.ExecuteAndCaptureAsync{TResponse}(IRequest{TResponse}, CancellationToken)"/>
        public Task<IInvocationContext<TResponse>> ExecuteAndCaptureAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default) => WorkflowMediator.ExecuteAndCaptureAsync(request, cancellationToken);

        /// <exclude />
        /// <inheritdoc cref="IMediator.ExecuteAsync{TResponse}(IRequest{TResponse}, CancellationToken)"/>
        public Task<TResponse> ExecuteAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default) => WorkflowMediator.ExecuteAsync(request, cancellationToken);

        /// <exclude />
        /// <inheritdoc cref="IMediator.HandleAndCaptureAsync{TResponse}(object, CancellationToken)"/>
        public Task<IInvocationContext<TResponse>> HandleAndCaptureAsync<TResponse>(object request, CancellationToken cancellationToken = default) => WorkflowMediator.HandleAndCaptureAsync<TResponse>(request, cancellationToken);

        /// <exclude />
        /// <inheritdoc cref="IMediator.HandleAsync{TResponse}(object, CancellationToken)"/>
        public Task<TResponse> HandleAsync<TResponse>(object request, CancellationToken cancellationToken = default) => WorkflowMediator.HandleAsync<TResponse>(request, cancellationToken);

        /// <exclude />
        /// <inheritdoc cref="IMediator.InvokeAndCaptureAsync(ICommand, CancellationToken)"/>
        public Task<IInvocationContext> InvokeAndCaptureAsync(ICommand command, CancellationToken cancellationToken = default) => WorkflowMediator.InvokeAndCaptureAsync(command, cancellationToken);

        /// <exclude />
        /// <inheritdoc cref="IMediator.InvokeAsync(ICommand, CancellationToken)"/>
        public Task InvokeAsync(ICommand command, CancellationToken cancellationToken = default) => WorkflowMediator.InvokeAsync(command, cancellationToken);

        /// <exclude />
        /// <inheritdoc cref="IMediator.PublishAndCaptureAsync(IEvent, CancellationToken)"/>
        public Task<IInvocationContext> PublishAndCaptureAsync(IEvent @event, CancellationToken cancellationToken = default) => WorkflowMediator.PublishAndCaptureAsync(@event, cancellationToken);

        /// <exclude />
        /// <inheritdoc cref="IMediator.PublishAsync(IEvent, CancellationToken)"/>
        public Task PublishAsync(IEvent @event, CancellationToken cancellationToken = default) => WorkflowMediator.PublishAsync(@event, cancellationToken);
    }
}
