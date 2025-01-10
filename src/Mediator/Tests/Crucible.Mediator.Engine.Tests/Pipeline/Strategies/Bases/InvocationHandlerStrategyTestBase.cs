using Crucible.Mediator.Abstractions.Tests.Invocation.Mocks;
using Crucible.Mediator.Engine.Pipeline.Strategies;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Strategies.Bases
{
    public abstract class InvocationHandlerStrategyTestBase<TRequest, TResponse, TStrategy>
       where TStrategy : IInvocationHandlerStrategy<TRequest, TResponse>
    {
        protected Lazy<TStrategy> StrategyInitializer { get; }

        protected TStrategy Strategy => StrategyInitializer.Value;

        protected MockInvocationContext<TRequest, TResponse> InvocationContext { get; }

        protected CancellationToken CancellationToken { get; set; } = CancellationToken.None;

        protected Func<CancellationToken, Task> Next { get; set; } = (ct) => Task.CompletedTask;

        protected TResponse Response { get; set; }

        protected InvocationHandlerStrategyTestBase(TRequest defaultRequest, TResponse defaultResponse)
        {
            Response = defaultResponse;
            InvocationContext = new(defaultRequest);

#pragma warning disable IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
            StrategyInitializer = new Lazy<TStrategy>(() => CreateStrategy());
#pragma warning restore IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
        }

        protected abstract TStrategy CreateStrategy();

        [Test]
        public async Task HandleAsync_DoesNotCallNextItemInTheChain()
        {
            // Arrange
            var nextExecuted = false;
            Next = (ct) =>
            {
                nextExecuted = true;
                return Task.CompletedTask;
            };

            // Act
            await Strategy.HandleAsync(InvocationContext, Next, CancellationToken);

            // Assert
            Assert.That(nextExecuted, Is.False, message: "Next item in the chain should not be called");
        }

        [Test]
        public async Task HandleAsync_DoesNotThrowWhenNextIsNull()
        {
            // Arrange
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Next = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            // Act
            await Strategy.HandleAsync(InvocationContext, Next!, CancellationToken);

            // Assert
            Assert.Pass();
        }

        [Test]
        public async Task HandleAsync_ContextHasResponse()
        {
            // Arrange
            // No arrange required

            // Act
            await Strategy.HandleAsync(InvocationContext, Next!, CancellationToken);

            // Assert
            Assert.That(InvocationContext.Response, Is.EqualTo(Response));
        }
    }
}
