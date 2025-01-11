using Crucible.Mediator.Abstractions.Tests.Internal;
using Crucible.Mediator.Abstractions.Tests.Invocation.Bases;
using Crucible.Mediator.Abstractions.Tests.Invocation.Mocks;

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
