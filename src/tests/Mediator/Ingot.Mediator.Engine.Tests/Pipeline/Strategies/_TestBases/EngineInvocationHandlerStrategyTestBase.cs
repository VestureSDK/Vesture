using Ingot.Mediator.Engine.Mocks.Pipeline.Resolvers;
using Ingot.Mediator.Engine.Pipeline.Strategies;
using Ingot.Mediator.Invocation;
using Moq;

namespace Ingot.Mediator.Engine.Tests.Pipeline.Strategies
{
    public abstract class EngineInvocationHandlerStrategyTestBase<TRequest, TResponse, TStrategy>
        : InvocationHandlerStrategyConformanceTestBase<TRequest, TResponse, TStrategy>
        where TStrategy : IInvocationHandlerStrategy<TRequest, TResponse>
    {
        protected MockInvocationComponentResolver<
            IInvocationHandler<TRequest, TResponse>
        > Resolver { get; }

        protected EngineInvocationHandlerStrategyTestBase(
            TRequest defaultRequest,
            TResponse defaultResponse
        )
            : base(defaultRequest, defaultResponse)
        {
            Resolver = new() { Component = Handler };
        }

        [Test]
        public void Ctor_HandlerIsNotResolved()
        {
            // Arrange
            // No arrange required

            // Act
            _ = Strategy;

            // Assert
            Resolver.Mock.Verify(m => m.ResolveComponent(), Times.Never);
        }

        [Test]
        [TestCase(1, Description = "Call HandleAsync once")]
        [TestCase(5, Description = "Call HandleAsync multiple times")]
        public async Task HandleAsync_ResolvesTheHandlerFromTheResolverEverytime(int iterationCount)
        {
            // Arrange
            // No arrange required

            // Act
            for (var i = 0; i < iterationCount; i++)
            {
                await Strategy.HandleAsync(Context, Next, CancellationToken);
            }

            // Assert
            Resolver.Mock.Verify(m => m.ResolveComponent(), Times.Exactly(iterationCount));
        }
    }
}
