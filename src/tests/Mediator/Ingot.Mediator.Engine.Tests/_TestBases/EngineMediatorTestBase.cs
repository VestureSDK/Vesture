using Ingot.Mediator.Abstractions.Tests;
using Ingot.Mediator.Abstractions.Tests.Data.Annotations.Commands;
using Ingot.Mediator.Abstractions.Tests.Data.Annotations.Events;
using Ingot.Mediator.Abstractions.Tests.Data.Annotations.Requests;
using Ingot.Mediator.Commands;
using Ingot.Mediator.Engine.Mocks.Pipeline.Internal;
using Ingot.Mediator.Events;
using Ingot.Mediator.Invocation;
using Ingot.Mediator.Mocks.Invocation;
using Ingot.Mediator.Requests;
using Ingot.Testing.Annotations;
using Moq;

namespace Ingot.Mediator.Engine.Tests
{
    public abstract class EngineMediatorTestBase<TMediator> : MediatorConformanceTestBase<TMediator>
        where TMediator : IMediator
    {
        protected void AddMiddleware<TRequest, TResponse>(int order, IInvocationMiddleware<TRequest, TResponse> middleware)
        {
            Middlewares.Add(middleware);
            RegisterMiddleware(order, middleware);
        }

        protected abstract void RegisterMiddleware<TRequest, TResponse>(int order, IInvocationMiddleware<TRequest, TResponse> middleware);

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_All(int.MinValue, int.MinValue)]
        [TestCaseSource_RequestResponse_All(int.MinValue, -123)]
        [TestCaseSource_RequestResponse_All(int.MinValue, 0)]
        [TestCaseSource_RequestResponse_All(int.MinValue, 123)]
        [TestCaseSource_RequestResponse_All(int.MinValue, int.MaxValue)]
        [TestCaseSource_RequestResponse_All(-123, int.MinValue)]
        [TestCaseSource_RequestResponse_All(-123, -123)]
        [TestCaseSource_RequestResponse_All(-123, 0)]
        [TestCaseSource_RequestResponse_All(-123, 123)]
        [TestCaseSource_RequestResponse_All(-123, int.MaxValue)]
        [TestCaseSource_RequestResponse_All(0, int.MinValue)]
        [TestCaseSource_RequestResponse_All(0, -123)]
        [TestCaseSource_RequestResponse_All(0, 0)]
        [TestCaseSource_RequestResponse_All(0, 123)]
        [TestCaseSource_RequestResponse_All(0, int.MaxValue)]
        [TestCaseSource_RequestResponse_All(123, int.MinValue)]
        [TestCaseSource_RequestResponse_All(123, -123)]
        [TestCaseSource_RequestResponse_All(123, 0)]
        [TestCaseSource_RequestResponse_All(123, 123)]
        [TestCaseSource_RequestResponse_All(123, int.MaxValue)]
        [TestCaseSource_RequestResponse_All(int.MaxValue, int.MinValue)]
        [TestCaseSource_RequestResponse_All(int.MaxValue, -123)]
        [TestCaseSource_RequestResponse_All(int.MaxValue, 0)]
        [TestCaseSource_RequestResponse_All(int.MaxValue, 123)]
        [TestCaseSource_RequestResponse_All(int.MaxValue, int.MaxValue)]
        public async Task HandleAndCaptureAsync_InvokesMiddlewares_InOrder<TRequest, TResponse>(TRequest request, TResponse response, int orderA, int orderB)
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            var middlewaresExecuted = new List<MockMiddlewareInvocationPipelineItem<TRequest, TResponse>>();

            var middlewareA = new MockMiddlewareInvocationPipelineItem<TRequest, TResponse>() { Order = orderA, };
            AddMiddlewareToExecutionListOnHandleAsyncInvoked(middlewareA);
            AddMiddleware(orderA, middlewareA);

            var middlewareB = new MockMiddlewareInvocationPipelineItem<TRequest, TResponse>() { Order = orderB, };
            AddMiddlewareToExecutionListOnHandleAsyncInvoked(middlewareB);
            AddMiddleware(orderB, middlewareB);

            var middlewareAExpectedIndex = orderA <= orderB ? 0 : 1;
            var middlewareBExpectedIndex = middlewareAExpectedIndex == 1 ? 0 : 1;

