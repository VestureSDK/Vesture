using Crucible.Mediator.Engine.Mocks.Pipeline;
using Crucible.Testing.Annotations;

namespace Crucible.Mediator.Engine.Tests.Pipeline
{
    [MockTest]
    public class MockPrePipelineMiddlewareTest : PrePipelineMiddlewareConformanceTestBase<MockPrePipelineMiddleware>
    {
        protected override MockPrePipelineMiddleware CreateInvocationMiddleware() => new();
    }
}
