using Crucible.Mediator.Requests;

namespace Crucible.Mediator.Invocation.Strategies
{
    /// <summary>
    /// <see cref="IRequestHandlerStrategy{TRequest, TResponse}"/> executing multiple <see cref="IRequestHandler{TRequest, TResponse}"/> in parallel.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class ParallelMultiRequestHandlerStrategy<TRequest, TResponse> : RequestHandlerStrategy<TRequest, TResponse>
    {
        private readonly IEnumerable<IRequestHandler<TRequest, TResponse>> _handlers;

        /// <summary>
        /// Initializes a new <see cref="ParallelMultiRequestHandlerStrategy{TRequest, TResponse}"/> instance.
        /// </summary>
        /// <param name="handlers">The underlying <see cref="IRequestHandler{TRequest, TResponse}"/> instances.</param>
        public ParallelMultiRequestHandlerStrategy(
            IEnumerable<IRequestHandler<TRequest, TResponse>> handlers)
        {
            _handlers = handlers;
        }

        /// <inheritdoc/>
        public override Task ExecuteAsync(IInvocationContext<TRequest, TResponse> context, CancellationToken cancellationToken = default)
        {
            var handlerTasks = _handlers.Select(h => ExecuteHandlerAsync(h, context, cancellationToken));
            return Task.WhenAll(handlerTasks);
        }
    }
}
