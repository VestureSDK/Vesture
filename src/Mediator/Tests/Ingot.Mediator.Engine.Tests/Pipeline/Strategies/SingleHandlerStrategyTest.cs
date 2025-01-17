using Ingot.Mediator.Abstractions.Tests.Data.Annotations.Commands;
using Ingot.Mediator.Engine.Pipeline.Strategies;
using Ingot.Testing.Annotations;

namespace Ingot.Mediator.Engine.Tests.Pipeline.Strategies
{
    [ImplementationTest]
    [TestFixtureSource_RequestResponse_All]
    public class SingleHandlerStrategyTest<TRequest, TResponse> : EngineInvocationHandlerStrategyTestBase<TRequest, TResponse, SingleHandlerStrategy<TRequest, TResponse>>
    {
        public SingleHandlerStrategyTest(TRequest request, TResponse response)
            : base(request, response) { }

        protected override SingleHandlerStrategy<TRequest, TResponse> CreateStrategy() => new(Resolver);
    }
}
