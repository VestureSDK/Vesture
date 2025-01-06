using System.Collections.Frozen;
using Crucible.Mediator.Commands;
using Crucible.Mediator.Engine.Pipeline;
using Crucible.Mediator.Events;
using Crucible.Mediator.Invocation;
using Crucible.Mediator.Requests;

namespace Crucible.Mediator.Engine
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
    /// <seealso cref="RequestHandler{TRequest, TResponse}"/>
    /// <seealso cref="CommandHandler{TCommand}"/>
    /// <seealso cref="Crucible.Mediator.Events.EventHandler{TEvent}"/>
    /// <seealso cref="InvocationMiddleware{TRequest, TResponse}"/>
    public class DefaultMediator : IMediator
    {
        private readonly Lazy<IDictionary<(Type request, Type response), IInvocationPipeline>> _invocationPipelines;

        /// <summary>
        /// Initializes a new <see cref="DefaultMediator"/> instance.
        /// </summary>
        /// <param name="invocationPipelines">The <see cref="IInvocationPipeline{TResponse}"/> instances.</param>
        /// <exception cref="ArgumentNullException"><paramref name="invocationPipelines"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException"><paramref name="invocationPipelines"/> is empty.</exception>
        public DefaultMediator(IEnumerable<IInvocationPipeline> invocationPipelines)
        {
            ArgumentNullException.ThrowIfNull(invocationPipelines, nameof(invocationPipelines));
            if (!invocationPipelines.Any())
            {
                throw new ArgumentException($"{nameof(invocationPipelines)} is empty", nameof(invocationPipelines));
            }

            _invocationPipelines = new Lazy<IDictionary<(Type request, Type response), IInvocationPipeline>>(() => CreateInvocationPipelineCache(invocationPipelines));
        }

        /// <exception cref="KeyNotFoundException">No relevant invocation pipeline found for contract '<paramref name="request"/> -> <typeparamref name="TResponse"/>'.</exception>
        private IInvocationPipeline<TResponse> GetInvocationPipeline<TResponse>(object request)
        {
            var requestType = request.GetType();
            var responseType = typeof(TResponse);

            if (_invocationPipelines.Value.TryGetValue((requestType, responseType), out var p) && p is IInvocationPipeline<TResponse> pipeline)
            {
                return pipeline;
            }
            else
            {
                throw new KeyNotFoundException($"No relevant invocation pipeline found for contract '{requestType.Name} -> {responseType.Name}'.");
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
                dict[(invocationPipeline.Request, invocationPipeline.Response)] = invocationPipeline;
            }

            return dict.ToFrozenDictionary();
        }
    }
}
