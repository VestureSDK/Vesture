using Ingot.Mediator.Engine.Mocks.Pipeline;
using Ingot.Testing.Annotations;

namespace Ingot.Mediator.Engine.Tests.Pipeline
{
    [MockTest]
    public class MockPreHandlerMiddlewareTest
        : PreHandlerMiddlewareConformanceTestBase<MockPreHandlerMiddleware>
    {
        protected override MockPreHandlerMiddleware CreateInvocationMiddleware() => new();
    }
}
