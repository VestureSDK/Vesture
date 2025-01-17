using Crucible.Mediator.Engine.Pipeline.Strategies;

namespace Crucible.Mediator.Engine.Pipeline.Internal.NoOp
{
    public interface INoOpInvocationHandlerStrategyResolver
    {
        IInvocationHandlerStrategy<object, TResponse> ResolveNoOpInvocationHandlerStrategy<TResponse>();
    }
}
