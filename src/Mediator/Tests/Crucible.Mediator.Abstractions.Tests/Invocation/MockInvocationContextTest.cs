using Crucible.Mediator.Invocation;
using Crucible.Mediator.Mocks.Invocation;
using Crucible.Testing.Annotations;

namespace Crucible.Mediator.Abstractions.Tests.Invocation
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
