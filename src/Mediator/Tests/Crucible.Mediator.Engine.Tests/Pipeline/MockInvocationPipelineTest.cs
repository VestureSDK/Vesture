using Crucible.Mediator.Abstractions.Tests.Invocation.Mocks;
using Crucible.Mediator.Engine.Tests.Pipeline.Bases;
using Crucible.Mediator.Engine.Tests.Pipeline.Mocks;

namespace Crucible.Mediator.Engine.Tests.Pipeline
{
    public class MockInvocationPipelineTest : InvocationPipelineTestBase<MockContract, MockContract, MockInvocationPipeline<MockContract, MockContract>>
    {
        public MockInvocationPipelineTest()
            : base(new()) { }

        protected override MockInvocationPipeline<MockContract, MockContract> CreatePipeline() => new(
            ContextFactory,
            PrePipelineMiddlewareResolver,
            MiddlewareInvocationPipelineItems,
            PreHandlerMiddlewareResolver,
            HandlerStrategy);
    }
}
