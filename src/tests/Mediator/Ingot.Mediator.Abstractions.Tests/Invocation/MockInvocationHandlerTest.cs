using Ingot.Mediator.Mocks.Invocation;
using Ingot.Testing.Annotations;

namespace Ingot.Mediator.Abstractions.Tests.Invocation
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
