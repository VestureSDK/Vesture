using Crucible.Mediator.Abstractions.Tests.Internal;
using Crucible.Mediator.Engine.Tests.Pipeline.Bases;
using Crucible.Mediator.Engine.Tests.Pipeline.Mocks;

namespace Crucible.Mediator.Engine.Tests.Pipeline
{
    [MockTest]
    public class MockPrePipelineMiddlewareTest : PrePipelineMiddlewareConformanceTestBase<MockPrePipelineMiddleware>
    {
        protected override MockPrePipelineMiddleware CreateInvocationMiddleware() => new();
    }
}
