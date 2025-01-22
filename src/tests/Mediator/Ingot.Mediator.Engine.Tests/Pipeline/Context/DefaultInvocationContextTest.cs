using Ingot.Mediator.Abstractions.Tests.Invocation;
using Ingot.Mediator.Engine.Pipeline.Context;
using Ingot.Mediator.Invocation;
using Ingot.Testing.Annotations;

namespace Ingot.Mediator.Engine.Tests.Pipeline.Context
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
