using System.Collections.Frozen;
using Ingot.Mediator.Commands;
using Ingot.Mediator.Engine.Pipeline;
using Ingot.Mediator.Engine.Pipeline.Extensions;
using Ingot.Mediator.Engine.Pipeline.Internal.NoOp;
using Ingot.Mediator.Events;
using Ingot.Mediator.Invocation;
using Ingot.Mediator.Requests;
using Microsoft.Extensions.Logging;

namespace Ingot.Mediator.Engine
{
    /// <summary>
    /// <para>
    /// The <see cref="DefaultMediator"/> provides a default implementation of <see cref="IMediator"/>.
    /// </para>
    /// <para>
    /// An <see cref="IMediator"/> coordinates the execution of different types of contracts 
    /// such as requests, commands, and events by invoking the appropriate handlers and middlewares.
    /// </para>
    /// <para>
    /// The mediator acts as a central point of communication in your application, 
    /// decoupling the components that send requests, commands, or events from those that handle them.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="DefaultMediator"/> uses the provided <see cref="IInvocationPipeline{TResponse}"/> 
    /// instances to create an efficient cache based on <see cref="FrozenDictionary{TKey, TValue}"/> to 
    /// lookup the right pipeline for the incoming <see cref="IRequest{TResponse}"/>, <see cref="ICommand"/>, 
    /// <see cref="IEvent"/> and invoke the corresponding <see cref="IInvocationMiddleware{TRequest, TResponse}"/>s
    /// and <see cref="IInvocationHandler{TRequest, TResponse}"/>.
    /// </para>
    /// <para>
    /// By pre-building the pipeline structure, the system optimizes performance by reducing 
    /// runtime resolution overhead, ensuring a more efficient execution flow while maintaining flexibility.
    /// </para>
    /// </remarks>
    /// <seealso cref="IMediator"/>
    /// <seealso cref="IRequest{TResponse}"/>
    /// <seealso cref="ICommand"/>
    /// <seealso cref="IEvent"/>
    /// <seealso cref="CommandHandler{TCommand}"/>
    /// <seealso cref="Ingot.Mediator.Events.EventHandler{TEvent}"/>
    /// <seealso cref="InvocationMiddleware{TRequest, TResponse}"/>
    public class DefaultMediator : IMediator
    {
        private readonly Lazy<IDictionary<(Type request, Type response), IInvocationPipeline>> _invocationPipelines;

        private readonly ILogger _logger;

        private readonly INoOpInvocationPipelineResolver _noOpInvocationPipelineResolver;

        /// <summary>
        /// Initializes a new <see cref="DefaultMediator"/> instance.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger{TCategoryName}"/> instance.</param>
        /// <param name="invocationPipelines">The <see cref="IInvocationPipeline{TResponse}"/> instances.</param>
        /// <param name="noOpInvocationPipelineResolver">The <see cref="INoOpInvocationPipelineResolver"/> instances.</param>
        /// <exception cref="ArgumentNullException"><paramref name="logger"/> is <see langword="null" /> or <paramref name="invocationPipelines"/> is <see langword="null" /> or <paramref name="noOpInvocationPipelineResolver"/> is <see langword="null" />.</exception>
        public DefaultMediator(
            ILogger<DefaultMediator> logger,
            IEnumerable<IInvocationPipeline> invocationPipelines,
            INoOpInvocationPipelineResolver noOpInvocationPipelineResolver)
        {
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));
            ArgumentNullException.ThrowIfNull(invocationPipelines, nameof(invocationPipelines));
            ArgumentNullException.ThrowIfNull(noOpInvocationPipelineResolver, nameof(noOpInvocationPipelineResolver));

            _logger = logger;
            _noOpInvocationPipelineResolver = noOpInvocationPipelineResolver;
            _invocationPipelines = new Lazy<IDictionary<(Type request, Type response), IInvocationPipeline>>(() =>
            {
                using var activity = MediatorEngineDiagnostics.s_mediatorActivitySource
                    .StartActivity("Invocation Pipelines Caching");

                // Set the activity status as error since it will be switched
                // back to "OK" if no errors are thrown.
                activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error);

                var pipelines = CreateInvocationPipelineCache(invocationPipelines);
                _logger.InvocationPipelinesCached(pipelines.Keys);

                // Set the activity status as "OK" since no error has been thrown
                activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Ok);

