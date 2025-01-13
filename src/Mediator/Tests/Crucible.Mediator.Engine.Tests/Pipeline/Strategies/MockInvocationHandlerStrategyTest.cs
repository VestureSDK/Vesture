using Crucible.Mediator.Abstractions.Tests.Data.Annotations.Commands;
using Crucible.Mediator.Abstractions.Tests.Internal;
using Crucible.Mediator.Engine.Tests.Pipeline.Strategies.Bases;
using Crucible.Mediator.Engine.Tests.Pipeline.Strategies.Mocks;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Strategies
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
