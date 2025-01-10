using Crucible.Mediator.Engine.Pipeline.Internal;
using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Engine.Pipeline
{
    /// <summary>
    /// <para>
    /// A <see cref="IPrePipelineMiddleware"/> is a specific <see cref="IInvocationMiddleware{TRequest, TResponse}"/>
    /// added by <see cref="DefaultInvocationPipeline{TRequest, TResponse}"/> at the beginning of the pipeline.
    /// </para>
    /// </summary>
    /// <seealso cref="DefaultPrePipelineAndHandlerMiddleware"/>
    /// <seealso cref="IInvocationMiddleware{TRequest, TResponse}"/>
    public interface IPrePipelineMiddleware : IInvocationMiddleware<object, object>
    {

    }
}
