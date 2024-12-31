using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Events
{
    /// <summary>
    /// Default implementation of <see cref="IEventPublisher"/>.
    /// </summary>
    /// <remarks>
    /// It uses <see cref="IInvocationPipelineProvider"/> under the hood to resolve an <see cref="InvocationPipeline{TResponse}"/>
    /// and execute the middleware and handler related to an <see cref="IEvent"/>.
    /// </remarks>
    internal class EventPublisher : Invoker, IEventPublisher
    {
        /// <summary>
        /// Initializes a new <see cref="EventPublisher"/> instance.
        /// </summary>
        /// <param name="pipelineProvider">The <see cref="IInvocationPipelineProvider"/> instance.</param>
        public EventPublisher(IInvocationPipelineProvider pipelineProvider)
            : base(pipelineProvider) { }

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
    }
}
