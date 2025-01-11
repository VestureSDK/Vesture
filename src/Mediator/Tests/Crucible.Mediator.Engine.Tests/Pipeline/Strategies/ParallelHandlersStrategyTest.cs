using Crucible.Mediator.Abstractions.Tests.Invocation.Mocks;
using Crucible.Mediator.Engine.Pipeline.Resolvers;
using Crucible.Mediator.Engine.Pipeline.Strategies;
using Crucible.Mediator.Engine.Tests.Pipeline.Resolvers.Mocks;
using Crucible.Mediator.Engine.Tests.Pipeline.Strategies.Bases;
using Crucible.Mediator.Invocation;
using Moq;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Strategies
{
    public class ParallelHandlersStrategyTest : InvocationHandlerStrategyTestBase<MockContract, MockContract, ParallelHandlersStrategy<MockContract, MockContract>>
    {
        protected MockInvocationHandler<MockContract, MockContract> HandlerA { get; }

        protected MockInvocationComponentResolver<IInvocationHandler<MockContract, MockContract>> ResolverA { get; }

        protected MockInvocationHandler<MockContract, MockContract> HandlerB { get; }

        protected MockInvocationComponentResolver<IInvocationHandler<MockContract, MockContract>> ResolverB { get; }

        protected ICollection<IInvocationComponentResolver<IInvocationHandler<MockContract, MockContract>>> Resolvers { get; }

        public ParallelHandlersStrategyTest()
            : base(new(), new())
        {
            HandlerA = new() { Response = Response };
            HandlerB = new() { Response = Response };

            ResolverA = new() { Component = HandlerA };
            ResolverB = new() { Component = HandlerB };
            Resolvers = [ResolverA, ResolverB];
        }

        protected override ParallelHandlersStrategy<MockContract, MockContract> CreateStrategy() => new(Resolvers);

        [Test]
        public void Ctor_HandlersAreNotResolved()
        {
            // Arrange
            // No arrange required

            // Act
            _ = Strategy;

            // Assert
            ResolverA.Mock.Verify(m => m.ResolveComponent(), Times.Never, failMessage: "ResolveComponent should not be called outside of HandleAsync");
            ResolverB.Mock.Verify(m => m.ResolveComponent(), Times.Never, failMessage: "ResolveComponent should not be called outside of HandleAsync");
        }

        [Theory]
        [TestCase(1, Description = "Call HandleAsync once")]
        [TestCase(5, Description = "Call HandleAsync multiple times")]
        public async Task HandleAsync_ResolvesTheHandlersFromTheResolversEverytime(int iterationCount)
        {
            // Arrange
            // No arrange required

            // Act
            for (var i = 0; i < iterationCount; i++)
            {
                await Strategy.HandleAsync(InvocationContext, Next, CancellationToken);
            }

            // Assert
            ResolverA.Mock.Verify(m => m.ResolveComponent(), Times.Exactly(iterationCount), failMessage: "ResolveComponent should be called everytime HandleAsync is invoked");
            ResolverB.Mock.Verify(m => m.ResolveComponent(), Times.Exactly(iterationCount), failMessage: "ResolveComponent should be called everytime HandleAsync is invoked");
        }

        [Test]
        public async Task HandleAsync_ResolvedHandlersAreInvoked()
        {
            // Arrange
            // No arrange required

            // Act
            await Strategy.HandleAsync(InvocationContext, Next, CancellationToken);

            // Assert
            HandlerA.Mock.Verify(m => m.HandleAsync(It.IsAny<MockContract>(), It.IsAny<CancellationToken>()), Times.Once, failMessage: "HandleAsync should called resolved handler");
            HandlerB.Mock.Verify(m => m.HandleAsync(It.IsAny<MockContract>(), It.IsAny<CancellationToken>()), Times.Once, failMessage: "HandleAsync should called resolved handler");
        }

        [Test]
        public async Task HandleAsync_BothHandlersAreInvoked_WhenEitherHandlerFails()
        {
            // Arrange
            HandlerA.Mock.Setup(m => m.HandleAsync(It.IsAny<MockContract>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("sample exception"));

            // Act
            try { await Strategy.HandleAsync(InvocationContext, Next, CancellationToken); } catch { }

            // Assert
            HandlerB.Mock.Verify(m => m.HandleAsync(It.IsAny<MockContract>(), It.IsAny<CancellationToken>()), Times.Once, failMessage: "HandleAsync should called resolved handler");
        }

        [Test]
        public async Task HandleAsync_BothHandlersAreResolved_WhenEitherHandlerFails()
        {
            // Arrange
            HandlerA.Mock.Setup(m => m.HandleAsync(It.IsAny<MockContract>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("sample exception"));

            // Act
            try { await Strategy.HandleAsync(InvocationContext, Next, CancellationToken); } catch { }

            // Assert
            ResolverB.Mock.Verify(m => m.ResolveComponent(), Times.Once, failMessage: "ResolveComponent should be called everytime HandleAsync is invoked");
        }

        [Test]
        public async Task HandleAsync_HandlersAreInvokedInParallel()
        {
            // Arrange
            var handlerAInvoked = false;
            var taskCompletionSourceA = new TaskCompletionSource<MockContract>();
            HandlerA.Mock.Setup(m => m.HandleAsync(It.IsAny<MockContract>(), It.IsAny<CancellationToken>()))
                .Returns(() =>
                {
                    handlerAInvoked = true;
                    return taskCompletionSourceA.Task;
                });

            var handlerBInvoked = false;
            var taskCompletionSourceB = new TaskCompletionSource<MockContract>();
            HandlerB.Mock.Setup(m => m.HandleAsync(It.IsAny<MockContract>(), It.IsAny<CancellationToken>()))
                .Returns(() =>
                {
                    handlerBInvoked = true;
                    return taskCompletionSourceB.Task;
                });

            // Act
            var task = Strategy.HandleAsync(InvocationContext, Next, CancellationToken);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(handlerAInvoked, Is.True);
                Assert.That(handlerBInvoked, Is.True);
            });

            // Cleanup
            taskCompletionSourceA.SetResult(new());
            taskCompletionSourceB.SetResult(new());
            await task;
        }
    }
}
