using Crucible.Mediator.Mocks.Invocation;
using Crucible.Testing.Annotations;

namespace Crucible.Mediator.Abstractions.Tests.Invocation
{
    [MockTest]
    public class MockInvocationMiddlewareTest : InvocationMiddlewareConformanceTestBase<MockContract, MockContract, MockInvocationMiddleware<MockContract, MockContract>>
    {
        public MockInvocationMiddlewareTest()
            : base(new()) { }

        protected override MockInvocationMiddleware<MockContract, MockContract> CreateInvocationMiddleware() => new MockInvocationMiddleware<MockContract, MockContract>();
    }
}
