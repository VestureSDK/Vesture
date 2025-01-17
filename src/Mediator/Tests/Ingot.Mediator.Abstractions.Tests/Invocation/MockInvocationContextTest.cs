using Ingot.Mediator.Invocation;
using Ingot.Mediator.Mocks.Invocation;
using Ingot.Testing.Annotations;

namespace Ingot.Mediator.Abstractions.Tests.Invocation
{
    [MockTest]
    public class MockInvocationContextTest : InvocationContextConformanceTestBase
    {
        protected override IInvocationContext<TRequest, TResponse> CreateInvocationContext<TRequest, TResponse>(TRequest request)
        {
            return new MockInvocationContext<TRequest, TResponse>
            {
                Request = request!,
            };
        }
    }
}
