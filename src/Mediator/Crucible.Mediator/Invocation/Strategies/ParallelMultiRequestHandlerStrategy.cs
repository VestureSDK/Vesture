using Crucible.Mediator.Requests;

namespace Crucible.Mediator.Invocation.Strategies
{
    public class ParallelMultiRequestHandlerStrategy<TRequest, TResponse> : IRequestHandlerStrategy<TRequest, TResponse>
    {
        private readonly IEnumerable<IRequestHandler<TRequest, TResponse>> _handlers;

        public ParallelMultiRequestHandlerStrategy(
            IEnumerable<IRequestHandler<TRequest, TResponse>> handlers)
        {
            _handlers = handlers;
        }

        public Task ExecuteAsync(IInvocationContext<TRequest, TResponse> context, CancellationToken cancellationToken = default)
        {
            var handlerTasks = _handlers.Select(async h =>
            {
                await h.ExecuteAsync(context, cancellationToken);
            });

            return Task.WhenAll(handlerTasks);
        }
    }
}
