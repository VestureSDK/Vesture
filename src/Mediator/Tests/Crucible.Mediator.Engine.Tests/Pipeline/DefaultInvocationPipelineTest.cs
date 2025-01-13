using Crucible.Mediator.Abstractions.Tests.Data.Annotations.Commands;
using Crucible.Mediator.Abstractions.Tests.Internal;
using Crucible.Mediator.Engine.Pipeline;
using Crucible.Mediator.Engine.Tests.Pipeline.Bases;
using Crucible.Mediator.Engine.Tests.Pipeline.Resolvers.Mocks;

namespace Crucible.Mediator.Engine.Tests.Pipeline
{
    [ImplementationTest]
    [TestFixtureSource_RequestResponse_All]
    public class DefaultInvocationPipelineTest<TRequest, TResponse> : InvocationPipelineConformanceTestBase<TRequest, TResponse, DefaultInvocationPipeline<TRequest, TResponse>>
    {
        protected MockInvocationComponentResolver<IPrePipelineMiddleware> PrePipelineMiddlewareResolver { get; }

        protected MockInvocationComponentResolver<IPreHandlerMiddleware> PreHandlerMiddlewareResolver { get; }

        public DefaultInvocationPipelineTest(TRequest request, TResponse response)
            : base(request)
        {
            PrePipelineMiddlewareResolver = new() { Component = PrePipelineMiddleware };
            PreHandlerMiddlewareResolver = new() { Component = PreHandlerMiddleware };
        }

        protected override DefaultInvocationPipeline<TRequest, TResponse> CreateInvocationPipeline() => new(
            ContextFactory,
            PrePipelineMiddlewareResolver,
            MiddlewareItems,
            PreHandlerMiddlewareResolver,
            HandlerStrategy);
    }
}
