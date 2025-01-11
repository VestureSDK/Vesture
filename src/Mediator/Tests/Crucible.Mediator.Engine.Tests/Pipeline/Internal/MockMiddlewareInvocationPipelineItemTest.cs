using Crucible.Mediator.Abstractions.Tests.Invocation.Mocks;
using Crucible.Mediator.Engine.Pipeline.Internal;
using Crucible.Mediator.Engine.Tests.Pipeline.Internal.Bases;
using Crucible.Mediator.Engine.Tests.Pipeline.Internal.Mocks;
using Crucible.Mediator.Engine.Tests.Pipeline.Resolvers.Mocks;
using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Internal
{
    public class MockMiddlewareInvocationPipelineItemTest : MiddlewareInvocationPipelineItemTestBase<MockContract, MockContract, MockMiddlewareInvocationPipelineItem<MockContract, MockContract>>
    {
        public MockMiddlewareInvocationPipelineItemTest()
            : base(new()) { }

        protected override MockMiddlewareInvocationPipelineItem<MockContract, MockContract> CreateMiddlewareItem(int order) => new () { 
            Order = order,
            Resolver = InvocationComponentResolver,
        };

        protected override IMiddlewareInvocationPipelineItem CreateItemForMiddlewareSignature<TRequest, TResponse>()
        {
            var resolver = new MockInvocationComponentResolver<IInvocationMiddleware<TRequest, TResponse>>();
            return new DefaultMiddlewareInvocationPipelineItem<TRequest, TResponse>(order: 0, resolver);
        }
    }
}
