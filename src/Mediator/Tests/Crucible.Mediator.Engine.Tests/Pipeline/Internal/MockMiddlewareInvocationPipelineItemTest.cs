using Crucible.Mediator.Abstractions.Tests.Internal;
using Crucible.Mediator.Abstractions.Tests.Invocation.Mocks;
using Crucible.Mediator.Engine.Pipeline.Internal;
using Crucible.Mediator.Engine.Tests.Pipeline.Internal.Bases;
using Crucible.Mediator.Engine.Tests.Pipeline.Internal.Mocks;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Internal
{
    [MockTest]
    public class MockMiddlewareInvocationPipelineItemTest : MiddlewareInvocationPipelineItemConformanceTestBase<MockContract, MockContract, MockMiddlewareInvocationPipelineItem<MockContract, MockContract>>
    {
        public MockMiddlewareInvocationPipelineItemTest()
            : base(new()) { }

        protected override MockMiddlewareInvocationPipelineItem<MockContract, MockContract> CreateMiddlewareItem(int order) => 
            new () 
            { 
                Order = order,
                Middleware = Middleware
            };

        protected override IMiddlewareInvocationPipelineItem CreateItemForMiddlewareSignature<TRequest, TResponse>()
        {
            return new MockMiddlewareInvocationPipelineItem<TRequest, TResponse>();
        }
    }
}
