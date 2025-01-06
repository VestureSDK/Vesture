using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Engine.Pipeline.Invocation
{
    public interface IPreHandlerMiddleware : IInvocationMiddleware<object, object>
    {
    }
}
