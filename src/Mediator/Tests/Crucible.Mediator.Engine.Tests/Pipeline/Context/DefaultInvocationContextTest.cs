using Crucible.Mediator.Abstractions.Tests.Invocation;
using Crucible.Mediator.Engine.Pipeline.Context;
using Crucible.Mediator.Invocation;
using Crucible.Testing.Annotations;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Context
{
    [ImplementationTest]
    public class DefaultInvocationContextTest : InvocationContextConformanceTestBase
    {
        protected override IInvocationContext<TRequest, TResponse> CreateInvocationContext<TRequest, TResponse>(TRequest request) => new DefaultInvocationContext<TRequest, TResponse>(request);
    }
}