            // Act
            _ = await Mediator.HandleAndCaptureAsync<TResponse>(request!, CancellationToken);

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
        [ConformanceTest]
        [TestCaseSource_RequestResponse_All(int.MinValue, int.MinValue)]
        [TestCaseSource_RequestResponse_All(int.MinValue, -123)]
        [TestCaseSource_RequestResponse_All(int.MinValue, 0)]
        [TestCaseSource_RequestResponse_All(int.MinValue, 123)]
        [TestCaseSource_RequestResponse_All(int.MinValue, int.MaxValue)]
        [TestCaseSource_RequestResponse_All(-123, int.MinValue)]
        [TestCaseSource_RequestResponse_All(-123, -123)]
        [TestCaseSource_RequestResponse_All(-123, 0)]
        [TestCaseSource_RequestResponse_All(-123, 123)]
        [TestCaseSource_RequestResponse_All(-123, int.MaxValue)]
        [TestCaseSource_RequestResponse_All(0, int.MinValue)]
        [TestCaseSource_RequestResponse_All(0, -123)]
        [TestCaseSource_RequestResponse_All(0, 0)]
        [TestCaseSource_RequestResponse_All(0, 123)]
        [TestCaseSource_RequestResponse_All(0, int.MaxValue)]
        [TestCaseSource_RequestResponse_All(123, int.MinValue)]
        [TestCaseSource_RequestResponse_All(123, -123)]
        [TestCaseSource_RequestResponse_All(123, 0)]
        [TestCaseSource_RequestResponse_All(123, 123)]
        [TestCaseSource_RequestResponse_All(123, int.MaxValue)]
        [TestCaseSource_RequestResponse_All(int.MaxValue, int.MinValue)]
        [TestCaseSource_RequestResponse_All(int.MaxValue, -123)]
        [TestCaseSource_RequestResponse_All(int.MaxValue, 0)]
        [TestCaseSource_RequestResponse_All(int.MaxValue, 123)]
        [TestCaseSource_RequestResponse_All(int.MaxValue, int.MaxValue)]
        public async Task HandleAsync_InvokesMiddlewares_InOrder<TRequest, TResponse>(TRequest request, TResponse response, int orderA, int orderB)
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            var middlewaresExecuted = new List<MockMiddlewareInvocationPipelineItem<TRequest, TResponse>>();

            var middlewareA = new MockMiddlewareInvocationPipelineItem<TRequest, TResponse>() { Order = orderA, };
            AddMiddlewareToExecutionListOnHandleAsyncInvoked(middlewareA);
            AddMiddleware(orderA, middlewareA);

            var middlewareB = new MockMiddlewareInvocationPipelineItem<TRequest, TResponse>() { Order = orderB, };
            AddMiddlewareToExecutionListOnHandleAsyncInvoked(middlewareB);
            AddMiddleware(orderB, middlewareB);

            var middlewareAExpectedIndex = orderA <= orderB ? 0 : 1;
            var middlewareBExpectedIndex = middlewareAExpectedIndex == 1 ? 0 : 1;

