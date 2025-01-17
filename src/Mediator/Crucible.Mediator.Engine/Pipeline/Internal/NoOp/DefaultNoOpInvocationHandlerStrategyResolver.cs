using Crucible.Mediator.Engine.Pipeline.Strategies;
using Crucible.Mediator.Events;
using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Engine.Pipeline.Internal.NoOp
{
    public class DefaultNoOpInvocationHandlerStrategyResolver : INoOpInvocationHandlerStrategyResolver
    {
        public IInvocationHandlerStrategy<object, TResponse> ResolveNoOpInvocationHandlerStrategy<TResponse>()
        {
            return new NoOpInvocationHandlerStrategy<TResponse>();
        }

        private class NoOpInvocationHandlerStrategy<TResponse> : IInvocationHandlerStrategy<object, TResponse>
        {
            public Task HandleAsync(IInvocationContext<object, TResponse> context, Func<CancellationToken, Task> next, CancellationToken cancellationToken)
            {
                if (typeof(TResponse) != EventResponse.Type)
                {
                    context.AddError(new KeyNotFoundException($"No relevant invocation pipeline found for contract '{context.RequestType.Name} -> {typeof(TResponse).Name}'."));
                }

                return Task.CompletedTask;
            }
        }
    }
}
