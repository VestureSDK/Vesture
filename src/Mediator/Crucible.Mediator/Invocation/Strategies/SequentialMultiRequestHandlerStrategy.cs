using Crucible.Mediator.Requests;

namespace Crucible.Mediator.Invocation.Strategies
{
    public class SequentialMultiRequestHandlerStrategy<TRequest, TResponse> : IRequestHandlerStrategy<TRequest, TResponse>
    {
        private readonly IEnumerable<IRequestHandler<TRequest, TResponse>> _handlers;

        public SequentialMultiRequestHandlerStrategy(
            IEnumerable<IRequestHandler<TRequest, TResponse>> handlers)
        {
            _handlers = handlers;
        }

        public async Task ExecuteAsync(IInvocationContext<TRequest, TResponse> context, CancellationToken cancellationToken = default)
        {
            foreach (var handler in _handlers)
            {
                await handler.ExecuteAsync(context, cancellationToken);
            }
        }
    }
}
