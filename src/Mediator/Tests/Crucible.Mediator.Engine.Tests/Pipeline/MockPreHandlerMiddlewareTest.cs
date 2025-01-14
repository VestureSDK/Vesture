using Crucible.Mediator.Engine.Mocks.Pipeline;
using Crucible.Testing.Annotations;

namespace Crucible.Mediator.Engine.Tests.Pipeline
{
    [MockTest]
    public class MockPreHandlerMiddlewareTest : PreHandlerMiddlewareConformanceTestBase<MockPreHandlerMiddleware>
    {
        protected override MockPreHandlerMiddleware CreateInvocationMiddleware() => new();
    }
}
