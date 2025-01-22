using Ingot.Mediator.Abstractions.Tests.Data.Annotations.Commands;
using Ingot.Mediator.Engine.Mocks.Pipeline.Strategies;
using Ingot.Testing.Annotations;

namespace Ingot.Mediator.Engine.Tests.Pipeline.Strategies
{
    [MockTest]
    [TestFixtureSource_RequestResponse_All]
    public class MockInvocationHandlerStrategyTest<TRequest, TResponse> : MultiInvocationHandlerStrategyConformanceTestBase<TRequest, TResponse, MockInvocationHandlerStrategy<TRequest, TResponse>>
    {
        public MockInvocationHandlerStrategyTest(TRequest request, TResponse response)
            : base(request, response) { }

        protected override MockInvocationHandlerStrategy<TRequest, TResponse> CreateStrategy() => new()
        {
            Handlers = Handlers
        };
    }
}
