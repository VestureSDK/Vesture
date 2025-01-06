using System.Collections.Frozen;
using Crucible.Mediator.Commands;
using Crucible.Mediator.Engine.Pipeline;
using Crucible.Mediator.Events;
using Crucible.Mediator.Invocation;
using Crucible.Mediator.Requests;

namespace Crucible.Mediator.Engine
{
    public class DefaultMediator : IMediator
    {
        private readonly Lazy<IDictionary<(Type request, Type response), IInvocationPipeline>> _invocationPipelines;

        public DefaultMediator(IEnumerable<IInvocationPipeline> invocationPipelines)
        {
            _invocationPipelines = new Lazy<IDictionary<(Type request, Type response), IInvocationPipeline>>(() => CreateInvocationPipelineCache(invocationPipelines));
        }

        private IInvocationPipeline<TResponse> GetInvocationPipeline<TResponse>(object request)
        {
            var requestType = request.GetType();
            var responseType = typeof(TResponse);

            return (IInvocationPipeline<TResponse>)_invocationPipelines.Value[(requestType, responseType)];
        }

        /// <inheritdoc/>
        public Task<IInvocationContext<TResponse>> HandleAndCaptureAsync<TResponse>(object request, CancellationToken cancellationToken = default)
        {
            var pipeline = GetInvocationPipeline<TResponse>(request);
            return pipeline.HandleAsync(request, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<TResponse> HandleAsync<TResponse>(object request, CancellationToken cancellationToken = default)
        {
            var context = await HandleAndCaptureAsync<TResponse>(request, cancellationToken).ConfigureAwait(false);
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

        private static FrozenDictionary<(Type request, Type response), IInvocationPipeline> CreateInvocationPipelineCache(IEnumerable<IInvocationPipeline> invocationPipelines)
        {
            var dict = new Dictionary<(Type request, Type response), IInvocationPipeline>();
            foreach (var invocationPipeline in invocationPipelines)
            {
                dict.TryAdd((invocationPipeline.Request, invocationPipeline.Response), invocationPipeline);
            }
            return dict.ToFrozenDictionary();
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
