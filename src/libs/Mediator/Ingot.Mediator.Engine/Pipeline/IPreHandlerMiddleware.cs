using Ingot.Mediator.Engine.Pipeline.Internal;
using Ingot.Mediator.Invocation;

namespace Ingot.Mediator.Engine.Pipeline
{
    /// <summary>
    /// <para>
    /// A <see cref="IPreHandlerMiddleware"/> is a specific <see cref="IInvocationMiddleware{TRequest, TResponse}"/>
    /// added by <see cref="DefaultInvocationPipeline{TRequest, TResponse}"/> right before calling the
    /// <see cref="IInvocationHandler{TRequest, TResponse}"/>.
    /// </para>
    /// </summary>
    /// <seealso cref="DefaultPrePipelineAndHandlerMiddleware"/>
    /// <seealso cref="IInvocationMiddleware{TRequest, TResponse}"/>
    public interface IPreHandlerMiddleware : IInvocationMiddleware<object, object> { }
}
