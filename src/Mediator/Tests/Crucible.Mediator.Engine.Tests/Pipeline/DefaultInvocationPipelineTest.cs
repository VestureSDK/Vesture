using Crucible.Mediator.Abstractions.Tests.Invocation.Mocks;
using Crucible.Mediator.Engine.Pipeline;
using Crucible.Mediator.Engine.Tests.Pipeline.Bases;

namespace Crucible.Mediator.Engine.Tests.Pipeline
{
    public class DefaultInvocationPipelineTest : InvocationPipelineTestBase<MockContract, MockContract, DefaultInvocationPipeline<MockContract, MockContract>>
    {
        public DefaultInvocationPipelineTest()
            : base(new()) { }

        protected override DefaultInvocationPipeline<MockContract, MockContract> CreatePipeline() => new(
            ContextFactory,
            PrePipelineMiddlewareResolver,
            MiddlewareInvocationPipelineItems,
            PreHandlerMiddlewareResolver,
            HandlerStrategy);
    }
}
