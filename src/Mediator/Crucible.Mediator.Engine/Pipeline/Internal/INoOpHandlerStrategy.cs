using Crucible.Mediator.Engine.Pipeline.Strategies;

namespace Crucible.Mediator.Engine.Pipeline.Internal
{
    public interface INoOpHandlerStrategy<TResponse> : IInvocationHandlerStrategy<object, TResponse>
    {

    }
}
