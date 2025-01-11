using Crucible.Mediator.Abstractions.Tests.Internal;
using Crucible.Mediator.Abstractions.Tests.Invocation.Mocks;
using Crucible.Mediator.Engine.Pipeline.Internal;
using Crucible.Mediator.Engine.Tests.Pipeline.Internal.Bases;
using Crucible.Mediator.Engine.Tests.Pipeline.Resolvers.Mocks;
using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Internal
{
    [ImplementationTest]
    public class DefaultMiddlewareInvocationPipelineItemTest : EngineMiddlewareInvocationPipelineItemTestBase<MockContract, MockContract, DefaultMiddlewareInvocationPipelineItem<MockContract, MockContract>>
    {
        public DefaultMiddlewareInvocationPipelineItemTest()
            : base(new()) { }

        protected override DefaultMiddlewareInvocationPipelineItem<MockContract, MockContract> CreateMiddlewareItem(int order) => new DefaultMiddlewareInvocationPipelineItem<MockContract, MockContract>(order, Resolver);

        protected override IMiddlewareInvocationPipelineItem CreateItemForMiddlewareSignature<TRequest, TResponse>()
        {
            var resolver = new MockInvocationComponentResolver<IInvocationMiddleware<TRequest, TResponse>>();
            return new DefaultMiddlewareInvocationPipelineItem<TRequest, TResponse>(order: 0, resolver);
        }
    }
}
