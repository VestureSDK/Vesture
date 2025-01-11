using Crucible.Mediator.Abstractions.Tests.Internal;
using Crucible.Mediator.Engine.Pipeline.Internal;
using Crucible.Mediator.Engine.Tests.Pipeline.Bases;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Internal
{
    [ImplementationTest]
    public class DefaultPrePipelineAndHandlerMiddlewareTest_PreHandler : PreHandlerMiddlewareConformanceTestBase<DefaultPrePipelineAndHandlerMiddleware>
    {
        protected override DefaultPrePipelineAndHandlerMiddleware CreateInvocationMiddleware() => new();
    }

    [ImplementationTest]
    public class DefaultPrePipelineAndHandlerMiddlewareTest_PrePipeline : PrePipelineMiddlewareConformanceTestBase<DefaultPrePipelineAndHandlerMiddleware>
    {
        protected override DefaultPrePipelineAndHandlerMiddleware CreateInvocationMiddleware() => new();
    }
}
