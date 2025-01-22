using Ingot.Mediator.Mocks.Invocation;
using Ingot.Testing.Annotations;

namespace Ingot.Mediator.Abstractions.Tests.Invocation
{
    [MockTest]
    public class MockInvocationMiddlewareTest : InvocationMiddlewareConformanceTestBase<MockContract, MockContract, MockInvocationMiddleware<MockContract, MockContract>>
    {
        public MockInvocationMiddlewareTest()
            : base(new()) { }

        protected override MockInvocationMiddleware<MockContract, MockContract> CreateInvocationMiddleware() => new MockInvocationMiddleware<MockContract, MockContract>();
    }
}
