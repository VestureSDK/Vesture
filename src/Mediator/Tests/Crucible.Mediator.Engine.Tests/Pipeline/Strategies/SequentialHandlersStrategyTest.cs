using Crucible.Mediator.Abstractions.Tests.Internal;
using Crucible.Mediator.Abstractions.Tests.Invocation.Mocks;
using Crucible.Mediator.Engine.Pipeline.Strategies;
using Crucible.Mediator.Engine.Tests.Pipeline.Strategies.Bases;
using Moq;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Strategies
{
    [ImplementationTest]
    public class SequentialHandlersStrategyTest : EngineMultiInvocationHandlerStrategyTestBase<MockContract, MockContract, SequentialHandlersStrategy<MockContract, MockContract>>
    {
        public SequentialHandlersStrategyTest()
            : base(new(), new()) { }

        protected override SequentialHandlersStrategy<MockContract, MockContract> CreateStrategy() => new(Resolvers);

        [Test]
        public async Task HandleAsync_OnlyFirstHandlerIsInvoked_WhenTheFirstHandlerFails()
        {
            // Arrange
            Handler.Mock.Setup(m => m.HandleAsync(It.IsAny<MockContract>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("sample exception"));

            // Act
            try { await Strategy.HandleAsync(Context, Next, CancellationToken); } catch { }

            // Assert
            OtherHandler.Mock.Verify(m => m.HandleAsync(It.IsAny<MockContract>(), It.IsAny<CancellationToken>()), Times.Never, failMessage: "HandleAsync should called resolved handler");
        }

        [Test]
        public async Task HandleAsync_OnlyFirstHandlerIsResolved_WhenTheFirstHandlerFails()
        {
            // Arrange
            Handler.Mock.Setup(m => m.HandleAsync(It.IsAny<MockContract>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("sample exception"));

            // Act
            try { await Strategy.HandleAsync(Context, Next, CancellationToken); } catch { }

            // Assert
            OtherResolver.Mock.Verify(m => m.ResolveComponent(), Times.Never, failMessage: "ResolveComponent should be called everytime HandleAsync is invoked");
        }

        [Test]
        public async Task HandleAsync_HandlersAreInvokedInSequence()
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
            OtherHandler.Mock.Verify(m => m.HandleAsync(It.IsAny<MockContract>(), It.IsAny<CancellationToken>()), Times.Never);

            tcsHandler.SetResult(Response);

            Handler.Mock.Verify(m => m.HandleAsync(It.IsAny<MockContract>(), It.IsAny<CancellationToken>()), Times.Once);
            OtherHandler.Mock.Verify(m => m.HandleAsync(It.IsAny<MockContract>(), It.IsAny<CancellationToken>()), Times.Once);

            // Cleanup
            tcsOtherHandler.SetResult(Response);
            await task;
        }
    }
}
