using Crucible.Mediator.Requests;

namespace Crucible.Mediator.Invocation.Strategies
{
    /// <summary>
    /// <see cref="IRequestHandlerStrategy{TRequest, TResponse}"/> executing multiple <see cref="IRequestHandler{TRequest, TResponse}"/> sequentially.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class SequentialMultiRequestHandlerStrategy<TRequest, TResponse> : IRequestHandlerStrategy<TRequest, TResponse>
    {
        private readonly IEnumerable<IRequestHandler<TRequest, TResponse>> _handlers;

        /// <summary>
        /// Initializes a new <see cref="SequentialMultiRequestHandlerStrategy{TRequest, TResponse}"/> instance.
        /// </summary>
        /// <param name="handlers">The underlying <see cref="IRequestHandler{TRequest, TResponse}"/> instances.</param>
        public SequentialMultiRequestHandlerStrategy(
            IEnumerable<IRequestHandler<TRequest, TResponse>> handlers)
        {
            _handlers = handlers;
        }

        /// <inheritdoc/>
        public async Task ExecuteAsync(IInvocationContext<TRequest, TResponse> context, CancellationToken cancellationToken = default)
        {
            foreach (var handler in _handlers)
            {
                await handler.ExecuteAsync(context, cancellationToken);
            }
        }
    }
}
