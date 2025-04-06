using Vesture.Mediator.Abstractions.Tests.Invocation;
using Vesture.Mediator.Engine.Pipeline.Context;
using Vesture.Mediator.Invocation;
using Vesture.Testing.Annotations;

namespace Vesture.Mediator.Engine.Tests.Pipeline.Context
{
    [ImplementationTest]
    public class DefaultInvocationContextTest : InvocationContextConformanceTestBase
    {
        protected override IInvocationContext<TRequest, TResponse> CreateInvocationContext<
            TRequest,
            TResponse
        >(TRequest request) => new DefaultInvocationContext<TRequest, TResponse>(request);
    }
}
