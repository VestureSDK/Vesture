using Crucible.Mediator.Commands;
using Crucible.Mediator.Events;
using Crucible.Mediator.Invocation;
using Crucible.Mediator.Requests;

namespace Crucible.Mediator
{
    /// <summary>
    /// Default implementation of <see cref="IMediator"/>.
    /// </summary>
    /// <remarks>
    /// It uses <see cref="ICommandInvoker"/>, <see cref="IRequestExecutor"/> and <see cref="IEventPublisher"/> under the hood.
    /// </remarks>
    public class Mediator : Invoker, IMediator
    {
        private readonly ICommandInvoker _commandInvoker;

        private readonly IRequestExecutor _requestInvoker;

        private readonly IEventPublisher _eventPublisher;

        /// <summary>
        /// Initializes a new <see cref="Mediator"/> instance.
        /// </summary>
        /// <param name="invocationPipelineProvider">The <see cref="IInvocationPipelineProvider"/> instance.</param>
        /// <param name="commandInvoker">The <see cref="ICommandInvoker"/> instance.</param>
        /// <param name="requestInvoker">The <see cref="IRequestExecutor"/> instance.</param>
        /// <param name="eventPublisher">The <see cref="IEventPublisher"/> instance.</param>
        public Mediator(IInvocationPipelineProvider invocationPipelineProvider, ICommandInvoker commandInvoker, IRequestExecutor requestInvoker, IEventPublisher eventPublisher)
            : base(invocationPipelineProvider)
        {
            _commandInvoker = commandInvoker;
            _requestInvoker = requestInvoker;
            _eventPublisher = eventPublisher;
        }

        /// <inheritdoc/>
        public Task<IInvocationContext<TResponse>> ExecuteAndCaptureAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            return _requestInvoker.ExecuteAndCaptureAsync(request, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<TResponse> ExecuteAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            return _requestInvoker.ExecuteAsync(request, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<IInvocationContext> InvokeAndCaptureAsync(ICommand command, CancellationToken cancellationToken = default)
        {
            return _commandInvoker.InvokeAndCaptureAsync(command, cancellationToken);
        }

        /// <inheritdoc/>
        public Task InvokeAsync(ICommand command, CancellationToken cancellationToken = default)
        {
            return _commandInvoker.InvokeAsync(command, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<IInvocationContext> PublishAndCaptureAsync(IEvent @event, CancellationToken cancellationToken = default)
        {
            return _eventPublisher.PublishAndCaptureAsync(@event, cancellationToken);
        }

        /// <inheritdoc/>
        public Task PublishAsync(IEvent @event, CancellationToken cancellationToken = default)
        {
            return _eventPublisher.PublishAsync(@event, cancellationToken);
        }
    }
}
