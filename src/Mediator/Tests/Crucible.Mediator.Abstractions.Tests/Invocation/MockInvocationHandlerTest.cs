using Crucible.Mediator.Mocks.Invocation;
using Crucible.Testing.Annotations;

namespace Crucible.Mediator.Abstractions.Tests.Invocation
{
    [MockTest]
    public class MockInvocationHandlerTest : InvocationHandlerConformanceTestBase<MockContract, MockContract, MockInvocationHandler<MockContract, MockContract>>
    {
        public MockInvocationHandlerTest()
            : base(new())
        {
        }

        protected override MockInvocationHandler<MockContract, MockContract> CreateInvocationHandler() => new();
    }
}
