using Vesture.Mediator.Engine.Pipeline.Internal;
using Vesture.Testing;
using Vesture.Testing.Annotations;

namespace Vesture.Mediator.Engine.Tests.Pipeline.Internal
{
    [ImplementationTest]
    public class DefaultPrePipelineAndHandlerMiddlewareTest_PreHandler
        : PreHandlerMiddlewareConformanceTestBase<DefaultPrePipelineAndHandlerMiddleware>
    {
        protected NUnitTestContextMsLogger<DefaultPrePipelineAndHandlerMiddleware> Logger { get; } =
            new();

        protected override DefaultPrePipelineAndHandlerMiddleware CreateInvocationMiddleware() =>
            new(Logger);
    }

    [ImplementationTest]
    public class DefaultPrePipelineAndHandlerMiddlewareTest_PrePipeline
        : PrePipelineMiddlewareConformanceTestBase<DefaultPrePipelineAndHandlerMiddleware>
    {
        protected NUnitTestContextMsLogger<DefaultPrePipelineAndHandlerMiddleware> Logger { get; } =
            new();

        protected override DefaultPrePipelineAndHandlerMiddleware CreateInvocationMiddleware() =>
            new(Logger);
    }
}
