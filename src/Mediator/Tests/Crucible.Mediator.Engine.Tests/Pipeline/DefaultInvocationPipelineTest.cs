using Crucible.Mediator.Abstractions.Tests.Internal;
using Crucible.Mediator.Abstractions.Tests.Invocation.Mocks;
using Crucible.Mediator.Engine.Pipeline;
using Crucible.Mediator.Engine.Tests.Pipeline.Bases;
using Crucible.Mediator.Engine.Tests.Pipeline.Resolvers.Mocks;

namespace Crucible.Mediator.Engine.Tests.Pipeline
{
    [ImplementationTest]
    public class DefaultInvocationPipelineTest : InvocationPipelineConformanceTestBase<MockContract, MockContract, DefaultInvocationPipeline<MockContract, MockContract>>
    {
        protected MockInvocationComponentResolver<IPrePipelineMiddleware> PrePipelineMiddlewareResolver { get; }

        protected MockInvocationComponentResolver<IPreHandlerMiddleware> PreHandlerMiddlewareResolver { get; }

        public DefaultInvocationPipelineTest()
            : base(new())
        {
            PrePipelineMiddlewareResolver = new() { Component = PrePipelineMiddleware };
            PreHandlerMiddlewareResolver = new() { Component = PreHandlerMiddleware };
        }

        protected override DefaultInvocationPipeline<MockContract, MockContract> CreateInvocationPipeline() => new(
            ContextFactory,
            PrePipelineMiddlewareResolver,
            MiddlewareItems,
            PreHandlerMiddlewareResolver,
            HandlerStrategy);
    }
}
