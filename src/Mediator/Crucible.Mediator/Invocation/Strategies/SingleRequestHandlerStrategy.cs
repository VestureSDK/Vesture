using Crucible.Mediator.Requests;

namespace Crucible.Mediator.Invocation.Strategies
{
    public class SingleRequestHandlerStrategy<TRequest, TResponse> : IRequestHandlerStrategy<TRequest, TResponse>
    {
        private readonly IRequestHandler<TRequest, TResponse> _handler;

        public SingleRequestHandlerStrategy(
            IRequestHandler<TRequest, TResponse> handler)
        {
            _handler = handler;
        }

        public async Task ExecuteAsync(IInvocationContext<TRequest, TResponse> context, CancellationToken cancellationToken = default)
        {
            await _handler.ExecuteAsync(context, cancellationToken);
        }
    }
}
