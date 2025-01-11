using Crucible.Mediator.Abstractions.Tests.Invocation.Bases;
using Crucible.Mediator.Abstractions.Tests.Invocation.Mocks;

namespace Crucible.Mediator.Abstractions.Tests.Invocation
{
    public class MockInvocationHandlerTest : InvocationHandlerConformanceTestBase<MockContract, MockContract, MockInvocationHandler<MockContract, MockContract>>
    {
        public MockInvocationHandlerTest()
            : base(new())
        {
        }

        protected override MockInvocationHandler<MockContract, MockContract> CreateInvocationHandler() => new();
    }
}
