using Vesture.Mediator.Mocks.Invocation;
using Vesture.Testing.Annotations;

namespace Vesture.Mediator.Abstractions.Tests.Invocation
{
    [MockTest]
    public class MockInvocationMiddlewareTest
        : InvocationMiddlewareConformanceTestBase<
            MockContract,
            MockContract,
            MockInvocationMiddleware<MockContract, MockContract>
        >
    {
        public MockInvocationMiddlewareTest()
            : base(new()) { }

        protected override MockInvocationMiddleware<
            MockContract,
            MockContract
        > CreateInvocationMiddleware() =>
            new MockInvocationMiddleware<MockContract, MockContract>();
    }
}
