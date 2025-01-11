using Crucible.Mediator.Abstractions.Tests.Internal;
using Crucible.Mediator.Abstractions.Tests.Invocation.Mocks;
using Crucible.Mediator.Engine.Tests.Pipeline.Strategies.Bases;
using Crucible.Mediator.Engine.Tests.Pipeline.Strategies.Mocks;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Strategies
{
    [MockTest]
    public class MockInvocationHandlerStrategyTest : MultiInvocationHandlerStrategyConformanceTestBase<MockContract, MockContract, MockInvocationHandlerStrategy<MockContract, MockContract>>
    {
        public MockInvocationHandlerStrategyTest()
            : base(new(), new()) { }

        protected override MockInvocationHandlerStrategy<MockContract, MockContract> CreateStrategy() => new()
        {
            Handlers = Handlers
        };
    }
}
