using Ingot.Mediator.Abstractions.Tests.Data.Annotations.Commands;
using Ingot.Mediator.Engine.Pipeline.Strategies;
using Ingot.Testing;
using Ingot.Testing.Annotations;
using Moq;

namespace Ingot.Mediator.Engine.Tests.Pipeline.Strategies
{
    [ImplementationTest]
    [TestFixtureSource_RequestResponse_All]
    public class ParallelHandlersStrategyTest<TRequest, TResponse>
        : EngineMultiInvocationHandlerStrategyTestBase<
            TRequest,
            TResponse,
            ParallelHandlersStrategy<TRequest, TResponse>
        >
    {
        protected NUnitTestContextMsLogger<
            ParallelHandlersStrategy<TRequest, TResponse>
        > Logger { get; } = new();

        public ParallelHandlersStrategyTest(TRequest request, TResponse response)
            : base(request, response) { }

        protected override ParallelHandlersStrategy<TRequest, TResponse> CreateStrategy() =>
            new(Logger, Resolvers);

        [Test]
        public void Ctor_ArgumentNullException_IfLoggerIsNull()
        {
            // Arrange
            // No arrange required

            // Act / Assert
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>(
                () => new ParallelHandlersStrategy<TRequest, TResponse>(null, Resolvers)
            );
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Test]
        public void Ctor_ArgumentNullException_IfResolversIsNull()
        {
            // Arrange
            // No arrange required

            // Act / Assert
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>(
                () => new ParallelHandlersStrategy<TRequest, TResponse>(Logger, null)
            );
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Test]
        public void Ctor_ArgumentException_IfResolversIsEmpty()
        {
            // Arrange
            // No arrange required

            // Act / Assert
            Assert.Throws<ArgumentException>(
                () => new ParallelHandlersStrategy<TRequest, TResponse>(Logger, [])
            );
        }

        [Test]
        public async Task HandleAsync_BothHandlersAreInvoked_WhenEitherHandlerFails()
        {
            // Arrange
            Handler
                .Mock.Setup(m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("sample exception"));

            // Act
            try
            {
                await Strategy.HandleAsync(Context, Next, CancellationToken);
            }
            catch { }

            // Assert
            OtherHandler.Mock.Verify(
                m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Test]
        public async Task HandleAsync_BothHandlersAreResolved_WhenEitherHandlerFails()
        {
            // Arrange
            Handler
                .Mock.Setup(m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("sample exception"));

            // Act
            try
            {
                await Strategy.HandleAsync(Context, Next, CancellationToken);
            }
            catch { }

            // Assert
            OtherResolver.Mock.Verify(m => m.ResolveComponent(), Times.Once);
        }

        [Test]
        public async Task HandleAsync_HandlersAreInvokedInParallel()
        {
            // Arrange
            var tcsHandler = new TaskCompletionSource<TResponse>();
            Handler
                .Mock.Setup(m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()))
                .Returns(tcsHandler.Task);

            var tcsOtherHandler = new TaskCompletionSource<TResponse>();
            OtherHandler
                .Mock.Setup(m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()))
                .Returns(tcsOtherHandler.Task);

            // Act
            var task = Strategy.HandleAsync(Context, Next, CancellationToken);

            // Assert
            Handler.Mock.Verify(
                m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()),
                Times.Once
            );
            OtherHandler.Mock.Verify(
                m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()),
                Times.Once
            );

            tcsHandler.SetResult(Response);
            tcsOtherHandler.SetResult(Response);

            // Cleanup
            await task;
        }
    }
}
