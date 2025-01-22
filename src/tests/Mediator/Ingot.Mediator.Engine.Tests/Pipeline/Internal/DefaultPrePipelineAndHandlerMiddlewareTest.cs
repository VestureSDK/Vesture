using Ingot.Mediator.Engine.Pipeline.Internal;
using Ingot.Testing;
using Ingot.Testing.Annotations;

namespace Ingot.Mediator.Engine.Tests.Pipeline.Internal
{
    [ImplementationTest]
    public class DefaultPrePipelineAndHandlerMiddlewareTest_PreHandler : PreHandlerMiddlewareConformanceTestBase<DefaultPrePipelineAndHandlerMiddleware>
    {
        protected NUnitTestContextMsLogger<DefaultPrePipelineAndHandlerMiddleware> Logger { get; } = new();

        protected override DefaultPrePipelineAndHandlerMiddleware CreateInvocationMiddleware() => new(Logger);
    }

    [ImplementationTest]
    public class DefaultPrePipelineAndHandlerMiddlewareTest_PrePipeline : PrePipelineMiddlewareConformanceTestBase<DefaultPrePipelineAndHandlerMiddleware>
    {
        protected NUnitTestContextMsLogger<DefaultPrePipelineAndHandlerMiddleware> Logger { get; } = new();

        protected override DefaultPrePipelineAndHandlerMiddleware CreateInvocationMiddleware() => new(Logger);
    }
}
