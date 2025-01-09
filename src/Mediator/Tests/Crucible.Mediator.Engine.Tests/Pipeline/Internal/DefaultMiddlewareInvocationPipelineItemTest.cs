using Crucible.Mediator.Abstractions.Tests.Invocation.Mocks;
using Crucible.Mediator.Engine.Pipeline.Internal;
using Crucible.Mediator.Engine.Tests.Pipeline.Internal.Bases;
using Crucible.Mediator.Engine.Tests.Pipeline.Resolvers.Mocks;
using Crucible.Mediator.Invocation;
using Any = object;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Internal
{
    public class DefaultMiddlewareInvocationPipelineItemTest : MiddlewareInvocationPipelineItemTestBase<object, object, DefaultMiddlewareInvocationPipelineItem<object, object>>
    {
        protected MockInvocationContext<object, object> InvocationContext { get; }

        protected MockInvocationComponentResolver<IInvocationMiddleware<Any, Any>> InvocationComponentResolver { get; } = new();

        public DefaultMiddlewareInvocationPipelineItemTest()
        {
            InvocationContext = new(new());
            InvocationComponentResolver = new();
        }

        protected override DefaultMiddlewareInvocationPipelineItem<object, object> CreateMiddlewareItem(int order) => new DefaultMiddlewareInvocationPipelineItem<object, object>(order, InvocationComponentResolver);

        protected override IMiddlewareInvocationPipelineItem CreateItemForMiddlewareSignature<TRequest, TResponse>()
        {
            var resolver = new MockInvocationComponentResolver<IInvocationMiddleware<TRequest, TResponse>>();
            return new DefaultMiddlewareInvocationPipelineItem<TRequest, TResponse>(order: 0, resolver);
        }
    }
}
