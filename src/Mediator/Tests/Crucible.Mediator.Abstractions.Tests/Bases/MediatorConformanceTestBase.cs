using Crucible.Mediator.Abstractions.Tests.Data;
using Crucible.Mediator.Abstractions.Tests.Data.Annotations.Commands;
using Crucible.Mediator.Abstractions.Tests.Data.Annotations.Events;
using Crucible.Mediator.Abstractions.Tests.Data.Annotations.Invocation;
using Crucible.Mediator.Abstractions.Tests.Data.Annotations.Requests;
using Crucible.Mediator.Abstractions.Tests.Internal;
using Crucible.Mediator.Abstractions.Tests.Invocation.Mocks;
using Crucible.Mediator.Commands;
using Crucible.Mediator.Events;
using Crucible.Mediator.Invocation;
using Crucible.Mediator.Requests;
using Moq;

namespace Crucible.Mediator.Abstractions.Tests.Bases
{
    public abstract class MediatorConformanceTestBase<TMediator>
        where TMediator : IMediator
    {
        protected ICollection<object> Middlewares { get; } = [];

        protected ICollection<object> Handlers { get; } = [];

        protected Lazy<TMediator> MediatorInitializer { get; }

        protected TMediator Mediator => MediatorInitializer.Value;

        protected CancellationToken CancellationToken { get; set; } = CancellationToken.None;

        protected MediatorConformanceTestBase()
        {
#pragma warning disable IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
            MediatorInitializer = new Lazy<TMediator>(() => CreateMediator());
#pragma warning restore IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
        }

        protected abstract TMediator CreateMediator();

        protected void AddMiddleware<TRequest, TResponse>(IInvocationMiddleware<TRequest, TResponse> middleware)
        {
            Middlewares.Add(middleware);
            RegisterMiddleware(middleware);
        }

        protected abstract void RegisterMiddleware<TRequest, TResponse>(IInvocationMiddleware<TRequest, TResponse> middleware);

        protected void AddHandler<TRequest, TResponse>(IInvocationHandler<TRequest, TResponse> handler)
        {
            Handlers.Add(handler);
            RegisterHandler(handler);
        }

        protected abstract void RegisterHandler<TRequest, TResponse>(IInvocationHandler<TRequest, TResponse> handler);

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_All(1)]
        [TestCaseSource_RequestResponse_All(10)]
        public async Task HandleAndCaptureAsync_InvokesHandlers<TRequest, TResponse>(TRequest request, TResponse response, int handlerCount)
        {
            // Arrange
            for (var i = 0; i < handlerCount; i++)
            {
                var handler = new MockInvocationHandler<TRequest, TResponse>();
                AddHandler(handler);
            }

            // Act
            _ = await Mediator.HandleAndCaptureAsync<TResponse>(request!, CancellationToken);

            // Assert
            foreach (var handler in Handlers.Cast<MockInvocationHandler<TRequest, TResponse>>())
            {
                handler.Mock.Verify(m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()), Times.Once);
            }
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_All]
        public async Task HandleAndCaptureAsync_DoesNotInvokeUnrelatedHandler<TRequest, TResponse>(TRequest request, TResponse response)
        {
            // Arrange
            var expectedHandler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(expectedHandler);

            var unrelatedHandler = new MockInvocationHandler<MediatorTestData.Unrelated, MediatorTestData.Unrelated>();
            AddHandler(unrelatedHandler);

            // Act
            _ = await Mediator.HandleAndCaptureAsync<TResponse>(request!, CancellationToken);

            // Assert
            unrelatedHandler.Mock.Verify(m => m.HandleAsync(It.IsAny<MediatorTestData.Unrelated>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponseMediatorRequestResponse_Applicable(isApplicable: true, 1)]
        [TestCaseSource_RequestResponseMediatorRequestResponse_Applicable(isApplicable: true, 10)]
        public async Task HandleAndCaptureAsync_InvokesMiddlewares<TRequest, TResponse, TMiddlewareRequest, TMiddlewareResponse>(TRequest request, TResponse response, TMiddlewareRequest middlewareRequest, TMiddlewareResponse middlewareResponse, int middlewareCount)
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            for (var i = 0; i < middlewareCount; i++)
            {
                var middleware = new MockInvocationMiddleware<TMiddlewareRequest, TMiddlewareResponse>();
                AddMiddleware(middleware);
            }

            // Act
            _ = await Mediator.HandleAndCaptureAsync<TResponse>(request!, CancellationToken);

            // Assert
            foreach (var middleware in Middlewares.Cast<MockInvocationMiddleware<TMiddlewareRequest, TMiddlewareResponse>>())
            {
                middleware.Mock.Verify(m => m.HandleAsync(It.IsAny<IInvocationContext<TMiddlewareRequest, TMiddlewareResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Once);
            }
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponseMediatorRequestResponse_Applicable(isApplicable: false)]
        public async Task HandleAndCaptureAsync_DoesNotInvokeUnrelatedMiddleware<TRequest, TResponse, TMiddlewareRequest, TMiddlewareResponse>(TRequest request, TResponse response, TMiddlewareRequest middlewareRequest, TMiddlewareResponse middlewareResponse)
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            var middleware = new MockInvocationMiddleware<TMiddlewareRequest, TMiddlewareResponse>();
            AddMiddleware(middleware);

            // Act
            _ = await Mediator.HandleAndCaptureAsync<TResponse>(request!, CancellationToken);

            // Assert
            middleware.Mock.Verify(m => m.HandleAsync(It.IsAny<IInvocationContext<TMiddlewareRequest, TMiddlewareResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_All]
        public async Task HandleAndCaptureAsync_ContextIsNotNull<TRequest, TResponse>(TRequest request, TResponse response)
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            // Act
            var context = await Mediator.HandleAndCaptureAsync<TResponse>(request!, CancellationToken);

            // Assert
            Assert.That(context, Is.Not.Null);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_All]
        public async Task HandleAndCaptureAsync_ContextContainsExpectedResponse<TRequest, TResponse>(TRequest request, TResponse response)
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>
            {
                Response = response
            };
            AddHandler(handler);

            // Act
            var context = await Mediator.HandleAndCaptureAsync<TResponse>(request!, CancellationToken);

            // Assert
            Assert.That(context.Response, Is.SameAs(response));
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_All]
        public void HandleAndCaptureAsync_DoesNotThrow_WhenHandlerThrows<TRequest, TResponse>(TRequest request, TResponse response)
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            handler.Mock
                .Setup(m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()))
                .Throws(new Exception("sample exception"));
            AddHandler(handler);

            // Act / Assert
            Assert.DoesNotThrowAsync(() => Mediator.HandleAndCaptureAsync<TResponse>(request!, CancellationToken));
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_All]
        public async Task HandleAndCaptureAsync_ContextHasError_WhenHandlerThrows<TRequest, TResponse>(TRequest request, TResponse response)
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            handler.Mock
                .Setup(m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()))
                .Throws(new Exception("sample exception"));
            AddHandler(handler);

            // Act
            var context = await Mediator.HandleAndCaptureAsync<TResponse>(request!, CancellationToken);

            // Assert
            Assert.That(context.HasError, Is.True);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_All]
        public void HandleAndCaptureAsync_DoesNotThrow_WhenMiddlewareThrows<TRequest, TResponse>(TRequest request, TResponse response)
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            var middleware = new MockInvocationMiddleware<TRequest, TResponse>();
            middleware.Mock
                .Setup(m => m.HandleAsync(It.IsAny<IInvocationContext<TRequest, TResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
                .Throws(new Exception("sample exception"));
            AddMiddleware(middleware);

            // Act / Assert
            Assert.DoesNotThrowAsync(() => Mediator.HandleAndCaptureAsync<TResponse>(request!, CancellationToken));
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_All]
        public async Task HandleAndCaptureAsync_ContextHasError_WhenMiddlewareThrows<TRequest, TResponse>(TRequest request, TResponse response)
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            var middleware = new MockInvocationMiddleware<TRequest, TResponse>();
            middleware.Mock
                .Setup(m => m.HandleAsync(It.IsAny<IInvocationContext<TRequest, TResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
                .Throws(new Exception("sample exception"));
            AddMiddleware(middleware);

            // Act
            var context = await Mediator.HandleAndCaptureAsync<TResponse>(request!, CancellationToken);

            // Assert
            Assert.That(context.HasError, Is.True);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_All]
        public async Task HandleAndCaptureAsync_ContextResponse_IsExpectedResponse<TRequest, TResponse>(TRequest request, TResponse response)
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>()
            {
                Response = response,
            };
            AddHandler(handler);

            // Act
            var context = await Mediator.HandleAndCaptureAsync<TResponse>(request!, CancellationToken);

            // Assert
            Assert.That(context.Response, Is.EqualTo(response));
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_All(1)]
        [TestCaseSource_RequestResponse_All(10)]
        public async Task HandleAsync_InvokesHandlers<TRequest, TResponse>(TRequest request, TResponse response, int handlerCount)
        {
            // Arrange
            for (var i = 0; i < handlerCount; i++)
            {
                var handler = new MockInvocationHandler<TRequest, TResponse>();
                AddHandler(handler);
            }

            // Act
            _ = await Mediator.HandleAsync<TResponse>(request!, CancellationToken);

            // Assert
            foreach (var handler in Handlers.Cast<MockInvocationHandler<TRequest, TResponse>>())
            {
                handler.Mock.Verify(m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()), Times.Once);
            }
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_All]
        public async Task HandleAsync_DoesNotInvokeUnrelatedHandler<TRequest, TResponse>(TRequest request, TResponse response)
        {
            // Arrange
            var expectedHandler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(expectedHandler);

            var unrelatedHandler = new MockInvocationHandler<MediatorTestData.Unrelated, MediatorTestData.Unrelated>();
            AddHandler(unrelatedHandler);

            // Act
            _ = await Mediator.HandleAsync<TResponse>(request!, CancellationToken);

            // Assert
            unrelatedHandler.Mock.Verify(m => m.HandleAsync(It.IsAny<MediatorTestData.Unrelated>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponseMediatorRequestResponse_Applicable(isApplicable: true, 1)]
        [TestCaseSource_RequestResponseMediatorRequestResponse_Applicable(isApplicable: true, 10)]
        public async Task HandleAsync_InvokesMiddlewares<TRequest, TResponse, TMiddlewareRequest, TMiddlewareResponse>(TRequest request, TResponse response, TMiddlewareRequest middlewareRequest, TMiddlewareResponse middlewareResponse, int middlewareCount)
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            for (var i = 0; i < middlewareCount; i++)
            {
                var middleware = new MockInvocationMiddleware<TMiddlewareRequest, TMiddlewareResponse>();
                AddMiddleware(middleware);
            }

            // Act
            _ = await Mediator.HandleAsync<TResponse>(request!, CancellationToken);

            // Assert
            foreach (var middleware in Middlewares.Cast<MockInvocationMiddleware<TMiddlewareRequest, TMiddlewareResponse>>())
            {
                middleware.Mock.Verify(m => m.HandleAsync(It.IsAny<IInvocationContext<TMiddlewareRequest, TMiddlewareResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Once);
            }
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponseMediatorRequestResponse_Applicable(isApplicable: false)]
        public async Task HandleAsync_DoesNotInvokeUnrelatedMiddleware<TRequest, TResponse, TMiddlewareRequest, TMiddlewareResponse>(TRequest request, TResponse response, TMiddlewareRequest middlewareRequest, TMiddlewareResponse middlewareResponse)
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            var middleware = new MockInvocationMiddleware<TMiddlewareRequest, TMiddlewareResponse>();
            AddMiddleware(middleware);

            // Act
            _ = await Mediator.HandleAsync<TResponse>(request!, CancellationToken);

            // Assert
            middleware.Mock.Verify(m => m.HandleAsync(It.IsAny<IInvocationContext<TMiddlewareRequest, TMiddlewareResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_All]
        public void HandleAsync_Throws_WhenHandlerThrows<TRequest, TResponse>(TRequest request, TResponse response)
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            handler.Mock
                .Setup(m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()))
                .Throws(new Exception("sample exception"));
            AddHandler(handler);

            // Act / Assert
            Assert.ThrowsAsync<Exception>(() => Mediator.HandleAsync<TResponse>(request!, CancellationToken));
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_All]
        public void HandleAsync_Throws_WhenMiddlewareThrows<TRequest, TResponse>(TRequest request, TResponse response)
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            var middleware = new MockInvocationMiddleware<TRequest, TResponse>();
            middleware.Mock
                .Setup(m => m.HandleAsync(It.IsAny<IInvocationContext<TRequest, TResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
                .Throws(new Exception("sample exception"));
            AddMiddleware(middleware);

            // Act / Assert
            Assert.ThrowsAsync<Exception>(() => Mediator.HandleAsync<TResponse>(request!, CancellationToken));
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_All]
        public async Task HandleAsync_ReturnsExpectedResponse<TRequest, TResponse>(TRequest request, TResponse response)
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>()
            {
                Response = response,
            };
            AddHandler(handler);

            // Act
            var actualResponse = await Mediator.HandleAsync<TResponse>(request!, CancellationToken);

            // Assert
            Assert.That(actualResponse, Is.EqualTo(response));
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Request]
        public async Task ExecuteAndCaptureAsync_InvokesHandler<TRequest, TResponse>(TRequest request, TResponse response)
            where TRequest : IRequest<TResponse>
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            // Act
            _ = await Mediator.ExecuteAndCaptureAsync(request!, CancellationToken);

            // Assert
            handler.Mock.Verify(m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Request]
        public async Task ExecuteAndCaptureAsync_DoesNotInvokeUnrelatedHandler<TRequest, TResponse>(TRequest request, TResponse response)
            where TRequest : IRequest<TResponse>
        {
            // Arrange
            var expectedHandler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(expectedHandler);

            var unrelatedHandler = new MockInvocationHandler<MediatorTestData.Unrelated, MediatorTestData.Unrelated>();
            AddHandler(unrelatedHandler);

            // Act
            _ = await Mediator.ExecuteAndCaptureAsync(request!, CancellationToken);

            // Assert
            unrelatedHandler.Mock.Verify(m => m.HandleAsync(It.IsAny<MediatorTestData.Unrelated>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponseMediatorRequestResponse_Applicable(isApplicable: true, typeof(IRequest<>), 1)]
        [TestCaseSource_RequestResponseMediatorRequestResponse_Applicable(isApplicable: true, typeof(IRequest<>), 10)]
        public async Task ExecuteAndCaptureAsync_InvokesMiddlewares<TRequest, TResponse, TMiddlewareRequest, TMiddlewareResponse>(TRequest request, TResponse response, TMiddlewareRequest middlewareRequest, TMiddlewareResponse middlewareResponse, int middlewareCount)
            where TRequest : IRequest<TResponse>
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            for (var i = 0; i < middlewareCount; i++)
            {
                var middleware = new MockInvocationMiddleware<TMiddlewareRequest, TMiddlewareResponse>();
                AddMiddleware(middleware);
            }

            // Act
            _ = await Mediator.ExecuteAndCaptureAsync(request!, CancellationToken);

            // Assert
            foreach (var middleware in Middlewares.Cast<MockInvocationMiddleware<TMiddlewareRequest, TMiddlewareResponse>>())
            {
                middleware.Mock.Verify(m => m.HandleAsync(It.IsAny<IInvocationContext<TMiddlewareRequest, TMiddlewareResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Once);
            }
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponseMediatorRequestResponse_Applicable(isApplicable: false, typeof(IRequest<>))]
        public async Task ExecuteAndCaptureAsync_DoesNotInvokeUnrelatedMiddleware<TRequest, TResponse, TMiddlewareRequest, TMiddlewareResponse>(TRequest request, TResponse response, TMiddlewareRequest middlewareRequest, TMiddlewareResponse middlewareResponse)
            where TRequest : IRequest<TResponse>
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            var middleware = new MockInvocationMiddleware<TMiddlewareRequest, TMiddlewareResponse>();
            AddMiddleware(middleware);

            // Act
            _ = await Mediator.ExecuteAndCaptureAsync(request!, CancellationToken);

            // Assert
            middleware.Mock.Verify(m => m.HandleAsync(It.IsAny<IInvocationContext<TMiddlewareRequest, TMiddlewareResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Request]
        public async Task ExecuteAndCaptureAsync_ContextIsNotNull<TRequest, TResponse>(TRequest request, TResponse response)
            where TRequest : IRequest<TResponse>
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            // Act
            var context = await Mediator.ExecuteAndCaptureAsync(request!, CancellationToken);

            // Assert
            Assert.That(context, Is.Not.Null);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Request]
        public async Task ExecuteAndCaptureAsync_ContextContainsExpectedResponse<TRequest, TResponse>(TRequest request, TResponse response)
            where TRequest : IRequest<TResponse>
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>
            {
                Response = response
            };
            AddHandler(handler);

            // Act
            var context = await Mediator.ExecuteAndCaptureAsync(request!, CancellationToken);

            // Assert
            Assert.That(context.Response, Is.SameAs(response));
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Request]
        public void ExecuteAndCaptureAsync_DoesNotThrow_WhenHandlerThrows<TRequest, TResponse>(TRequest request, TResponse response)
            where TRequest : IRequest<TResponse>
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            handler.Mock
                .Setup(m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()))
                .Throws(new Exception("sample exception"));
            AddHandler(handler);

            // Act / Assert
            Assert.DoesNotThrowAsync(() => Mediator.ExecuteAndCaptureAsync(request!, CancellationToken));
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Request]
        public async Task ExecuteAndCaptureAsync_ContextHasError_WhenHandlerThrows<TRequest, TResponse>(TRequest request, TResponse response)
            where TRequest : IRequest<TResponse>
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            handler.Mock
                .Setup(m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()))
                .Throws(new Exception("sample exception"));
            AddHandler(handler);

            // Act
            var context = await Mediator.ExecuteAndCaptureAsync(request!, CancellationToken);

            // Assert
            Assert.That(context.HasError, Is.True);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Request]
        public void ExecuteAndCaptureAsync_DoesNotThrow_WhenMiddlewareThrows<TRequest, TResponse>(TRequest request, TResponse response)
            where TRequest : IRequest<TResponse>
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            var middleware = new MockInvocationMiddleware<TRequest, TResponse>();
            middleware.Mock
                .Setup(m => m.HandleAsync(It.IsAny<IInvocationContext<TRequest, TResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
                .Throws(new Exception("sample exception"));
            AddMiddleware(middleware);

            // Act / Assert
            Assert.DoesNotThrowAsync(() => Mediator.ExecuteAndCaptureAsync(request!, CancellationToken));
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Request]
        public async Task ExecuteAndCaptureAsync_ContextHasError_WhenMiddlewareThrows<TRequest, TResponse>(TRequest request, TResponse response)
            where TRequest : IRequest<TResponse>
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            var middleware = new MockInvocationMiddleware<TRequest, TResponse>();
            middleware.Mock
                .Setup(m => m.HandleAsync(It.IsAny<IInvocationContext<TRequest, TResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
                .Throws(new Exception("sample exception"));
            AddMiddleware(middleware);

            // Act
            var context = await Mediator.ExecuteAndCaptureAsync(request!, CancellationToken);

            // Assert
            Assert.That(context.HasError, Is.True);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Request]
        public async Task ExecuteAndCaptureAsync_ContextResponse_IsExpectedResponse<TRequest, TResponse>(TRequest request, TResponse response)
            where TRequest : IRequest<TResponse>
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>()
            {
                Response = response,
            };
            AddHandler(handler);

            // Act
            var context = await Mediator.ExecuteAndCaptureAsync(request!, CancellationToken);

            // Assert
            Assert.That(context.Response, Is.EqualTo(response));
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Request]
        public async Task ExecuteAsync_InvokesHandler<TRequest, TResponse>(TRequest request, TResponse response)
            where TRequest : IRequest<TResponse>
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            // Act
            _ = await Mediator.ExecuteAsync(request!, CancellationToken);

            // Assert
            handler.Mock.Verify(m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Request]
        public async Task ExecuteAsync_DoesNotInvokeUnrelatedHandler<TRequest, TResponse>(TRequest request, TResponse response)
            where TRequest : IRequest<TResponse>
        {
            // Arrange
            var expectedHandler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(expectedHandler);

            var unrelatedHandler = new MockInvocationHandler<MediatorTestData.Unrelated, MediatorTestData.Unrelated>();
            AddHandler(unrelatedHandler);

            // Act
            _ = await Mediator.ExecuteAsync(request!, CancellationToken);

            // Assert
            unrelatedHandler.Mock.Verify(m => m.HandleAsync(It.IsAny<MediatorTestData.Unrelated>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponseMediatorRequestResponse_Applicable(isApplicable: true, typeof(IRequest<>), 1)]
        [TestCaseSource_RequestResponseMediatorRequestResponse_Applicable(isApplicable: true, typeof(IRequest<>), 10)]
        public async Task ExecuteAsync_InvokesMiddlewares<TRequest, TResponse, TMiddlewareRequest, TMiddlewareResponse>(TRequest request, TResponse response, TMiddlewareRequest middlewareRequest, TMiddlewareResponse middlewareResponse, int middlewareCount)
            where TRequest : IRequest<TResponse>
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            for (var i = 0; i < middlewareCount; i++)
            {
                var middleware = new MockInvocationMiddleware<TMiddlewareRequest, TMiddlewareResponse>();
                AddMiddleware(middleware);
            }

            // Act
            _ = await Mediator.ExecuteAsync(request!, CancellationToken);

            // Assert
            foreach (var middleware in Middlewares.Cast<MockInvocationMiddleware<TMiddlewareRequest, TMiddlewareResponse>>())
            {
                middleware.Mock.Verify(m => m.HandleAsync(It.IsAny<IInvocationContext<TMiddlewareRequest, TMiddlewareResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Once);
            }
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponseMediatorRequestResponse_Applicable(isApplicable: false, typeof(IRequest<>))]
        public async Task ExecuteAsync_DoesNotInvokeUnrelatedMiddleware<TRequest, TResponse, TMiddlewareRequest, TMiddlewareResponse>(TRequest request, TResponse response, TMiddlewareRequest middlewareRequest, TMiddlewareResponse middlewareResponse)
            where TRequest : IRequest<TResponse>
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            var middleware = new MockInvocationMiddleware<TMiddlewareRequest, TMiddlewareResponse>();
            AddMiddleware(middleware);

            // Act
            _ = await Mediator.ExecuteAsync(request!, CancellationToken);

            // Assert
            middleware.Mock.Verify(m => m.HandleAsync(It.IsAny<IInvocationContext<TMiddlewareRequest, TMiddlewareResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Request]
        public void ExecuteAsync_Throws_WhenHandlerThrows<TRequest, TResponse>(TRequest request, TResponse response)
            where TRequest : IRequest<TResponse>
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            handler.Mock
                .Setup(m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()))
                .Throws(new Exception("sample exception"));
            AddHandler(handler);

            // Act / Assert
            Assert.ThrowsAsync<Exception>(() => Mediator.ExecuteAsync(request!, CancellationToken));
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Request]
        public void ExecuteAsync_Throws_WhenMiddlewareThrows<TRequest, TResponse>(TRequest request, TResponse response)
            where TRequest : IRequest<TResponse>
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            var middleware = new MockInvocationMiddleware<TRequest, TResponse>();
            middleware.Mock
                .Setup(m => m.HandleAsync(It.IsAny<IInvocationContext<TRequest, TResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
                .Throws(new Exception("sample exception"));
            AddMiddleware(middleware);

            // Act / Assert
            Assert.ThrowsAsync<Exception>(() => Mediator.ExecuteAsync(request!, CancellationToken));
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Request]
        public async Task ExecuteAsync_ReturnsExpectedResponse<TRequest, TResponse>(TRequest request, TResponse response)
            where TRequest : IRequest<TResponse>
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>()
            {
                Response = response,
            };
            AddHandler(handler);

            // Act
            var actualResponse = await Mediator.ExecuteAsync(request!, CancellationToken);

            // Assert
            Assert.That(actualResponse, Is.EqualTo(response));
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Event(1)]
        [TestCaseSource_RequestResponse_Event(10)]
        public async Task PublishAndCaptureAsync_InvokesHandlers<TRequest, TResponse>(TRequest request, TResponse response, int handlerCount)
            where TRequest : IEvent
        {
            // Arrange
            for (var i = 0; i < handlerCount; i++)
            {
                var handler = new MockInvocationHandler<TRequest, TResponse>();
                AddHandler(handler);
            }

            // Act
            _ = await Mediator.PublishAndCaptureAsync(request!, CancellationToken);

            // Assert
            foreach (var handler in Handlers.Cast<MockInvocationHandler<TRequest, TResponse>>())
            {
                handler.Mock.Verify(m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()), Times.Once);
            }
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Event]
        public async Task PublishAndCaptureAsync_DoesNotInvokeUnrelatedHandler<TRequest, TResponse>(TRequest request, TResponse response)
            where TRequest : IEvent
        {
            // Arrange
            var expectedHandler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(expectedHandler);

            var unrelatedHandler = new MockInvocationHandler<MediatorTestData.Unrelated, MediatorTestData.Unrelated>();
            AddHandler(unrelatedHandler);

            // Act
            _ = await Mediator.PublishAndCaptureAsync(request!, CancellationToken);

            // Assert
            unrelatedHandler.Mock.Verify(m => m.HandleAsync(It.IsAny<MediatorTestData.Unrelated>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponseMediatorRequestResponse_Applicable(isApplicable: true, typeof(IEvent), 1)]
        [TestCaseSource_RequestResponseMediatorRequestResponse_Applicable(isApplicable: true, typeof(IEvent), 10)]
        public async Task PublishAndCaptureAsync_InvokesMiddlewares<TRequest, TResponse, TMiddlewareRequest, TMiddlewareResponse>(TRequest request, TResponse response, TMiddlewareRequest middlewareRequest, TMiddlewareResponse middlewareResponse, int middlewareCount)
            where TRequest : IEvent
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            for (var i = 0; i < middlewareCount; i++)
            {
                var middleware = new MockInvocationMiddleware<TMiddlewareRequest, TMiddlewareResponse>();
                AddMiddleware(middleware);
            }

            // Act
            _ = await Mediator.PublishAndCaptureAsync(request!, CancellationToken);

            // Assert
            foreach (var middleware in Middlewares.Cast<MockInvocationMiddleware<TMiddlewareRequest, TMiddlewareResponse>>())
            {
                middleware.Mock.Verify(m => m.HandleAsync(It.IsAny<IInvocationContext<TMiddlewareRequest, TMiddlewareResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Once);
            }
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponseMediatorRequestResponse_Applicable(isApplicable: false, typeof(IEvent))]
        public async Task PublishAndCaptureAsync_DoesNotInvokeUnrelatedMiddleware<TRequest, TResponse, TMiddlewareRequest, TMiddlewareResponse>(TRequest request, TResponse response, TMiddlewareRequest middlewareRequest, TMiddlewareResponse middlewareResponse)
            where TRequest : IEvent
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            var middleware = new MockInvocationMiddleware<TMiddlewareRequest, TMiddlewareResponse>();
            AddMiddleware(middleware);

            // Act
            _ = await Mediator.PublishAndCaptureAsync(request!, CancellationToken);

            // Assert
            middleware.Mock.Verify(m => m.HandleAsync(It.IsAny<IInvocationContext<TMiddlewareRequest, TMiddlewareResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Event]
        public async Task PublishAndCaptureAsync_ContextIsNotNull<TRequest, TResponse>(TRequest request, TResponse response)
            where TRequest : IEvent
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            // Act
            var context = await Mediator.PublishAndCaptureAsync(request!, CancellationToken);

            // Assert
            Assert.That(context, Is.Not.Null);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Event]
        public void PublishAndCaptureAsync_DoesNotThrow_WhenHandlerThrows<TRequest, TResponse>(TRequest request, TResponse response)
            where TRequest : IEvent
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            handler.Mock
                .Setup(m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()))
                .Throws(new Exception("sample exception"));
            AddHandler(handler);

            // Act / Assert
            Assert.DoesNotThrowAsync(() => Mediator.PublishAndCaptureAsync(request!, CancellationToken));
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Event]
        public async Task PublishAndCaptureAsync_ContextHasError_WhenHandlerThrows<TRequest, TResponse>(TRequest request, TResponse response)
            where TRequest : IEvent
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            handler.Mock
                .Setup(m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()))
                .Throws(new Exception("sample exception"));
            AddHandler(handler);

            // Act
            var context = await Mediator.PublishAndCaptureAsync(request!, CancellationToken);

            // Assert
            Assert.That(context.HasError, Is.True);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Event]
        public void PublishAndCaptureAsync_DoesNotThrow_WhenMiddlewareThrows<TRequest, TResponse>(TRequest request, TResponse response)
            where TRequest : IEvent
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            var middleware = new MockInvocationMiddleware<TRequest, TResponse>();
            middleware.Mock
                .Setup(m => m.HandleAsync(It.IsAny<IInvocationContext<TRequest, TResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
                .Throws(new Exception("sample exception"));
            AddMiddleware(middleware);

            // Act / Assert
            Assert.DoesNotThrowAsync(() => Mediator.PublishAndCaptureAsync(request!, CancellationToken));
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Event]
        public async Task PublishAndCaptureAsync_ContextHasError_WhenMiddlewareThrows<TRequest, TResponse>(TRequest request, TResponse response)
            where TRequest : IEvent
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            var middleware = new MockInvocationMiddleware<TRequest, TResponse>();
            middleware.Mock
                .Setup(m => m.HandleAsync(It.IsAny<IInvocationContext<TRequest, TResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
                .Throws(new Exception("sample exception"));
            AddMiddleware(middleware);

            // Act
            var context = await Mediator.PublishAndCaptureAsync(request!, CancellationToken);

            // Assert
            Assert.That(context.HasError, Is.True);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Event(1)]
        [TestCaseSource_RequestResponse_Event(10)]
        public async Task PublishAsync_InvokesHandlers<TRequest, TResponse>(TRequest request, TResponse response, int handlerCount)
            where TRequest : IEvent
        {
            // Arrange
            for (var i = 0; i < handlerCount; i++)
            {
                var handler = new MockInvocationHandler<TRequest, TResponse>();
                AddHandler(handler);
            }

            // Act
            await Mediator.PublishAsync(request!, CancellationToken);

            // Assert
            foreach (var handler in Handlers.Cast<MockInvocationHandler<TRequest, TResponse>>())
            {
                handler.Mock.Verify(m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()), Times.Once);
            }
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Event]
        public async Task PublishAsync_DoesNotInvokeUnrelatedHandler<TRequest, TResponse>(TRequest request, TResponse response)
            where TRequest : IEvent
        {
            // Arrange
            var expectedHandler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(expectedHandler);

            var unrelatedHandler = new MockInvocationHandler<MediatorTestData.Unrelated, MediatorTestData.Unrelated>();
            AddHandler(unrelatedHandler);

            // Act
            await Mediator.PublishAsync(request!, CancellationToken);

            // Assert
            unrelatedHandler.Mock.Verify(m => m.HandleAsync(It.IsAny<MediatorTestData.Unrelated>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponseMediatorRequestResponse_Applicable(isApplicable: true, typeof(IEvent), 1)]
        [TestCaseSource_RequestResponseMediatorRequestResponse_Applicable(isApplicable: true, typeof(IEvent), 10)]
        public async Task PublishAsync_InvokesMiddlewares<TRequest, TResponse, TMiddlewareRequest, TMiddlewareResponse>(TRequest request, TResponse response, TMiddlewareRequest middlewareRequest, TMiddlewareResponse middlewareResponse, int middlewareCount)
            where TRequest : IEvent
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            for (var i = 0; i < middlewareCount; i++)
            {
                var middleware = new MockInvocationMiddleware<TMiddlewareRequest, TMiddlewareResponse>();
                AddMiddleware(middleware);
            }

            // Act
            await Mediator.PublishAsync(request!, CancellationToken);

            // Assert
            foreach (var middleware in Middlewares.Cast<MockInvocationMiddleware<TMiddlewareRequest, TMiddlewareResponse>>())
            {
                middleware.Mock.Verify(m => m.HandleAsync(It.IsAny<IInvocationContext<TMiddlewareRequest, TMiddlewareResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Once);
            }
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponseMediatorRequestResponse_Applicable(isApplicable: false, typeof(IEvent))]
        public async Task PublishAsync_DoesNotInvokeUnrelatedMiddleware<TRequest, TResponse, TMiddlewareRequest, TMiddlewareResponse>(TRequest request, TResponse response, TMiddlewareRequest middlewareRequest, TMiddlewareResponse middlewareResponse)
            where TRequest : IEvent
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            var middleware = new MockInvocationMiddleware<TMiddlewareRequest, TMiddlewareResponse>();
            AddMiddleware(middleware);

            // Act
            await Mediator.PublishAsync(request!, CancellationToken);

            // Assert
            middleware.Mock.Verify(m => m.HandleAsync(It.IsAny<IInvocationContext<TMiddlewareRequest, TMiddlewareResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Event]
        public void PublishAsync_Throws_WhenHandlerThrows<TRequest, TResponse>(TRequest request, TResponse response)
            where TRequest : IEvent
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            handler.Mock
                .Setup(m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()))
                .Throws(new Exception("sample exception"));
            AddHandler(handler);

            // Act / Assert
            Assert.ThrowsAsync<Exception>(() => Mediator.PublishAsync(request!, CancellationToken));
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Event]
        public void PublishAsync_Throws_WhenMiddlewareThrows<TRequest, TResponse>(TRequest request, TResponse response)
            where TRequest : IEvent
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            var middleware = new MockInvocationMiddleware<TRequest, TResponse>();
            middleware.Mock
                .Setup(m => m.HandleAsync(It.IsAny<IInvocationContext<TRequest, TResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
                .Throws(new Exception("sample exception"));
            AddMiddleware(middleware);

            // Act / Assert
            Assert.ThrowsAsync<Exception>(() => Mediator.PublishAsync(request!, CancellationToken));
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Command]
        public async Task InvokeAndCaptureAsync_InvokesHandler<TRequest, TResponse>(TRequest request, TResponse response)
            where TRequest : ICommand
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            // Act
            _ = await Mediator.InvokeAndCaptureAsync(request!, CancellationToken);

            // Assert
            handler.Mock.Verify(m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Command]
        public async Task InvokeAndCaptureAsync_DoesNotInvokeUnrelatedHandler<TRequest, TResponse>(TRequest request, TResponse response)
            where TRequest : ICommand
        {
            // Arrange
            var expectedHandler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(expectedHandler);

            var unrelatedHandler = new MockInvocationHandler<MediatorTestData.Unrelated, MediatorTestData.Unrelated>();
            AddHandler(unrelatedHandler);

            // Act
            _ = await Mediator.InvokeAndCaptureAsync(request!, CancellationToken);

            // Assert
            unrelatedHandler.Mock.Verify(m => m.HandleAsync(It.IsAny<MediatorTestData.Unrelated>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponseMediatorRequestResponse_Applicable(isApplicable: true, typeof(ICommand), 1)]
        [TestCaseSource_RequestResponseMediatorRequestResponse_Applicable(isApplicable: true, typeof(ICommand), 10)]
        public async Task InvokeAndCaptureAsync_InvokesMiddlewares<TRequest, TResponse, TMiddlewareRequest, TMiddlewareResponse>(TRequest request, TResponse response, TMiddlewareRequest middlewareRequest, TMiddlewareResponse middlewareResponse, int middlewareCount)
            where TRequest : ICommand
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            for (var i = 0; i < middlewareCount; i++)
            {
                var middleware = new MockInvocationMiddleware<TMiddlewareRequest, TMiddlewareResponse>();
                AddMiddleware(middleware);
            }

            // Act
            _ = await Mediator.InvokeAndCaptureAsync(request!, CancellationToken);

            // Assert
            foreach (var middleware in Middlewares.Cast<MockInvocationMiddleware<TMiddlewareRequest, TMiddlewareResponse>>())
            {
                middleware.Mock.Verify(m => m.HandleAsync(It.IsAny<IInvocationContext<TMiddlewareRequest, TMiddlewareResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Once);
            }
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponseMediatorRequestResponse_Applicable(isApplicable: false, typeof(ICommand))]
        public async Task InvokeAndCaptureAsync_DoesNotInvokeUnrelatedMiddleware<TRequest, TResponse, TMiddlewareRequest, TMiddlewareResponse>(TRequest request, TResponse response, TMiddlewareRequest middlewareRequest, TMiddlewareResponse middlewareResponse)
            where TRequest : ICommand
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            var middleware = new MockInvocationMiddleware<TMiddlewareRequest, TMiddlewareResponse>();
            AddMiddleware(middleware);

            // Act
            _ = await Mediator.InvokeAndCaptureAsync(request!, CancellationToken);

            // Assert
            middleware.Mock.Verify(m => m.HandleAsync(It.IsAny<IInvocationContext<TMiddlewareRequest, TMiddlewareResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Command]
        public async Task InvokeAndCaptureAsync_ContextIsNotNull<TRequest, TResponse>(TRequest request, TResponse response)
            where TRequest : ICommand
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            // Act
            var context = await Mediator.InvokeAndCaptureAsync(request!, CancellationToken);

            // Assert
            Assert.That(context, Is.Not.Null);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Command]
        public void InvokeAndCaptureAsync_DoesNotThrow_WhenHandlerThrows<TRequest, TResponse>(TRequest request, TResponse response)
            where TRequest : ICommand
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            handler.Mock
                .Setup(m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()))
                .Throws(new Exception("sample exception"));
            AddHandler(handler);

            // Act / Assert
            Assert.DoesNotThrowAsync(() => Mediator.InvokeAndCaptureAsync(request!, CancellationToken));
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Command]
        public async Task InvokeAndCaptureAsync_ContextHasError_WhenHandlerThrows<TRequest, TResponse>(TRequest request, TResponse response)
            where TRequest : ICommand
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            handler.Mock
                .Setup(m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()))
                .Throws(new Exception("sample exception"));
            AddHandler(handler);

            // Act
            var context = await Mediator.InvokeAndCaptureAsync(request!, CancellationToken);

            // Assert
            Assert.That(context.HasError, Is.True);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Command]
        public void InvokeAndCaptureAsync_DoesNotThrow_WhenMiddlewareThrows<TRequest, TResponse>(TRequest request, TResponse response)
            where TRequest : ICommand
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            var middleware = new MockInvocationMiddleware<TRequest, TResponse>();
            middleware.Mock
                .Setup(m => m.HandleAsync(It.IsAny<IInvocationContext<TRequest, TResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
                .Throws(new Exception("sample exception"));
            AddMiddleware(middleware);

            // Act / Assert
            Assert.DoesNotThrowAsync(() => Mediator.InvokeAndCaptureAsync(request!, CancellationToken));
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Command]
        public async Task InvokeAndCaptureAsync_ContextHasError_WhenMiddlewareThrows<TRequest, TResponse>(TRequest request, TResponse response)
            where TRequest : ICommand
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            var middleware = new MockInvocationMiddleware<TRequest, TResponse>();
            middleware.Mock
                .Setup(m => m.HandleAsync(It.IsAny<IInvocationContext<TRequest, TResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
                .Throws(new Exception("sample exception"));
            AddMiddleware(middleware);

            // Act
            var context = await Mediator.InvokeAndCaptureAsync(request!, CancellationToken);

            // Assert
            Assert.That(context.HasError, Is.True);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Command]
        public async Task InvokeAsync_InvokesHandler<TRequest, TResponse>(TRequest request, TResponse response)
            where TRequest : ICommand
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            // Act
            await Mediator.InvokeAsync(request!, CancellationToken);

            // Assert
            handler.Mock.Verify(m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Command]
        public async Task InvokeAsync_DoesNotInvokeUnrelatedHandler<TRequest, TResponse>(TRequest request, TResponse response)
            where TRequest : ICommand
        {
            // Arrange
            var expectedHandler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(expectedHandler);

            var unrelatedHandler = new MockInvocationHandler<MediatorTestData.Unrelated, MediatorTestData.Unrelated>();
            AddHandler(unrelatedHandler);

            // Act
            await Mediator.InvokeAsync(request!, CancellationToken);

            // Assert
            unrelatedHandler.Mock.Verify(m => m.HandleAsync(It.IsAny<MediatorTestData.Unrelated>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponseMediatorRequestResponse_Applicable(isApplicable: true, typeof(ICommand), 1)]
        [TestCaseSource_RequestResponseMediatorRequestResponse_Applicable(isApplicable: true, typeof(ICommand), 10)]
        public async Task InvokeAsync_InvokesMiddlewares<TRequest, TResponse, TMiddlewareRequest, TMiddlewareResponse>(TRequest request, TResponse response, TMiddlewareRequest middlewareRequest, TMiddlewareResponse middlewareResponse, int middlewareCount)
            where TRequest : ICommand
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            for (var i = 0; i < middlewareCount; i++)
            {
                var middleware = new MockInvocationMiddleware<TMiddlewareRequest, TMiddlewareResponse>();
                AddMiddleware(middleware);
            }

            // Act
            await Mediator.InvokeAsync(request!, CancellationToken);

            // Assert
            foreach (var middleware in Middlewares.Cast<MockInvocationMiddleware<TMiddlewareRequest, TMiddlewareResponse>>())
            {
                middleware.Mock.Verify(m => m.HandleAsync(It.IsAny<IInvocationContext<TMiddlewareRequest, TMiddlewareResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Once);
            }
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponseMediatorRequestResponse_Applicable(isApplicable: false, typeof(ICommand))]
        public async Task InvokeAsync_DoesNotInvokeUnrelatedMiddleware<TRequest, TResponse, TMiddlewareRequest, TMiddlewareResponse>(TRequest request, TResponse response, TMiddlewareRequest middlewareRequest, TMiddlewareResponse middlewareResponse)
            where TRequest : ICommand
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            var middleware = new MockInvocationMiddleware<TMiddlewareRequest, TMiddlewareResponse>();
            AddMiddleware(middleware);

            // Act
            await Mediator.InvokeAsync(request!, CancellationToken);

            // Assert
            middleware.Mock.Verify(m => m.HandleAsync(It.IsAny<IInvocationContext<TMiddlewareRequest, TMiddlewareResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Command]
        public void InvokeAsync_Throws_WhenHandlerThrows<TRequest, TResponse>(TRequest request, TResponse response)
            where TRequest : ICommand
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            handler.Mock
                .Setup(m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()))
                .Throws(new Exception("sample exception"));
            AddHandler(handler);

            // Act / Assert
            Assert.ThrowsAsync<Exception>(() => Mediator.InvokeAsync(request!, CancellationToken));
        }

        [Test]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_Command]
        public void InvokeAsync_Throws_WhenMiddlewareThrows<TRequest, TResponse>(TRequest request, TResponse response)
            where TRequest : ICommand
        {
            // Arrange
            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            var middleware = new MockInvocationMiddleware<TRequest, TResponse>();
            middleware.Mock
                .Setup(m => m.HandleAsync(It.IsAny<IInvocationContext<TRequest, TResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
                .Throws(new Exception("sample exception"));
            AddMiddleware(middleware);

            // Act / Assert
            Assert.ThrowsAsync<Exception>(() => Mediator.InvokeAsync(request!, CancellationToken));
        }
    }
}