            // Act
            _ = await Mediator.HandleAsync<TResponse>(request!, CancellationToken);

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
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Request(int.MinValue, int.MinValue)]
        [TestCaseSource_RequestResponse_Request(int.MinValue, -123)]
        [TestCaseSource_RequestResponse_Request(int.MinValue, 0)]
        [TestCaseSource_RequestResponse_Request(int.MinValue, 123)]
        [TestCaseSource_RequestResponse_Request(int.MinValue, int.MaxValue)]
        [TestCaseSource_RequestResponse_Request(-123, int.MinValue)]
        [TestCaseSource_RequestResponse_Request(-123, -123)]
        [TestCaseSource_RequestResponse_Request(-123, 0)]
        [TestCaseSource_RequestResponse_Request(-123, 123)]
        [TestCaseSource_RequestResponse_Request(-123, int.MaxValue)]
        [TestCaseSource_RequestResponse_Request(0, int.MinValue)]
        [TestCaseSource_RequestResponse_Request(0, -123)]
        [TestCaseSource_RequestResponse_Request(0, 0)]
        [TestCaseSource_RequestResponse_Request(0, 123)]
        [TestCaseSource_RequestResponse_Request(0, int.MaxValue)]
        [TestCaseSource_RequestResponse_Request(123, int.MinValue)]
        [TestCaseSource_RequestResponse_Request(123, -123)]
        [TestCaseSource_RequestResponse_Request(123, 0)]
        [TestCaseSource_RequestResponse_Request(123, 123)]
        [TestCaseSource_RequestResponse_Request(123, int.MaxValue)]
        [TestCaseSource_RequestResponse_Request(int.MaxValue, int.MinValue)]
        [TestCaseSource_RequestResponse_Request(int.MaxValue, -123)]
        [TestCaseSource_RequestResponse_Request(int.MaxValue, 0)]
        [TestCaseSource_RequestResponse_Request(int.MaxValue, 123)]
        [TestCaseSource_RequestResponse_Request(int.MaxValue, int.MaxValue)]
        public async Task ExecuteAndCaptureAsync_InvokesMiddlewares_InOrder<TRequest, TResponse>(TRequest request, TResponse response, int orderA, int orderB)
            where TRequest : IRequest<TResponse>
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            var middlewaresExecuted = new List<MockMiddlewareInvocationPipelineItem<TRequest, TResponse>>();

            var middlewareA = new MockMiddlewareInvocationPipelineItem<TRequest, TResponse>() { Order = orderA, };
            AddMiddlewareToExecutionListOnHandleAsyncInvoked(middlewareA);
            AddMiddleware(orderA, middlewareA);

            var middlewareB = new MockMiddlewareInvocationPipelineItem<TRequest, TResponse>() { Order = orderB, };
            AddMiddlewareToExecutionListOnHandleAsyncInvoked(middlewareB);
            AddMiddleware(orderB, middlewareB);

            var middlewareAExpectedIndex = orderA <= orderB ? 0 : 1;
            var middlewareBExpectedIndex = middlewareAExpectedIndex == 1 ? 0 : 1;

            // Act
            _ = await Mediator.ExecuteAndCaptureAsync(request!, CancellationToken);

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
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Request(int.MinValue, int.MinValue)]
        [TestCaseSource_RequestResponse_Request(int.MinValue, -123)]
        [TestCaseSource_RequestResponse_Request(int.MinValue, 0)]
        [TestCaseSource_RequestResponse_Request(int.MinValue, 123)]
        [TestCaseSource_RequestResponse_Request(int.MinValue, int.MaxValue)]
        [TestCaseSource_RequestResponse_Request(-123, int.MinValue)]
        [TestCaseSource_RequestResponse_Request(-123, -123)]
        [TestCaseSource_RequestResponse_Request(-123, 0)]
        [TestCaseSource_RequestResponse_Request(-123, 123)]
        [TestCaseSource_RequestResponse_Request(-123, int.MaxValue)]
        [TestCaseSource_RequestResponse_Request(0, int.MinValue)]
        [TestCaseSource_RequestResponse_Request(0, -123)]
        [TestCaseSource_RequestResponse_Request(0, 0)]
        [TestCaseSource_RequestResponse_Request(0, 123)]
        [TestCaseSource_RequestResponse_Request(0, int.MaxValue)]
        [TestCaseSource_RequestResponse_Request(123, int.MinValue)]
        [TestCaseSource_RequestResponse_Request(123, -123)]
        [TestCaseSource_RequestResponse_Request(123, 0)]
        [TestCaseSource_RequestResponse_Request(123, 123)]
        [TestCaseSource_RequestResponse_Request(123, int.MaxValue)]
        [TestCaseSource_RequestResponse_Request(int.MaxValue, int.MinValue)]
        [TestCaseSource_RequestResponse_Request(int.MaxValue, -123)]
        [TestCaseSource_RequestResponse_Request(int.MaxValue, 0)]
        [TestCaseSource_RequestResponse_Request(int.MaxValue, 123)]
        [TestCaseSource_RequestResponse_Request(int.MaxValue, int.MaxValue)]
        public async Task ExecuteAsync_InvokesMiddlewares_InOrder<TRequest, TResponse>(TRequest request, TResponse response, int orderA, int orderB)
            where TRequest : IRequest<TResponse>
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            var middlewaresExecuted = new List<MockMiddlewareInvocationPipelineItem<TRequest, TResponse>>();

            var middlewareA = new MockMiddlewareInvocationPipelineItem<TRequest, TResponse>() { Order = orderA, };
            AddMiddlewareToExecutionListOnHandleAsyncInvoked(middlewareA);
            AddMiddleware(orderA, middlewareA);

            var middlewareB = new MockMiddlewareInvocationPipelineItem<TRequest, TResponse>() { Order = orderB, };
            AddMiddlewareToExecutionListOnHandleAsyncInvoked(middlewareB);
            AddMiddleware(orderB, middlewareB);

            var middlewareAExpectedIndex = orderA <= orderB ? 0 : 1;
            var middlewareBExpectedIndex = middlewareAExpectedIndex == 1 ? 0 : 1;

            // Act
            _ = await Mediator.ExecuteAsync(request!, CancellationToken);

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
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Command(int.MinValue, int.MinValue)]
        [TestCaseSource_RequestResponse_Command(int.MinValue, -123)]
        [TestCaseSource_RequestResponse_Command(int.MinValue, 0)]
        [TestCaseSource_RequestResponse_Command(int.MinValue, 123)]
        [TestCaseSource_RequestResponse_Command(int.MinValue, int.MaxValue)]
        [TestCaseSource_RequestResponse_Command(-123, int.MinValue)]
        [TestCaseSource_RequestResponse_Command(-123, -123)]
        [TestCaseSource_RequestResponse_Command(-123, 0)]
        [TestCaseSource_RequestResponse_Command(-123, 123)]
        [TestCaseSource_RequestResponse_Command(-123, int.MaxValue)]
        [TestCaseSource_RequestResponse_Command(0, int.MinValue)]
        [TestCaseSource_RequestResponse_Command(0, -123)]
        [TestCaseSource_RequestResponse_Command(0, 0)]
        [TestCaseSource_RequestResponse_Command(0, 123)]
        [TestCaseSource_RequestResponse_Command(0, int.MaxValue)]
        [TestCaseSource_RequestResponse_Command(123, int.MinValue)]
        [TestCaseSource_RequestResponse_Command(123, -123)]
        [TestCaseSource_RequestResponse_Command(123, 0)]
        [TestCaseSource_RequestResponse_Command(123, 123)]
        [TestCaseSource_RequestResponse_Command(123, int.MaxValue)]
        [TestCaseSource_RequestResponse_Command(int.MaxValue, int.MinValue)]
        [TestCaseSource_RequestResponse_Command(int.MaxValue, -123)]
        [TestCaseSource_RequestResponse_Command(int.MaxValue, 0)]
        [TestCaseSource_RequestResponse_Command(int.MaxValue, 123)]
        [TestCaseSource_RequestResponse_Command(int.MaxValue, int.MaxValue)]
        public async Task InvokeAndCaptureAsync_InvokesMiddlewares_InOrder<TRequest, TResponse>(TRequest request, TResponse response, int orderA, int orderB)
            where TRequest : ICommand
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            var middlewaresExecuted = new List<MockMiddlewareInvocationPipelineItem<TRequest, TResponse>>();

            var middlewareA = new MockMiddlewareInvocationPipelineItem<TRequest, TResponse>() { Order = orderA, };
            AddMiddlewareToExecutionListOnHandleAsyncInvoked(middlewareA);
            AddMiddleware(orderA, middlewareA);

            var middlewareB = new MockMiddlewareInvocationPipelineItem<TRequest, TResponse>() { Order = orderB, };
            AddMiddlewareToExecutionListOnHandleAsyncInvoked(middlewareB);
            AddMiddleware(orderB, middlewareB);

            var middlewareAExpectedIndex = orderA <= orderB ? 0 : 1;
            var middlewareBExpectedIndex = middlewareAExpectedIndex == 1 ? 0 : 1;

            // Act
            _ = await Mediator.InvokeAndCaptureAsync(request!, CancellationToken);

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
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Command(int.MinValue, int.MinValue)]
        [TestCaseSource_RequestResponse_Command(int.MinValue, -123)]
        [TestCaseSource_RequestResponse_Command(int.MinValue, 0)]
        [TestCaseSource_RequestResponse_Command(int.MinValue, 123)]
        [TestCaseSource_RequestResponse_Command(int.MinValue, int.MaxValue)]
        [TestCaseSource_RequestResponse_Command(-123, int.MinValue)]
        [TestCaseSource_RequestResponse_Command(-123, -123)]
        [TestCaseSource_RequestResponse_Command(-123, 0)]
        [TestCaseSource_RequestResponse_Command(-123, 123)]
        [TestCaseSource_RequestResponse_Command(-123, int.MaxValue)]
        [TestCaseSource_RequestResponse_Command(0, int.MinValue)]
        [TestCaseSource_RequestResponse_Command(0, -123)]
        [TestCaseSource_RequestResponse_Command(0, 0)]
        [TestCaseSource_RequestResponse_Command(0, 123)]
        [TestCaseSource_RequestResponse_Command(0, int.MaxValue)]
        [TestCaseSource_RequestResponse_Command(123, int.MinValue)]
        [TestCaseSource_RequestResponse_Command(123, -123)]
        [TestCaseSource_RequestResponse_Command(123, 0)]
        [TestCaseSource_RequestResponse_Command(123, 123)]
        [TestCaseSource_RequestResponse_Command(123, int.MaxValue)]
        [TestCaseSource_RequestResponse_Command(int.MaxValue, int.MinValue)]
        [TestCaseSource_RequestResponse_Command(int.MaxValue, -123)]
        [TestCaseSource_RequestResponse_Command(int.MaxValue, 0)]
        [TestCaseSource_RequestResponse_Command(int.MaxValue, 123)]
        [TestCaseSource_RequestResponse_Command(int.MaxValue, int.MaxValue)]
        public async Task InvokeAsync_InvokesMiddlewares_InOrder<TRequest, TResponse>(TRequest request, TResponse response, int orderA, int orderB)
            where TRequest : ICommand
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            var middlewaresExecuted = new List<MockMiddlewareInvocationPipelineItem<TRequest, TResponse>>();

            var middlewareA = new MockMiddlewareInvocationPipelineItem<TRequest, TResponse>() { Order = orderA, };
            AddMiddlewareToExecutionListOnHandleAsyncInvoked(middlewareA);
            AddMiddleware(orderA, middlewareA);

            var middlewareB = new MockMiddlewareInvocationPipelineItem<TRequest, TResponse>() { Order = orderB, };
            AddMiddlewareToExecutionListOnHandleAsyncInvoked(middlewareB);
            AddMiddleware(orderB, middlewareB);

            var middlewareAExpectedIndex = orderA <= orderB ? 0 : 1;
            var middlewareBExpectedIndex = middlewareAExpectedIndex == 1 ? 0 : 1;

            // Act
            await Mediator.InvokeAsync(request!, CancellationToken);

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
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Event(int.MinValue, int.MinValue)]
        [TestCaseSource_RequestResponse_Event(int.MinValue, -123)]
        [TestCaseSource_RequestResponse_Event(int.MinValue, 0)]
        [TestCaseSource_RequestResponse_Event(int.MinValue, 123)]
        [TestCaseSource_RequestResponse_Event(int.MinValue, int.MaxValue)]
        [TestCaseSource_RequestResponse_Event(-123, int.MinValue)]
        [TestCaseSource_RequestResponse_Event(-123, -123)]
        [TestCaseSource_RequestResponse_Event(-123, 0)]
        [TestCaseSource_RequestResponse_Event(-123, 123)]
        [TestCaseSource_RequestResponse_Event(-123, int.MaxValue)]
        [TestCaseSource_RequestResponse_Event(0, int.MinValue)]
        [TestCaseSource_RequestResponse_Event(0, -123)]
        [TestCaseSource_RequestResponse_Event(0, 0)]
        [TestCaseSource_RequestResponse_Event(0, 123)]
        [TestCaseSource_RequestResponse_Event(0, int.MaxValue)]
        [TestCaseSource_RequestResponse_Event(123, int.MinValue)]
        [TestCaseSource_RequestResponse_Event(123, -123)]
        [TestCaseSource_RequestResponse_Event(123, 0)]
        [TestCaseSource_RequestResponse_Event(123, 123)]
        [TestCaseSource_RequestResponse_Event(123, int.MaxValue)]
        [TestCaseSource_RequestResponse_Event(int.MaxValue, int.MinValue)]
        [TestCaseSource_RequestResponse_Event(int.MaxValue, -123)]
        [TestCaseSource_RequestResponse_Event(int.MaxValue, 0)]
        [TestCaseSource_RequestResponse_Event(int.MaxValue, 123)]
        [TestCaseSource_RequestResponse_Event(int.MaxValue, int.MaxValue)]
        public async Task PublishAndCaptureAsync_InvokesMiddlewares_InOrder<TRequest, TResponse>(TRequest request, TResponse response, int orderA, int orderB)
            where TRequest : IEvent
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            var middlewaresExecuted = new List<MockMiddlewareInvocationPipelineItem<TRequest, TResponse>>();

            var middlewareA = new MockMiddlewareInvocationPipelineItem<TRequest, TResponse>() { Order = orderA, };
            AddMiddlewareToExecutionListOnHandleAsyncInvoked(middlewareA);
            AddMiddleware(orderA, middlewareA);

            var middlewareB = new MockMiddlewareInvocationPipelineItem<TRequest, TResponse>() { Order = orderB, };
            AddMiddlewareToExecutionListOnHandleAsyncInvoked(middlewareB);
            AddMiddleware(orderB, middlewareB);

            var middlewareAExpectedIndex = orderA <= orderB ? 0 : 1;
            var middlewareBExpectedIndex = middlewareAExpectedIndex == 1 ? 0 : 1;

            // Act
            _ = await Mediator.PublishAndCaptureAsync(request!, CancellationToken);

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
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Event(int.MinValue, int.MinValue)]
        [TestCaseSource_RequestResponse_Event(int.MinValue, -123)]
        [TestCaseSource_RequestResponse_Event(int.MinValue, 0)]
        [TestCaseSource_RequestResponse_Event(int.MinValue, 123)]
        [TestCaseSource_RequestResponse_Event(int.MinValue, int.MaxValue)]
        [TestCaseSource_RequestResponse_Event(-123, int.MinValue)]
        [TestCaseSource_RequestResponse_Event(-123, -123)]
        [TestCaseSource_RequestResponse_Event(-123, 0)]
        [TestCaseSource_RequestResponse_Event(-123, 123)]
        [TestCaseSource_RequestResponse_Event(-123, int.MaxValue)]
        [TestCaseSource_RequestResponse_Event(0, int.MinValue)]
        [TestCaseSource_RequestResponse_Event(0, -123)]
        [TestCaseSource_RequestResponse_Event(0, 0)]
        [TestCaseSource_RequestResponse_Event(0, 123)]
        [TestCaseSource_RequestResponse_Event(0, int.MaxValue)]
        [TestCaseSource_RequestResponse_Event(123, int.MinValue)]
        [TestCaseSource_RequestResponse_Event(123, -123)]
        [TestCaseSource_RequestResponse_Event(123, 0)]
        [TestCaseSource_RequestResponse_Event(123, 123)]
        [TestCaseSource_RequestResponse_Event(123, int.MaxValue)]
        [TestCaseSource_RequestResponse_Event(int.MaxValue, int.MinValue)]
        [TestCaseSource_RequestResponse_Event(int.MaxValue, -123)]
        [TestCaseSource_RequestResponse_Event(int.MaxValue, 0)]
        [TestCaseSource_RequestResponse_Event(int.MaxValue, 123)]
        [TestCaseSource_RequestResponse_Event(int.MaxValue, int.MaxValue)]
        public async Task PublishAsync_InvokesMiddlewares_InOrder<TRequest, TResponse>(TRequest request, TResponse response, int orderA, int orderB)
            where TRequest : IEvent
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            var middlewaresExecuted = new List<MockMiddlewareInvocationPipelineItem<TRequest, TResponse>>();

            var middlewareA = new MockMiddlewareInvocationPipelineItem<TRequest, TResponse>() { Order = orderA, };
            AddMiddlewareToExecutionListOnHandleAsyncInvoked(middlewareA);
            AddMiddleware(orderA, middlewareA);

            var middlewareB = new MockMiddlewareInvocationPipelineItem<TRequest, TResponse>() { Order = orderB, };
            AddMiddlewareToExecutionListOnHandleAsyncInvoked(middlewareB);
            AddMiddleware(orderB, middlewareB);

            var middlewareAExpectedIndex = orderA <= orderB ? 0 : 1;
            var middlewareBExpectedIndex = middlewareAExpectedIndex == 1 ? 0 : 1;

            // Act
            await Mediator.PublishAsync(request!, CancellationToken);

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
    }
}
