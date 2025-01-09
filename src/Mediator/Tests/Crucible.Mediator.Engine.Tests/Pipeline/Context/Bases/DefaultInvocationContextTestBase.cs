using Crucible.Mediator.Abstractions.Tests.Invocation.Bases;
using Crucible.Mediator.Engine.Pipeline.Context;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Context.Bases
{
    public class DefaultInvocationContextTestBase<TRequest, TResponse> : InvocationContextTestBase<TRequest, TResponse, DefaultInvocationContext<TRequest, TResponse>>
    {
        public DefaultInvocationContextTestBase(TRequest defaultRequest)
            : base(defaultRequest)
        {
        }

        protected override DefaultInvocationContext<TRequest, TResponse> CreateInvocationContext(TRequest request) => new (Request);
    }
}
