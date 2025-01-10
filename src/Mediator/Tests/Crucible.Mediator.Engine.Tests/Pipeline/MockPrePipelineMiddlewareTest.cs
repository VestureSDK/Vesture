using Crucible.Mediator.Engine.Tests.Pipeline.Bases;
using Crucible.Mediator.Engine.Tests.Pipeline.Mocks;

namespace Crucible.Mediator.Engine.Tests.Pipeline
{
    public class MockPrePipelineMiddlewareTest : PrePipelineMiddlewareTestBase<MockPrePipelineMiddleware>
    {
        protected override MockPrePipelineMiddleware CreateMiddleware() => new();
    }
}
