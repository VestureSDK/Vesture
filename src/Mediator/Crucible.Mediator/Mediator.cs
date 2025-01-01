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
    public class Mediator : IMediator
    {
        private readonly IInvocationPipelineProvider _pipelineProvider;

        /// <summary>
        /// Initializes a new <see cref="Mediator"/> instance.
        /// </summary>
        /// <param name="pipelineProvider">The <see cref="IInvocationPipelineProvider"/> instance.</param>
        public Mediator(IInvocationPipelineProvider pipelineProvider)
        {
            _pipelineProvider = pipelineProvider;
        }

        /// <inheritdoc/>
        public Task<IInvocationContext<TResponse>> HandleAndCaptureAsync<TResponse>(object request, CancellationToken cancellationToken = default)
        {
            var pipeline = _pipelineProvider.GetInvocationPipeline<TResponse>(request);
            return pipeline.ExecuteAndCaptureAsync(request, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<TResponse> HandleAsync<TResponse>(object request, CancellationToken cancellationToken = default)
        {
            var pipeline = _pipelineProvider.GetInvocationPipeline<TResponse>(request);
            var context = await pipeline.ExecuteAndCaptureAsync(request, cancellationToken).ConfigureAwait(false);

            return ThrowIfContextHasErrorOrReturnResponse(context);
        }

        /// <inheritdoc/>
        public Task<IInvocationContext<TResponse>> ExecuteAndCaptureAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            return HandleAndCaptureAsync<TResponse>(request, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<TResponse> ExecuteAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            return HandleAsync<TResponse>(request, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<IInvocationContext> InvokeAndCaptureAsync(ICommand command, CancellationToken cancellationToken = default)
        {
            return await HandleAndCaptureAsync<CommandResponse>(command, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public Task InvokeAsync(ICommand command, CancellationToken cancellationToken = default)
        {
            return HandleAsync<CommandResponse>(command, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<IInvocationContext> PublishAndCaptureAsync(IEvent @event, CancellationToken cancellationToken = default)
        {
            return await HandleAndCaptureAsync<EventResponse>(@event, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public Task PublishAsync(IEvent @event, CancellationToken cancellationToken = default)
        {
            return HandleAsync<EventResponse>(@event, cancellationToken);
        }

        private static void ThrowIfContextHasError(IInvocationContext context)
        {
            if (context.HasError)
            {
                // If an error occured, then throw it.
                throw context.Error!;
            }
        }

        private static TResponse ThrowIfContextHasErrorOrReturnResponse<TResponse>(IInvocationContext<TResponse> context)
        {
            ThrowIfContextHasError(context);

#pragma warning disable CS8603 // Possible null reference return.
            // If the context is successful, then return the response
            return context.HasResponse ? context.Response! : default;
#pragma warning restore CS8603 // Possible null reference return.
        }
    }
}
