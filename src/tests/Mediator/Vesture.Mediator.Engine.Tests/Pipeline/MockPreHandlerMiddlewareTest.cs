using Vesture.Mediator.Engine.Mocks.Pipeline;
using Vesture.Testing.Annotations;

namespace Vesture.Mediator.Engine.Tests.Pipeline
{
    [MockTest]
    public class MockPreHandlerMiddlewareTest
        : PreHandlerMiddlewareConformanceTestBase<MockPreHandlerMiddleware>
    {
        protected override MockPreHandlerMiddleware CreateInvocationMiddleware() => new();
    }
}
