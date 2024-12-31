using Crucible.Mediator.Invocation.Accessors;
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
        private readonly IEnumerable<IInvocationComponentAccessor<IRequestHandler<TRequest, TResponse>>> _handlersAccessor;

        /// <summary>
        /// Initializes a new <see cref="ParallelMultiRequestHandlerStrategy{TRequest, TResponse}"/> instance.
        /// </summary>
        /// <param name="handlers">The underlying <see cref="IRequestHandler{TRequest, TResponse}"/> instances.</param>
        public ParallelMultiRequestHandlerStrategy(IEnumerable<IInvocationComponentAccessor<IRequestHandler<TRequest, TResponse>>> handlersAccessor)
        {
            _handlersAccessor = handlersAccessor;
        }

        /// <inheritdoc/>
        public override Task ExecuteAsync(IInvocationContext<TRequest, TResponse> context, CancellationToken cancellationToken = default)
        {
            var handlerTasks = _handlersAccessor.Select(ha =>
            {
                var handler = ha.GetComponent();
                return ExecuteHandlerAsync(handler, context, cancellationToken);
            });

            return Task.WhenAll(handlerTasks);
        }
    }
}
