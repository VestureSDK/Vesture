using Crucible.Mediator.Abstractions.Tests.Invocation.Mocks;
using Crucible.Mediator.Engine.Pipeline.Strategies;
using Crucible.Mediator.Engine.Tests.Pipeline.Resolvers.Mocks;
using Crucible.Mediator.Engine.Tests.Pipeline.Strategies.Bases;
using Crucible.Mediator.Invocation;
using Moq;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Strategies
{
    public class SingleHandlerStrategyTest : InvocationHandlerStrategyTestBase<MockContract, MockContract, SingleHandlerStrategy<MockContract, MockContract>>
    {
        protected MockInvocationHandler<MockContract, MockContract> Handler { get; }

        protected MockInvocationComponentResolver<IInvocationHandler<MockContract, MockContract>> Resolver { get; }

        public SingleHandlerStrategyTest()
            : base(new(), new())
        {
            Handler = new() { Response = Response };
            Resolver = new() { Component = Handler };
        }

        protected override SingleHandlerStrategy<MockContract, MockContract> CreateStrategy() => new(Resolver);

        [Test]
        public void Ctor_HandlerIsNotResolved()
        {
            // Arrange
            // No arrange required

            // Act
            _ = Strategy;

            // Assert
            Resolver.Mock.Verify(m => m.ResolveComponent(), Times.Never, failMessage: "ResolveComponent should not be called outside of HandleAsync");
        }

        [Theory]
        [TestCase(1, Description = "Call HandleAsync once")]
        [TestCase(5, Description = "Call HandleAsync multiple times")]
        public async Task HandleAsync_ResolvesTheHandlerFromTheResolverEverytime(int iterationCount)
        {
            // Arrange
            // No arrange required

            // Act
            for (var i = 0; i < iterationCount; i++)
            {
                await Strategy.HandleAsync(InvocationContext, Next, CancellationToken);
            }

            // Assert
            Resolver.Mock.Verify(m => m.ResolveComponent(), Times.Exactly(iterationCount), failMessage: "ResolveComponent should be called everytime HandleAsync is invoked");
        }

        [Test]
        public async Task HandleAsync_ResolvedHandlerIsInvoked()
        {
            // Arrange
            // No arrange required

            // Act
            await Strategy.HandleAsync(InvocationContext, Next, CancellationToken);

            // Assert
            Handler.Mock.Verify(m => m.HandleAsync(It.IsAny<MockContract>(), It.IsAny<CancellationToken>()), Times.Once, failMessage: "HandleAsync should called resolved handler");
        }
    }
}
