using Crucible.Mediator.Engine.Pipeline.Components.Resolvers;
using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Engine.Pipeline.Invocation.Strategies
{
    public class SequentialHandlersStrategy<TRequest, TResponse> : IInvocationHandlerStrategy<TRequest, TResponse>
    {
        private readonly IEnumerable<IInvocationComponentResolver<IInvocationHandler<TRequest, TResponse>>> _resolvers;

        public SequentialHandlersStrategy(IEnumerable<IInvocationComponentResolver<IInvocationHandler<TRequest, TResponse>>> resolvers)
        {
            _resolvers = resolvers;
        }

        public async Task HandleAsync(IInvocationContext<TRequest, TResponse> context, Func<CancellationToken, Task> next, CancellationToken cancellationToken)
        {
            foreach (var resolver in _resolvers)
            {
                await SingleHandlerStrategy<TRequest, TResponse>.InvokeHandlerAsync(resolver, context, cancellationToken);
            }
        }
    }
}
