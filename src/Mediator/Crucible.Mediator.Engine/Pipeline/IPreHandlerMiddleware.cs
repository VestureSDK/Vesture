using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Engine.Pipeline
{
    public interface IPreHandlerMiddleware : IInvocationMiddleware<object, object>
    {
    }
}
