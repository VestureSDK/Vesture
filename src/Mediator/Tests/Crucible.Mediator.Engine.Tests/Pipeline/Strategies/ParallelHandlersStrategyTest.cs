using Crucible.Mediator.Abstractions.Tests.Internal;
using Crucible.Mediator.Abstractions.Tests.Invocation.Mocks;
using Crucible.Mediator.Engine.Pipeline.Strategies;
using Crucible.Mediator.Engine.Tests.Pipeline.Strategies.Bases;
using Moq;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Strategies
{
    [ImplementationTest]
    public class ParallelHandlersStrategyTest : EngineMultiInvocationHandlerStrategyTestBase<MockContract, MockContract, ParallelHandlersStrategy<MockContract, MockContract>>
    {
        public ParallelHandlersStrategyTest()
            : base(new(), new()) { }

        protected override ParallelHandlersStrategy<MockContract, MockContract> CreateStrategy() => new(Resolvers);

        [Test]
        public async Task HandleAsync_BothHandlersAreInvoked_WhenEitherHandlerFails()
        {
            // Arrange
            Handler.Mock.Setup(m => m.HandleAsync(It.IsAny<MockContract>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("sample exception"));

            // Act
            try { await Strategy.HandleAsync(Context, Next, CancellationToken); } catch { }

            // Assert
            OtherHandler.Mock.Verify(m => m.HandleAsync(It.IsAny<MockContract>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task HandleAsync_BothHandlersAreResolved_WhenEitherHandlerFails()
        {
            // Arrange
            Handler.Mock.Setup(m => m.HandleAsync(It.IsAny<MockContract>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("sample exception"));

            // Act
            try { await Strategy.HandleAsync(Context, Next, CancellationToken); } catch { }

            // Assert
            OtherResolver.Mock.Verify(m => m.ResolveComponent(), Times.Once);
        }

        [Test]
        public async Task HandleAsync_HandlersAreInvokedInParallel()
        {
            // Arrange
            var tcsHandler = new TaskCompletionSource<MockContract>();
            Handler.Mock.Setup(m => m.HandleAsync(It.IsAny<MockContract>(), It.IsAny<CancellationToken>()))
                .Returns(tcsHandler.Task);

            var tcsOtherHandler = new TaskCompletionSource<MockContract>();
            OtherHandler.Mock.Setup(m => m.HandleAsync(It.IsAny<MockContract>(), It.IsAny<CancellationToken>()))
                .Returns(tcsOtherHandler.Task);

            // Act
            var task = Strategy.HandleAsync(Context, Next, CancellationToken);

            // Assert
            Handler.Mock.Verify(m => m.HandleAsync(It.IsAny<MockContract>(), It.IsAny<CancellationToken>()), Times.Once);
            OtherHandler.Mock.Verify(m => m.HandleAsync(It.IsAny<MockContract>(), It.IsAny<CancellationToken>()), Times.Once);

            tcsHandler.SetResult(Response);
            tcsOtherHandler.SetResult(Response);
            
            // Cleanup
            await task;
        }
    }
}