                return pipelines;
            });
        }

        /// <exception cref="KeyNotFoundException">No relevant invocation pipeline found for contract '<paramref name="request"/> -> <typeparamref name="TResponse"/>'.</exception>
        private IInvocationPipeline<TResponse> GetInvocationPipeline<TResponse>(object request)
        {
            var requestType = request.GetType();
            var responseType = typeof(TResponse);

            if (_invocationPipelines.Value.TryGetValue((requestType, responseType), out var p) && p is IInvocationPipeline<TResponse> pipeline)
            {
                _logger.InvocationPipelineFound<TResponse>(request);
                return pipeline;
            }
            else
            {
                _logger.InvocationPipelineNotFound<TResponse>(request);
                return _noOpInvocationPipelineResolver.ResolveNoOpInvocationPipeline<TResponse>();
            }
        }

        /// <exception cref="ArgumentNullException"><paramref name="contract"/> is <see langword="null" />.</exception>
        /// <exception cref="KeyNotFoundException">No relevant <see cref="IInvocationPipeline{TResponse}"/> found for contract '<paramref name="contract"/> -> <typeparamref name="TResponse"/>'.</exception>
        /// <inheritdoc/>
        public Task<IInvocationContext<TResponse>> HandleAndCaptureAsync<TResponse>(object contract, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(contract, nameof(contract));

            var pipeline = GetInvocationPipeline<TResponse>(contract);
            return pipeline.HandleAsync(contract, cancellationToken);
        }

        /// <inheritdoc cref="HandleAndCaptureAsync{TResponse}(object, CancellationToken)" path="/exception"/>
        /// <inheritdoc cref="InvocationContextExtensions.ThrowIfHasError{TContext}(TContext)" path="/exception[@name='invocation-exception']"/>
        /// <inheritdoc/>
        public async Task<TResponse> HandleAsync<TResponse>(object contract, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(contract, nameof(contract));

            var context = await HandleAndCaptureAsync<TResponse>(contract, cancellationToken).ConfigureAwait(false);
            return context.ThrowIfHasError().GetResponseOrDefault<TResponse>();
        }


        /// <inheritdoc cref="HandleAndCaptureAsync{TResponse}(object, CancellationToken)" path="/exception"/>
        /// <inheritdoc/>
        public Task<IInvocationContext<TResponse>> ExecuteAndCaptureAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            return HandleAndCaptureAsync<TResponse>(request, cancellationToken);
        }

        /// <inheritdoc cref="HandleAndCaptureAsync{TResponse}(object, CancellationToken)" path="/exception"/>
        /// <inheritdoc cref="InvocationContextExtensions.ThrowIfHasError{TContext}(TContext)" path="/exception[@name='invocation-exception']"/>
        /// <inheritdoc/>
        public Task<TResponse> ExecuteAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            return HandleAsync<TResponse>(request, cancellationToken);
        }

        /// <exception cref="ArgumentNullException"><paramref name="command"/> is <see langword="null" />.</exception>
        /// <exception cref="KeyNotFoundException">No relevant <see cref="IInvocationPipeline{TResponse}"/> found for contract '<paramref name="command"/> -> <see cref="CommandResponse"/>'.</exception>
        /// <inheritdoc/>
        public async Task<IInvocationContext> InvokeAndCaptureAsync(ICommand command, CancellationToken cancellationToken = default)
        {
            return await HandleAndCaptureAsync<CommandResponse>(command, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc cref="InvokeAndCaptureAsync(ICommand, CancellationToken)" path="/exception"/>
        /// <inheritdoc cref="InvocationContextExtensions.ThrowIfHasError{TContext}(TContext)" path="/exception[@name='invocation-exception']"/>
        /// <inheritdoc/>
        public Task InvokeAsync(ICommand command, CancellationToken cancellationToken = default)
        {
            return HandleAsync<CommandResponse>(command, cancellationToken);
        }

        /// <exception cref="ArgumentNullException"><paramref name="event"/> is <see langword="null" />.</exception>
        /// <exception cref="KeyNotFoundException">No relevant <see cref="IInvocationPipeline{TResponse}"/> found for contract '<paramref name="event"/> -> <see cref="EventResponse"/>'.</exception>
        /// <inheritdoc/>
        public async Task<IInvocationContext> PublishAndCaptureAsync(IEvent @event, CancellationToken cancellationToken = default)
        {
            return await HandleAndCaptureAsync<EventResponse>(@event, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc cref="PublishAndCaptureAsync(IEvent, CancellationToken)" path="/exception"/>
        /// <inheritdoc cref="InvocationContextExtensions.ThrowIfHasError{TContext}(TContext)" path="/exception[@name='invocation-exception']"/>
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
                dict[(invocationPipeline.RequestType, invocationPipeline.ResponseType)] = invocationPipeline;
            }

            return dict.ToFrozenDictionary();
        }
    }
}
