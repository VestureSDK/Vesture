using Crucible.Mediator.Abstractions.Tests.Invocation.Bases;
using Crucible.Mediator.Abstractions.Tests.Invocation.Mocks;

namespace Crucible.Mediator.Abstractions.Tests.Invocation
{
    public class MockInvocationMiddlewareTest : InvocationMiddlewareTestBase<MockContract, MockContract, MockInvocationMiddleware<MockContract, MockContract>>
    {
        public MockInvocationMiddlewareTest()
            : base(new()) { }

        protected override MockInvocationMiddleware<MockContract, MockContract> CreateMiddleware() => new MockInvocationMiddleware<MockContract, MockContract>();
    }
}
