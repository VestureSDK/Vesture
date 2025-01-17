using Ingot.Mediator.Engine.Mocks.Pipeline.Resolvers;
using Ingot.Mediator.Engine.Pipeline.Resolvers;
using Ingot.Mediator.Engine.Pipeline.Strategies;
using Ingot.Mediator.Invocation;
using Moq;

namespace Ingot.Mediator.Engine.Tests.Pipeline.Strategies
{
    public abstract class EngineMultiInvocationHandlerStrategyTestBase<TRequest, TResponse, TStrategy> :
       MultiInvocationHandlerStrategyConformanceTestBase<TRequest, TResponse, TStrategy>
       where TStrategy : IInvocationHandlerStrategy<TRequest, TResponse>
    {
        protected MockInvocationComponentResolver<IInvocationHandler<TRequest, TResponse>> Resolver { get; }

        protected MockInvocationComponentResolver<IInvocationHandler<TRequest, TResponse>> OtherResolver { get; }

        protected ICollection<IInvocationComponentResolver<IInvocationHandler<TRequest, TResponse>>> Resolvers { get; } = [];

        protected EngineMultiInvocationHandlerStrategyTestBase(TRequest defaultRequest, TResponse defaultResponse)
            : base(defaultRequest, defaultResponse)
        {
            Resolver = new() { Component = Handler };
            OtherResolver = new() { Component = OtherHandler };

            Resolvers.Add(Resolver);
            Resolvers.Add(OtherResolver);
        }

        [Test]
        public void Ctor_HandlersAreNotResolved()
        {
            // Arrange
            // No arrange required

            // Act
            _ = Strategy;

            // Assert
            Resolver.Mock.Verify(m => m.ResolveComponent(), Times.Never);
            OtherResolver.Mock.Verify(m => m.ResolveComponent(), Times.Never);
        }

        [Test]
        [TestCase(1, Description = "Call HandleAsync once")]
        [TestCase(5, Description = "Call HandleAsync multiple times")]
        public async Task HandleAsync_ResolvesTheHandlersFromTheResolverEverytime(int iterationCount)
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
            OtherResolver.Mock.Verify(m => m.ResolveComponent(), Times.Exactly(iterationCount));
        }
    }
}
