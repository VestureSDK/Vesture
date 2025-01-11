using Crucible.Mediator.Abstractions.Tests.Invocation.Bases;
using Crucible.Mediator.Engine.Pipeline.Context;
using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Context
{
    public class DefaultInvocationContextTest : InvocationContextConformanceTestBase
    {
        protected override IInvocationContext<TRequest, TResponse> CreateInvocationContext<TRequest, TResponse>(TRequest request) => new DefaultInvocationContext<TRequest, TResponse>(request);
    }
}
