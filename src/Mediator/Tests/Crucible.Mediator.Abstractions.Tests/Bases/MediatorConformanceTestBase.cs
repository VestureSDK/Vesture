using Crucible.Mediator.Abstractions.Tests.Data;
using Crucible.Mediator.Abstractions.Tests.Internal;
using Crucible.Mediator.Abstractions.Tests.Invocation.Mocks;
using Crucible.Mediator.Abstractions.Tests.Requests.Mocks;
using Crucible.Mediator.Invocation;
using Moq;
using Any = object;

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

        [Theory]
        [ConformanceTest]
        [TestCaseGenericNoParams<MockUnmarked, MockUnmarked>(1)]
        [TestCaseGenericNoParams<MockUnmarked, MockUnmarked>(10)]
        public async Task HandleAndCaptureAsync_InvokesHandlers<TRequest, TResponse>(int handlerCount)
            where TRequest : new()
        {
            // Arrange
            var request = new TRequest();

            for (var i = 0; i < handlerCount; i++)
            {
                var handler = new MockInvocationHandler<TRequest, TResponse>();
                AddHandler(handler);
            }

            // Act
            _ = await Mediator.HandleAndCaptureAsync<TResponse>(request, CancellationToken);

            // Assert
            foreach (var handler in Handlers.Cast<MockInvocationHandler<TRequest, TResponse>>())
            {
                handler.Mock.Verify(m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()), Times.Once);
            }
        }

        [Theory]
        [ConformanceTest]
        [TestCaseGenericNoParams<MockUnmarked, MockUnmarked>]
        public async Task HandleAndCaptureAsync_DoesNotInvokeUnrelatedHandler<TRequest, TResponse>()
            where TRequest : new()
        {
            // Arrange
            var request = new TRequest();

            var expectedHandler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(expectedHandler);

            var unrelatedHandler = new MockInvocationHandler<Unrelated, Unrelated>();
            AddHandler(unrelatedHandler);

            // Act
            _ = await Mediator.HandleAndCaptureAsync<TResponse>(request, CancellationToken);

            // Assert
            unrelatedHandler.Mock.Verify(m => m.HandleAsync(It.IsAny<Unrelated>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [ConformanceTest]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockRequest, MockResponse, /*Middleware:*/ Any, Any>(1)]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockRequest, MockResponse, /*Middleware:*/ Any, Any>(10)]
        public async Task HandleAndCaptureAsync_InvokesMiddlewares<TRequest, TResponse, TMiddlewareRequest, TMiddlewareResponse>(int middlewareCount)
            where TRequest : new()
        {
            // Arrange
            var request = new TRequest();

            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            for (var i = 0; i < middlewareCount; i++)
            {
                var middleware = new MockInvocationMiddleware<TMiddlewareRequest, TMiddlewareResponse>();
                AddMiddleware(middleware);
            }

            // Act
            _ = await Mediator.HandleAndCaptureAsync<TResponse>(request, CancellationToken);

            // Assert
            foreach (var middleware in Middlewares.Cast<MockInvocationMiddleware<TMiddlewareRequest, TMiddlewareResponse>>())
            {
                middleware.Mock.Verify(m => m.HandleAsync(It.IsAny<IInvocationContext<TMiddlewareRequest, TMiddlewareResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Once);
            }
        }

        [Theory]
        [ConformanceTest]
        [TestCaseGenericNoParamsAttribute</*Contract:*/ MockRequest, MockResponse, /*Middleware:*/ Unrelated, Unrelated>()]
        public async Task HandleAndCaptureAsync_DoesNotInvokeUnrelatedMiddleware<TRequest, TResponse, TMiddlewareRequest, TMiddlewareResponse>()
            where TRequest : new()
        {
            // Arrange
            var request = new TRequest();

            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            var middleware = new MockInvocationMiddleware<TMiddlewareRequest, TMiddlewareResponse>();
            AddMiddleware(middleware);

            // Act
            _ = await Mediator.HandleAndCaptureAsync<TResponse>(request, CancellationToken);

            // Assert
            middleware.Mock.Verify(m => m.HandleAsync(It.IsAny<IInvocationContext<TMiddlewareRequest, TMiddlewareResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [ConformanceTest]
        // [TestCaseSource(typeof(MediatorTestData), nameof(MediatorTestData.GetCommand_Request))]
        public async Task HandleAndCaptureAsync_ContextIsNotNull<TRequest, TResponse>(TRequest _a, TResponse _b)
            where TRequest : new()
        {
            // Arrange
            var request = new TRequest();

            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            // Act
            var context = await Mediator.HandleAndCaptureAsync<TResponse>(request, CancellationToken);

            // Assert
            Assert.That(context, Is.Not.Null);
        }

        [Theory]
        [ConformanceTest]
        [TestCaseGenericNoParams<MockUnmarked, MockUnmarked>]
        public async Task HandleAndCaptureAsync_ContextContainsExpectedResponse<TRequest, TResponse>()
            where TRequest : new()
            where TResponse : new()
        {
            // Arrange
            var request = new TRequest();
            var expectedResponse = new TResponse();

            var handler = new MockInvocationHandler<TRequest, TResponse>
            {
                Response = expectedResponse
            };
            AddHandler(handler);

            // Act
            var context = await Mediator.HandleAndCaptureAsync<TResponse>(request, CancellationToken);

            // Assert
            Assert.That(context.Response, Is.SameAs(expectedResponse));
        }

        [Theory]
        [ConformanceTest]
        [TestCaseGenericNoParams<MockUnmarked, MockUnmarked>]
        public void HandleAndCaptureAsync_DoesNotThrow_WhenHandlerThrows<TRequest, TResponse>()
            where TRequest : new()
        {
            // Arrange
            var request = new TRequest();

            var handler = new MockInvocationHandler<TRequest, TResponse>();
            handler.Mock
                .Setup(m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()))
                .Throws(new Exception("sample exception"));
            AddHandler(handler);

            // Act / Assert
            Assert.DoesNotThrowAsync(() => Mediator.HandleAndCaptureAsync<TResponse>(request, CancellationToken));
        }

        [Theory]
        [ConformanceTest]
        [TestCaseGenericNoParams<MockUnmarked, MockUnmarked>]
        public async Task HandleAndCaptureAsync_ContextHasError_WhenHandlerThrows<TRequest, TResponse>()
            where TRequest : new()
        {
            // Arrange
            var request = new TRequest();

            var handler = new MockInvocationHandler<TRequest, TResponse>();
            handler.Mock
                .Setup(m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()))
                .Throws(new Exception("sample exception"));
            AddHandler(handler);

            // Act
            var context = await Mediator.HandleAndCaptureAsync<TResponse>(request, CancellationToken);

            // Assert
            Assert.That(context.HasError, Is.True);
        }

        [Theory]
        [ConformanceTest]
        [TestCaseGenericNoParams<MockUnmarked, MockUnmarked>]
        public void HandleAndCaptureAsync_DoesNotThrow_WhenMiddlewareThrows<TRequest, TResponse>()
            where TRequest : new()
        {
            // Arrange
            var request = new TRequest();

            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            var middleware = new MockInvocationMiddleware<TRequest, TResponse>();
            middleware.Mock
                .Setup(m => m.HandleAsync(It.IsAny<IInvocationContext<TRequest, TResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
                .Throws(new Exception("sample exception"));
            AddMiddleware(middleware);

            // Act / Assert
            Assert.DoesNotThrowAsync(() => Mediator.HandleAndCaptureAsync<TResponse>(request, CancellationToken));
        }

        [Theory]
        [ConformanceTest]
        [TestCaseGenericNoParams<MockUnmarked, MockUnmarked>]
        public async Task HandleAndCaptureAsync_ContextHasError_WhenMiddlewareThrows<TRequest, TResponse>()
            where TRequest : new()
        {
            // Arrange
            var request = new TRequest();

            var handler = new MockInvocationHandler<TRequest, TResponse>();
            AddHandler(handler);

            var middleware = new MockInvocationMiddleware<TRequest, TResponse>();
            middleware.Mock
                .Setup(m => m.HandleAsync(It.IsAny<IInvocationContext<TRequest, TResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
                .Throws(new Exception("sample exception"));
            AddMiddleware(middleware);

            // Act
            var context = await Mediator.HandleAndCaptureAsync<TResponse>(request, CancellationToken);

            // Assert
            Assert.That(context.HasError, Is.True);
        }
    }

    public class Unrelated
    {

    }

    public class TestData
    {
        public static object[] GetData()
        {
            var r = new object[2];
            r[0] = new object[] { new MockUnmarked(), new MockUnmarked() };
            r[1] = new object[] { new MockRequest(), new MockUnmarked() };
            return r;
        }
    }
}
