using Crucible.Mediator.Engine.Pipeline.Components.Resolvers;
using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Engine.Pipeline.Invocation.Strategies
{
    public class ParallelHandlersStrategy<TRequest, TResponse> : IInvocationHandlerStrategy<TRequest, TResponse>
    {
        private readonly IEnumerable<IInvocationComponentResolver<IInvocationHandler<TRequest, TResponse>>> _resolvers;

        public ParallelHandlersStrategy(IEnumerable<IInvocationComponentResolver<IInvocationHandler<TRequest, TResponse>>> resolvers)
        {
            _resolvers = resolvers;
        }

        public Task HandleAsync(IInvocationContext<TRequest, TResponse> context, Func<CancellationToken, Task> next, CancellationToken cancellationToken)
        {
            var tasks = _resolvers.Select(resolver => SingleHandlerStrategy<TRequest, TResponse>.InvokeHandlerAsync(resolver, context, cancellationToken));
            return Task.WhenAll(tasks);
        }
    }
}
