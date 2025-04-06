using Vesture.Mediator.Abstractions.Tests.Data.Annotations.Commands;
using Vesture.Mediator.Engine.Mocks.Pipeline.Strategies;
using Vesture.Testing.Annotations;

namespace Vesture.Mediator.Engine.Tests.Pipeline.Strategies
{
    [MockTest]
    [TestFixtureSource_RequestResponse_All]
    public class MockInvocationHandlerStrategyTest<TRequest, TResponse>
        : MultiInvocationHandlerStrategyConformanceTestBase<
            TRequest,
            TResponse,
            MockInvocationHandlerStrategy<TRequest, TResponse>
        >
    {
        public MockInvocationHandlerStrategyTest(TRequest request, TResponse response)
            : base(request, response) { }

        protected override MockInvocationHandlerStrategy<TRequest, TResponse> CreateStrategy() =>
            new() { Handlers = Handlers };
    }
}
