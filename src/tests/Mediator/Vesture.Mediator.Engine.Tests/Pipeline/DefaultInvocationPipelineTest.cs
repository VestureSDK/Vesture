using Vesture.Mediator.Abstractions.Tests.Data.Annotations.Commands;
using Vesture.Mediator.Engine.Mocks.Pipeline.Resolvers;
using Vesture.Mediator.Engine.Pipeline;
using Vesture.Testing;
using Vesture.Testing.Annotations;

namespace Vesture.Mediator.Engine.Tests.Pipeline
{
    [ImplementationTest]
    [TestFixtureSource_RequestResponse_All]
    public class DefaultInvocationPipelineTest<TRequest, TResponse>
        : InvocationPipelineConformanceTestBase<
            TRequest,
            TResponse,
            DefaultInvocationPipeline<TRequest, TResponse>
        >
    {
        protected NUnitTestContextMsLogger<
            DefaultInvocationPipeline<TRequest, TResponse>
        > Logger { get; } = new();

        protected MockInvocationComponentResolver<IPrePipelineMiddleware> PrePipelineMiddlewareResolver { get; }

        protected MockInvocationComponentResolver<IPreHandlerMiddleware> PreHandlerMiddlewareResolver { get; }

        public DefaultInvocationPipelineTest(TRequest request, TResponse response)
            : base(request)
        {
            PrePipelineMiddlewareResolver = new() { Component = PrePipelineMiddleware };
            PreHandlerMiddlewareResolver = new() { Component = PreHandlerMiddleware };
        }

        protected override DefaultInvocationPipeline<
            TRequest,
            TResponse
        > CreateInvocationPipeline() =>
            new(
                Logger,
                ContextFactory,
                PrePipelineMiddlewareResolver,
                MiddlewareItems,
                PreHandlerMiddlewareResolver,
                HandlerStrategy
            );
    }
}
