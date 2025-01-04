using Crucible.Mediator.Invocation.Accessors;
using Crucible.Mediator.Requests;

namespace Crucible.Mediator.Invocation.Strategies
{
    /// <summary>
    /// <see cref="IRequestHandlerStrategy{TRequest, TResponse}"/> executing multiple <see cref="IRequestHandler{TRequest, TResponse}"/> sequentially.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class SequentialMultiRequestHandlerStrategy<TRequest, TResponse> : RequestHandlerStrategy<TRequest, TResponse>
    {
        private readonly IEnumerable<IInvocationComponentAccessor<IInvocationHandler<TRequest, TResponse>>> _handlersAccessor;

        /// <summary>
        /// Initializes a new <see cref="SequentialMultiRequestHandlerStrategy{TRequest, TResponse}"/> instance.
        /// </summary>
        /// <param name="handlers">The underlying <see cref="IRequestHandler{TRequest, TResponse}"/> instances.</param>
        public SequentialMultiRequestHandlerStrategy(IEnumerable<IInvocationComponentAccessor<IInvocationHandler<TRequest, TResponse>>> handlersAccessor)
        {
            _handlersAccessor = handlersAccessor;
        }

        /// <inheritdoc/>
        public override async Task ExecuteAsync(IInvocationContext<TRequest, TResponse> context, CancellationToken cancellationToken = default)
        {
            foreach (var handlerAccessor in _handlersAccessor)
            {
                var handler = handlerAccessor.GetComponent();
                await ExecuteHandlerAsync(handler, context, cancellationToken).ConfigureAwait(false);
                if (!context.IsSuccess)
                {
                    break;
                }
            }
        }
    }
}
