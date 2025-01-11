using Crucible.Mediator.Abstractions.Tests.Internal;
using Crucible.Mediator.Abstractions.Tests.Invocation.Mocks;
using Crucible.Mediator.Engine.Tests.Pipeline.Bases;
using Crucible.Mediator.Engine.Tests.Pipeline.Mocks;

namespace Crucible.Mediator.Engine.Tests.Pipeline
{
    [MockTest]
    public class MockInvocationPipelineTest : InvocationPipelineConformanceTestBase<MockContract, MockContract, MockInvocationPipeline<MockContract, MockContract>>
    {
        public MockInvocationPipelineTest()
            : base(new()) { }

        protected override MockInvocationPipeline<MockContract, MockContract> CreateInvocationPipeline() => new ()
        {
            ContextFactory = ContextFactory,
            PrePipelineMiddleware = PrePipelineMiddleware,
            Middlewares = MiddlewareItems,
            PreHandlerMiddleware = PreHandlerMiddleware,
            HandlerStrategy = HandlerStrategy
        };
    }
}
