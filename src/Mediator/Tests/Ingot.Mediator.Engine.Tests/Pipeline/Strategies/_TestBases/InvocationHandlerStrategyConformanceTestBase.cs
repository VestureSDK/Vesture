using Ingot.Mediator.Engine.Pipeline.Strategies;
using Ingot.Mediator.Mocks.Invocation;
using Ingot.Testing.Annotations;
using Moq;

namespace Ingot.Mediator.Engine.Tests.Pipeline.Strategies
{
    public abstract class InvocationHandlerStrategyConformanceTestBase<TRequest, TResponse, TStrategy>
       where TStrategy : IInvocationHandlerStrategy<TRequest, TResponse>
    {
        protected Lazy<TStrategy> StrategyInitializer { get; }

        protected TStrategy Strategy => StrategyInitializer.Value;

        protected MockInvocationContext<TRequest, TResponse> Context { get; }

        protected MockInvocationHandler<TRequest, TResponse> Handler { get; }

        protected CancellationToken CancellationToken { get; set; } = CancellationToken.None;

        protected MockNext Next { get; } = new();

        protected TResponse Response { get; set; }

        protected InvocationHandlerStrategyConformanceTestBase(TRequest defaultRequest, TResponse defaultResponse)
        {
            Response = defaultResponse;
            Context = new() { Request = defaultRequest! };
            Handler = new()
            {
                Response = defaultResponse
            };

#pragma warning disable IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
            StrategyInitializer = new Lazy<TStrategy>(() => CreateStrategy());
#pragma warning restore IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
        }

        protected abstract TStrategy CreateStrategy();

        [Test]
        [ConformanceTest]
        public async Task HandleAsync_DoesNotInvokeNextItemInTheChain()
        {
            // Arrange
            // No arrange required

            // Act
            await Strategy.HandleAsync(Context, Next, CancellationToken);

            // Assert
            Next.Mock.Verify(m => m(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        [ConformanceTest]
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        public async Task HandleAsync_DoesNotThrowWhenNextIsNull()
        {
            // Arrange
            // No arrange required

            // Act
            await Strategy.HandleAsync(Context, null, CancellationToken);

            // Assert
            Assert.Pass();
        }
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        [Test]
        [ConformanceTest]
        public async Task HandleAsync_ContextHasResponse()
        {
            // Arrange
            // No arrange required

            // Act
            await Strategy.HandleAsync(Context, Next!, CancellationToken);

            // Assert
            Assert.That(Context.Response, Is.EqualTo(Response));
        }

        [Test]
        [ConformanceTest]
        public async Task HandleAsync_InvokesHandler()
        {
            // Arrange
            // No arrange required

            // Act
            await Strategy.HandleAsync(Context, Next!, CancellationToken);

            // Assert
            Handler.Mock.Verify(m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
