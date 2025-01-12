using Crucible.Mediator.Abstractions.Tests.Internal;
using Crucible.Mediator.Abstractions.Tests.Invocation.Bases;
using Crucible.Mediator.Abstractions.Tests.Invocation.Mocks;
using Crucible.Mediator.Invocation;

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
