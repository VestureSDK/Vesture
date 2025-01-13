using Crucible.Mediator.Abstractions.Tests.Data;
using Crucible.Mediator.Abstractions.Tests.Invocation.Mocks;
using Crucible.Mediator.Engine.Pipeline;
using Crucible.Mediator.Engine.Pipeline.Internal;
using Crucible.Mediator.Engine.Tests.Pipeline.Context.Mocks;
using Crucible.Mediator.Engine.Tests.Pipeline.Internal.Mocks;
using Crucible.Mediator.Engine.Tests.Pipeline.Mocks;
using Crucible.Mediator.Engine.Tests.Pipeline.Strategies.Mocks;
using Crucible.Mediator.Invocation;
using Moq;
using Any = object;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Bases
{
    public abstract class InvocationPipelineConformanceTestBase<TRequest, TResponse, TPipeline>
        where TPipeline : IInvocationPipeline<TResponse>
    {
        protected Lazy<TPipeline> PipelineInitializer { get; }

        protected TPipeline Pipeline => PipelineInitializer.Value;

        protected TRequest Request { get; set; }

        protected CancellationToken CancellationToken { get; set; }

        protected MockInvocationContext<TRequest, TResponse> Context { get; }

        protected MockInvocationContextFactory<TRequest, TResponse> ContextFactory { get; }

        protected MockPrePipelineMiddleware PrePipelineMiddleware { get; } = new();

        protected ICollection<IMiddlewareInvocationPipelineItem> MiddlewareItems { get; } = [];

        protected MockPreHandlerMiddleware PreHandlerMiddleware { get; } = new();

        protected MockInvocationHandlerStrategy<TRequest, TResponse> HandlerStrategy { get; }

        protected InvocationPipelineConformanceTestBase(TRequest defaultRequest)
        {
            Request = defaultRequest;
            Context = new() { Request = Request! };
            ContextFactory = new() { Context = Context };

            HandlerStrategy = new();

#pragma warning disable IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
            PipelineInitializer = new Lazy<TPipeline>(() => CreateInvocationPipeline());
#pragma warning restore IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
        }

        protected abstract TPipeline CreateInvocationPipeline();

        [Test]
        public void Request_IsTRequest()
        {
            // Arrange
            // no arrange required

            // Act
            var request = Pipeline.RequestType;

            // Assert
            Assert.That(request, Is.EqualTo(typeof(TRequest)));
        }

        [Test]
        public void Response_IsTResponse()
        {
            // Arrange
            // no arrange required

            // Act
            var response = Pipeline.ResponseType;

            // Assert
            Assert.That(response, Is.EqualTo(typeof(TResponse)));
        }

        [Test]
        public async Task HandleAsync_HandlerStrategyIsInvoked()
        {
            // Arrange
            // no arrange required

            // Act
            await Pipeline.HandleAsync(Request!, CancellationToken);

            // Assert
            HandlerStrategy.Mock.Verify(m => m.HandleAsync(It.IsAny<IInvocationContext<TRequest, TResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task HandleAsync_MiddlewareIsInvoked()
        {
            // Arrange
            var middleware = new MockMiddlewareInvocationPipelineItem<TRequest, TResponse>();
            MiddlewareItems.Add(middleware);

            // Act
            await Pipeline.HandleAsync(Request!, CancellationToken);

            // Assert
            middleware.Mock.Verify(m => m.HandleAsync(It.IsAny<IInvocationContext<TRequest, TResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task HandleAsync_PrePipelineMiddlewareIsInvoked()
        {
            // Arrange
            // no arrange required

            // Act
            await Pipeline.HandleAsync(Request!, CancellationToken);

            // Assert
            PrePipelineMiddleware.Mock.Verify(m => m.HandleAsync(It.IsAny<IInvocationContext<Any, Any>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task HandleAsync_PreHandlerMiddlewareIsInvoked()
        {
            // Arrange
            // no arrange required

            // Act
            await Pipeline.HandleAsync(Request!, CancellationToken);

            // Assert
            PreHandlerMiddleware.Mock.Verify(m => m.HandleAsync(It.IsAny<IInvocationContext<Any, Any>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task HandleAsync_ComponentsAreInvokedInSequence()
        {
            // Arrange
            var prePipelineMiddlewareTaskCompletionSource = new TaskCompletionSource();
            SetupMiddlewareWithTaskCompletionSource(PrePipelineMiddleware, prePipelineMiddlewareTaskCompletionSource);

            var middlewareTaskCompletionSource = new TaskCompletionSource();
            var middleware = new MockMiddlewareInvocationPipelineItem<TRequest, TResponse>();
            SetupMiddlewareItemWithTaskCompletionSource(middleware, middlewareTaskCompletionSource);
            MiddlewareItems.Add(middleware);

            var preHandlerMiddlewareTaskCompletionSource = new TaskCompletionSource();
            SetupMiddlewareWithTaskCompletionSource(PreHandlerMiddleware, preHandlerMiddlewareTaskCompletionSource);

            var handlerStrategyTaskCompletionSource = new TaskCompletionSource();
            SetupHandlerStrategyWithTaskCompletionSource(HandlerStrategy, handlerStrategyTaskCompletionSource);

            // Act / Assert
            var task = Pipeline.HandleAsync(Request!, CancellationToken);

            PrePipelineMiddleware.Mock.Verify(m => m.HandleAsync(It.IsAny<IInvocationContext<Any, Any>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Once);
            middleware.Mock.Verify(m => m.HandleAsync(It.IsAny<IInvocationContext<TRequest, TResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Never);
            PreHandlerMiddleware.Mock.Verify(m => m.HandleAsync(It.IsAny<IInvocationContext<Any, Any>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Never);
            HandlerStrategy.Mock.Verify(m => m.HandleAsync(It.IsAny<IInvocationContext<TRequest, TResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Never);

            prePipelineMiddlewareTaskCompletionSource.SetResult();

            PrePipelineMiddleware.Mock.Verify(m => m.HandleAsync(It.IsAny<IInvocationContext<Any, Any>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Once);
            middleware.Mock.Verify(m => m.HandleAsync(It.IsAny<IInvocationContext<TRequest, TResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Once);
            PreHandlerMiddleware.Mock.Verify(m => m.HandleAsync(It.IsAny<IInvocationContext<Any, Any>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Never);
            HandlerStrategy.Mock.Verify(m => m.HandleAsync(It.IsAny<IInvocationContext<TRequest, TResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Never);

            middlewareTaskCompletionSource.SetResult();

            PrePipelineMiddleware.Mock.Verify(m => m.HandleAsync(It.IsAny<IInvocationContext<Any, Any>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Once);
            middleware.Mock.Verify(m => m.HandleAsync(It.IsAny<IInvocationContext<TRequest, TResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Once);
            PreHandlerMiddleware.Mock.Verify(m => m.HandleAsync(It.IsAny<IInvocationContext<Any, Any>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Once);
            HandlerStrategy.Mock.Verify(m => m.HandleAsync(It.IsAny<IInvocationContext<TRequest, TResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Never);

            preHandlerMiddlewareTaskCompletionSource.SetResult();

            PrePipelineMiddleware.Mock.Verify(m => m.HandleAsync(It.IsAny<IInvocationContext<Any, Any>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Once);
            middleware.Mock.Verify(m => m.HandleAsync(It.IsAny<IInvocationContext<TRequest, TResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Once);
            PreHandlerMiddleware.Mock.Verify(m => m.HandleAsync(It.IsAny<IInvocationContext<Any, Any>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Once);
            HandlerStrategy.Mock.Verify(m => m.HandleAsync(It.IsAny<IInvocationContext<TRequest, TResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Once);

            handlerStrategyTaskCompletionSource.SetResult();

            // Cleanup
            await task;

            void SetupHandlerStrategyWithTaskCompletionSource<TReq, TRes>(MockInvocationHandlerStrategy<TReq, TRes> middleware, TaskCompletionSource source)
            {
                middleware.Mock
                    .Setup(m => m.HandleAsync(It.IsAny<IInvocationContext<TReq, TRes>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
                    .Returns<IInvocationContext<TReq, TRes>, Func<CancellationToken, Task>, CancellationToken>(async (ctx, next, ct) =>
                    {
                        await source.Task;
                    });
            }

            void SetupMiddlewareItemWithTaskCompletionSource<TReq, TRes>(MockMiddlewareInvocationPipelineItem<TReq, TRes> middleware, TaskCompletionSource source)
            {
                middleware.Mock
                    .Setup(m => m.HandleAsync(It.IsAny<IInvocationContext<TReq, TRes>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
                    .Returns<IInvocationContext<TReq, TRes>, Func<CancellationToken, Task>, CancellationToken>(async (ctx, next, ct) =>
                    {
                        await source.Task;
                        await next(ct);
                    });
            }

            void SetupMiddlewareWithTaskCompletionSource<TReq, TRes>(MockInvocationMiddleware<TReq, TRes> middleware, TaskCompletionSource source)
            {
                middleware.Mock
                    .Setup(m => m.HandleAsync(It.IsAny<IInvocationContext<TReq, TRes>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
                    .Returns<IInvocationContext<TReq, TRes>, Func<CancellationToken, Task>, CancellationToken>(async (ctx, next, ct) =>
                    {
                        await source.Task;
                        await next(ct);
                    });
            }
        }

        [Test]
        [TestCase(1, Description = "One middleware")]
        [TestCase(5, Description = "Multiple middlewares")]
        public async Task HandleAsync_HandlerStrategyIsInvoked_WhenMiddlewaresAreRegistered(int middlewareCount)
        {
            // Arrange
            for (var i = 0; i < middlewareCount; i++)
            {
                var middleware = new MockMiddlewareInvocationPipelineItem<TRequest, TResponse>();
                MiddlewareItems.Add(middleware);
            }

            // Act
            await Pipeline.HandleAsync(Request!, CancellationToken);

            // Assert
            HandlerStrategy.Mock.Verify(m => m.HandleAsync(It.IsAny<IInvocationContext<TRequest, TResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task HandleAsync_MiddlewaresAreInvokedInOrder(
            [Values(Int32.MinValue, -123, 0, 123, Int32.MaxValue)] int orderA,
            [Values(Int32.MinValue, -123, 0, 123, Int32.MaxValue)] int orderB)
        {
            // Arrange
            var middlewaresExecuted = new List<MockMiddlewareInvocationPipelineItem<TRequest, TResponse>>();

            var middlewareA = new MockMiddlewareInvocationPipelineItem<TRequest, TResponse>() { Order = orderA, };
            AddMiddlewareToExecutionListOnHandleAsyncInvoked(middlewareA);
            MiddlewareItems.Add(middlewareA);

            var middlewareB = new MockMiddlewareInvocationPipelineItem<TRequest, TResponse>() { Order = orderB, };
            AddMiddlewareToExecutionListOnHandleAsyncInvoked(middlewareB);
            MiddlewareItems.Add(middlewareB);

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

            void AddMiddlewareToExecutionListOnHandleAsyncInvoked(MockMiddlewareInvocationPipelineItem<TRequest, TResponse> middleware)
            {
                middleware.Mock
                .Setup(m => m.HandleAsync(It.IsAny<IInvocationContext<TRequest, TResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
                .Returns<IInvocationContext<TRequest, TResponse>, Func<CancellationToken, Task>, CancellationToken>((context, next, cancellationtoken) =>
                {
                    middlewaresExecuted.Add(middleware);
                    return next(cancellationtoken);
                });
            }
        }

        [Test]
        public async Task HandleAsync_NotApplicableMiddlewareIsNotInvoked()
        {
            // Arrange
            var middleware = new MockMiddlewareInvocationPipelineItem<MediatorTestData.Unrelated, MediatorTestData.Unrelated>();
            MiddlewareItems.Add(middleware);

            // Act
            await Pipeline.HandleAsync(Request!, CancellationToken);

            // Assert
            middleware.Mock.Verify(m => m.HandleAsync(It.IsAny<IInvocationContext<MediatorTestData.Unrelated, MediatorTestData.Unrelated>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public void HandleAsync_DoesNotThrow_WhenHandlerStrategyThrows()
        {
            // Arrange
            HandlerStrategy.Mock.Setup(m => m.HandleAsync(It.IsAny<IInvocationContext<TRequest, TResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("sample exception"));

            // Act / Assert
            Assert.DoesNotThrowAsync(() => Pipeline.HandleAsync(Request!, CancellationToken));
        }

        [Test]
        public void HandleAsync_DoesNotThrow_WhenMiddlewareThrows()
        {
            // Arrange
            var middleware = new MockMiddlewareInvocationPipelineItem<TRequest, TResponse>();
            MiddlewareItems.Add(middleware);

            middleware.Mock.Setup(m => m.HandleAsync(It.IsAny<IInvocationContext<TRequest, TResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("sample exception"));

            // Act / Assert
            Assert.DoesNotThrowAsync(() => Pipeline.HandleAsync(Request!, CancellationToken));
        }
    }
}
