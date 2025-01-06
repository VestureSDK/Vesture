using Crucible.Mediator.Engine.Pipeline;

namespace Crucible.Mediator.Engine.Invocation.Strategies
{
    public interface IInvocationHandlerStrategy<TRequest, TResponse> : IInvocationPipelineItem<TRequest, TResponse>
    {

    }
}
