using Crucible.Mediator.Engine.Pipeline.Strategies;
using Crucible.Mediator.Invocation;
using Crucible.Mediator.Mocks.Invocation;
using Crucible.Testing.Annotations;
using Moq;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Strategies
{
    public abstract class MultiInvocationHandlerStrategyConformanceTestBase<TRequest, TResponse, TStrategy> :
       InvocationHandlerStrategyConformanceTestBase<TRequest, TResponse, TStrategy>
       where TStrategy : IInvocationHandlerStrategy<TRequest, TResponse>
    {
        protected MockInvocationHandler<TRequest, TResponse> OtherHandler { get; }

        protected ICollection<IInvocationHandler<TRequest, TResponse>> Handlers { get; } = [];

        protected MultiInvocationHandlerStrategyConformanceTestBase(TRequest defaultRequest, TResponse defaultResponse)
            : base(defaultRequest, defaultResponse)
        {
            OtherHandler = new()
            {
                Response = defaultResponse
            };

            Handlers.Add(Handler);
            Handlers.Add(OtherHandler);
        }

        [Test]
        [ConformanceTest]
        public async Task HandleAsync_InvokesHandlers()
        {
            // Arrange
            // No arrange required

            // Act
            await Strategy.HandleAsync(Context, Next!, CancellationToken);

            // Assert
            Handler.Mock.Verify(m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()), Times.Once);
            OtherHandler.Mock.Verify(m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }

}
