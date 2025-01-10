using Crucible.Mediator.Abstractions.Tests.Invocation.Mocks;
using Crucible.Mediator.Engine.Pipeline;
using Crucible.Mediator.Engine.Pipeline.Internal;
using Crucible.Mediator.Engine.Tests.Pipeline.Context.Mocks;
using Crucible.Mediator.Engine.Tests.Pipeline.Internal.Mocks;
using Crucible.Mediator.Engine.Tests.Pipeline.Mocks;
using Crucible.Mediator.Engine.Tests.Pipeline.Resolvers.Mocks;
using Crucible.Mediator.Engine.Tests.Pipeline.Strategies.Mocks;
using Crucible.Mediator.Invocation;
using Moq;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Bases
{
    public abstract class InvocationPipelineTestBase<TRequest, TResponse, TPipeline>
        where TPipeline : IInvocationPipeline<TResponse>
    {
        protected Lazy<TPipeline> PipelineInitializer { get; }

        protected TPipeline Pipeline => PipelineInitializer.Value;

        protected TRequest Request { get; set; }

        protected CancellationToken CancellationToken { get; set; }

        protected MockInvocationContext<TRequest, TResponse> Context { get; }

        protected MockInvocationContextFactory<TRequest, TResponse> ContextFactory { get; }

        protected MockPrePipelineMiddleware PrePipelineMiddleware { get; } = new();

        protected MockInvocationComponentResolver<IPrePipelineMiddleware> PrePipelineMiddlewareResolver { get; }

        protected ICollection<IMiddlewareInvocationPipelineItem> MiddlewareInvocationPipelineItems { get; } = [];

        protected MockPreHandlerMiddleware PreHandlerMiddleware { get; } = new();

        protected MockInvocationComponentResolver<IPreHandlerMiddleware> PreHandlerMiddlewareResolver { get; }

        protected MockInvocationHandlerStrategy<TRequest, TResponse> HandlerStrategy { get; }

        protected MockInvocationHandler<TRequest, TResponse> Handler { get; } = new();

        protected InvocationPipelineTestBase(TRequest defaultRequest)
        {
            Request = defaultRequest;
            Context = new(Request);
            ContextFactory = new(Context);
            HandlerStrategy = new(Handler);

            PrePipelineMiddlewareResolver = new(PrePipelineMiddleware);
            PreHandlerMiddlewareResolver = new(PreHandlerMiddleware);

#pragma warning disable IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
            PipelineInitializer = new Lazy<TPipeline>(() => CreatePipeline());
#pragma warning restore IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
        }

        protected abstract TPipeline CreatePipeline();

        [Test]
        public void Request_IsTRequest()
        {
            // Arrange
            // no arrange required

            // Act
            var request = Pipeline.Request;

            // Assert
            Assert.That(request, Is.EqualTo(typeof(TRequest)));
        }

        [Test]
        public void Response_IsTResponse()
        {
            // Arrange
            // no arrange required

            // Act
            var response = Pipeline.Response;

            // Assert
            Assert.That(response, Is.EqualTo(typeof(TResponse)));
        }

        [Test]
        public async Task HandleAsync_HandlerIsInvoked()
        {
            // Arrange
            // no arrange required

            // Act
            await Pipeline.HandleAsync(Request!, CancellationToken);

            // Assert
            Handler.Mock.Verify(m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [TestCase(1, Description = "One middleware")]
        [TestCase(5, Description = "Multiple middlewares")]
        public async Task HandleAsync_HandlerIsInvoked_WhenMiddlewaresAreRegistered(int middlewareCount)
        {
            // Arrange
            for (var i = 0; i < middlewareCount; i++)
            {
                var middleware = new MockInvocationMiddleware<TRequest, TResponse>();
                var item = new MockMiddlewareInvocationPipelineItem<TRequest, TResponse>(middleware);
                MiddlewareInvocationPipelineItems.Add(item);
            }

            // Act
            await Pipeline.HandleAsync(Request!, CancellationToken);

            // Assert
            Handler.Mock.Verify(m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        public async Task HandleAsync_MiddlewaresAreInvokedInOrder(
            [Values(Int32.MinValue, -123, 0, 123, Int32.MaxValue)] int orderA,
            [Values(Int32.MinValue, -123, 0, 123, Int32.MaxValue)] int orderB)
        {
            // Arrange
            var middlewaresExecuted = new List<MockInvocationMiddleware<TRequest, TResponse>>();

            var middlewareA = new MockInvocationMiddleware<TRequest, TResponse>();
            middlewareA.Mock
                .Setup(m => m.HandleAsync(It.IsAny<IInvocationContext<TRequest, TResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
                .Returns<IInvocationContext<TRequest, TResponse>, Func<CancellationToken, Task>, CancellationToken>((context, next, cancellationtoken) =>
                {
                    middlewaresExecuted.Add(middlewareA);
                    return next(cancellationtoken);
                });
            var itemA = new MockMiddlewareInvocationPipelineItem<TRequest, TResponse>(middlewareA)
            {
                Order = orderA,
            };
            MiddlewareInvocationPipelineItems.Add(itemA);

            var middlewareB = new MockInvocationMiddleware<TRequest, TResponse>();
            middlewareB.Mock
                .Setup(m => m.HandleAsync(It.IsAny<IInvocationContext<TRequest, TResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
                .Returns<IInvocationContext<TRequest, TResponse>, Func<CancellationToken, Task>, CancellationToken>((context, next, cancellationtoken) =>
                {
                    middlewaresExecuted.Add(middlewareB);
                    return next(cancellationtoken);
                });
            var itemB = new MockMiddlewareInvocationPipelineItem<TRequest, TResponse>(middlewareB)
            {
                Order = orderB,
            };
            MiddlewareInvocationPipelineItems.Add(itemB);

            var middlewareAExpectedIndex = orderA <= orderB ? 0 : 1;
            var middlewareBExpectedIndex = middlewareAExpectedIndex == 1 ? 0 : 1;

            // Act
            await Pipeline.HandleAsync(Request!, CancellationToken);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(middlewaresExecuted[middlewareAExpectedIndex], Is.EqualTo(middlewareA));
                Assert.That(middlewaresExecuted[middlewareBExpectedIndex], Is.EqualTo(middlewareB));
            });
        }

        [Test]
        public async Task HandleAsync_MiddlewareIsInvoked()
        {
            // Arrange
            var middleware = new MockInvocationMiddleware<TRequest, TResponse>();
            var item = new MockMiddlewareInvocationPipelineItem<TRequest, TResponse>(middleware);
            MiddlewareInvocationPipelineItems.Add(item);

            // Act
            await Pipeline.HandleAsync(Request!, CancellationToken);

            // Assert
            middleware.Mock.Verify(m => m.HandleAsync(It.IsAny<IInvocationContext<TRequest, TResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task HandleAsync_NotApplicableMiddlewareIsNotInvoked()
        {
            // Arrange
            var middleware = new MockInvocationMiddleware<Unrelated, Unrelated>();
            var item = new MockMiddlewareInvocationPipelineItem<Unrelated, Unrelated>(middleware);
            MiddlewareInvocationPipelineItems.Add(item);

            // Act
            await Pipeline.HandleAsync(Request!, CancellationToken);

            // Assert
            middleware.Mock.Verify(m => m.HandleAsync(It.IsAny<IInvocationContext<Unrelated, Unrelated>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public void HandleAsync_DoesNotThrow_WhenHandlerThrows()
        {
            // Arrange
            Handler.Mock.Setup(m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("sample exception"));

            // Act / Assert
            Assert.DoesNotThrowAsync(() => Pipeline.HandleAsync(Request!, CancellationToken));
        }

        [Test]
        public void HandleAsync_DoesNotThrow_WhenMiddlewareThrows()
        {
            // Arrange
            var middleware = new MockInvocationMiddleware<TRequest, TResponse>();
            var item = new MockMiddlewareInvocationPipelineItem<TRequest, TResponse>(middleware);
            MiddlewareInvocationPipelineItems.Add(item);

            middleware.Mock.Setup(m => m.HandleAsync(It.IsAny<IInvocationContext<TRequest, TResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("sample exception"));

            // Act / Assert
            Assert.DoesNotThrowAsync(() => Pipeline.HandleAsync(Request!, CancellationToken));
        }

        public class Unrelated
        {

        }
    }
}
