using Ingot.Mediator.Abstractions.Tests.Data.Annotations.Commands;
using Ingot.Mediator.Engine.Mocks.Pipeline.Resolvers;
using Ingot.Mediator.Engine.Pipeline;
using Ingot.Testing;
using Ingot.Testing.Annotations;

namespace Ingot.Mediator.Engine.Tests.Pipeline
{
    [ImplementationTest]
    [TestFixtureSource_RequestResponse_All]
    public class DefaultInvocationPipelineTest<TRequest, TResponse> : InvocationPipelineConformanceTestBase<TRequest, TResponse, DefaultInvocationPipeline<TRequest, TResponse>>
    {
        protected NUnitTestContextMsLogger<DefaultInvocationPipeline<TRequest, TResponse>> Logger { get; } = new();

        protected MockInvocationComponentResolver<IPrePipelineMiddleware> PrePipelineMiddlewareResolver { get; }

        protected MockInvocationComponentResolver<IPreHandlerMiddleware> PreHandlerMiddlewareResolver { get; }

        public DefaultInvocationPipelineTest(TRequest request, TResponse response)
            : base(request)
        {
            PrePipelineMiddlewareResolver = new() { Component = PrePipelineMiddleware };
            PreHandlerMiddlewareResolver = new() { Component = PreHandlerMiddleware };
        }

        protected override DefaultInvocationPipeline<TRequest, TResponse> CreateInvocationPipeline() => new(
            Logger,
            ContextFactory,
            PrePipelineMiddlewareResolver,
            MiddlewareItems,
            PreHandlerMiddlewareResolver,
            HandlerStrategy);
    }
}
