using Vesture.Mediator.Mocks.Invocation;
using Vesture.Testing.Annotations;

namespace Vesture.Mediator.Abstractions.Tests.Invocation
{
    [MockTest]
    public class MockInvocationHandlerTest
        : InvocationHandlerConformanceTestBase<
            MockContract,
            MockContract,
            MockInvocationHandler<MockContract, MockContract>
        >
    {
        public MockInvocationHandlerTest()
            : base(new()) { }

        protected override MockInvocationHandler<
            MockContract,
            MockContract
        > CreateInvocationHandler() => new();
    }
}
