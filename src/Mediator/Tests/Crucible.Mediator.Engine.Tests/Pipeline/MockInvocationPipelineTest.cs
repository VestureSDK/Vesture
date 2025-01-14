using Crucible.Mediator.Abstractions.Tests.Data.Annotations.Commands;
using Crucible.Mediator.Engine.Mocks.Pipeline;
using Crucible.Testing.Annotations;

namespace Crucible.Mediator.Engine.Tests.Pipeline
{
    [MockTest]
    [TestFixtureSource_RequestResponse_All]
    public class MockInvocationPipelineTest<TRequest, TResponse> : InvocationPipelineConformanceTestBase<TRequest, TResponse, MockInvocationPipeline<TRequest, TResponse>>
    {
        public MockInvocationPipelineTest(TRequest request, TResponse response)
            : base(request) { }

        protected override MockInvocationPipeline<TRequest, TResponse> CreateInvocationPipeline() => new()
        {
            ContextFactory = ContextFactory,
            PrePipelineMiddleware = PrePipelineMiddleware,
            Middlewares = MiddlewareItems,
            PreHandlerMiddleware = PreHandlerMiddleware,
            HandlerStrategy = HandlerStrategy
        };
    }
}
