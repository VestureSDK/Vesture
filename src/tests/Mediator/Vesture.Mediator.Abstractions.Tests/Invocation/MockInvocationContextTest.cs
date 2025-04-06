using Vesture.Mediator.Invocation;
using Vesture.Mediator.Mocks.Invocation;
using Vesture.Testing.Annotations;

namespace Vesture.Mediator.Abstractions.Tests.Invocation
{
    [MockTest]
    public class MockInvocationContextTest : InvocationContextConformanceTestBase
    {
        protected override IInvocationContext<TRequest, TResponse> CreateInvocationContext<
            TRequest,
            TResponse
        >(TRequest request)
        {
            return new MockInvocationContext<TRequest, TResponse> { Request = request! };
        }
    }
}
