using Crucible.Mediator.Engine.Tests.Pipeline.Bases;
using Crucible.Mediator.Engine.Tests.Pipeline.Mocks;

namespace Crucible.Mediator.Engine.Tests.Pipeline
{
    public class MockPreHandlerMiddlewareTest : PreHandlerMiddlewareTestBase<MockPreHandlerMiddleware>
    {
        protected override MockPreHandlerMiddleware CreateMiddleware() => new();
    }
}
