using Crucible.Mediator.Engine.Pipeline.Internal;
using Crucible.Mediator.Engine.Tests.Pipeline.Bases;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Internal
{
    public class DefaultPrePipelineAndHandlerMiddlewareTest_PreHandler : PreHandlerMiddlewareTestBase<DefaultPrePipelineAndHandlerMiddleware>
    {
        protected override DefaultPrePipelineAndHandlerMiddleware CreateMiddleware() => new();
    }

    public class DefaultPrePipelineAndHandlerMiddlewareTest_PrePipeline : PrePipelineMiddlewareTestBase<DefaultPrePipelineAndHandlerMiddleware>
    {
        protected override DefaultPrePipelineAndHandlerMiddleware CreateMiddleware() => new();
    }
}
