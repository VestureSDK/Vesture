using Ingot.Mediator.Engine.Mocks.Pipeline;
using Ingot.Testing.Annotations;

namespace Ingot.Mediator.Engine.Tests.Pipeline
{
    [MockTest]
    public class MockPrePipelineMiddlewareTest
        : PrePipelineMiddlewareConformanceTestBase<MockPrePipelineMiddleware>
    {
        protected override MockPrePipelineMiddleware CreateInvocationMiddleware() => new();
    }
}
