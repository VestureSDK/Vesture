using Vesture.Mediator.Engine.Mocks.Pipeline;
using Vesture.Testing.Annotations;

namespace Vesture.Mediator.Engine.Tests.Pipeline
{
    [MockTest]
    public class MockPrePipelineMiddlewareTest
        : PrePipelineMiddlewareConformanceTestBase<MockPrePipelineMiddleware>
    {
        protected override MockPrePipelineMiddleware CreateInvocationMiddleware() => new();
    }
}
