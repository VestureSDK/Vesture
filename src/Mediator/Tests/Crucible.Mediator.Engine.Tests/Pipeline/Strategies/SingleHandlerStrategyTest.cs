using Crucible.Mediator.Abstractions.Tests.Internal;
using Crucible.Mediator.Abstractions.Tests.Invocation.Mocks;
using Crucible.Mediator.Engine.Pipeline.Strategies;
using Crucible.Mediator.Engine.Tests.Pipeline.Strategies.Bases;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Strategies
{
    [ImplementationTest]
    public class SingleHandlerStrategyTest : EngineInvocationHandlerStrategyTestBase<MockContract, MockContract, SingleHandlerStrategy<MockContract, MockContract>>
    {
        public SingleHandlerStrategyTest()
            : base(new(), new()) { }

        protected override SingleHandlerStrategy<MockContract, MockContract> CreateStrategy() => new(Resolver);
    }
}
