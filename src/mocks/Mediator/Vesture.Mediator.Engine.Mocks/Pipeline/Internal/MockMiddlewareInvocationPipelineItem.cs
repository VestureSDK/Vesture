using Moq;
using Vesture.Mediator.Engine.Pipeline.Internal;
using Vesture.Mediator.Invocation;
using Vesture.Mediator.Mocks.Invocation;

namespace Vesture.Mediator.Engine.Mocks.Pipeline.Internal
{
    public class MockMiddlewareInvocationPipelineItem<TRequest, TResponse>
        : IMiddlewareInvocationPipelineItem<TRequest, TResponse>
    {
        public Mock<IMiddlewareInvocationPipelineItem<TRequest, TResponse>> Mock { get; } = new();

        private IMiddlewareInvocationPipelineItem<TRequest, TResponse> _inner => Mock.Object;

        private readonly MockInvocationMiddleware<TRequest, TResponse> _managedMiddleware;

        private IInvocationMiddleware<TRequest, TResponse>? _middleware;

        public IInvocationMiddleware<TRequest, TResponse> Middleware
        {
            get => _middleware ?? _managedMiddleware;
            set { _middleware = value; }
        }

        public MockMiddlewareInvocationPipelineItem()
        {
            _managedMiddleware = new();

            Mock.SetupGet(m => m.MiddlewareType)
                .Returns(typeof(IInvocationMiddleware<TRequest, TResponse>));

            Mock.Setup(m => m.IsApplicable(It.IsAny<Type>()))
                .Returns<Type>(t =>
                    typeof(IInvocationContext<TRequest, TResponse>).IsAssignableFrom(t)
                );

            Mock.Setup(m =>
                    m.HandleAsync(
                        It.IsAny<IInvocationContext<TRequest, TResponse>>(),
                        It.IsAny<Func<CancellationToken, Task>>(),
                        It.IsAny<CancellationToken>()
                    )
                )
                .Returns<
                    IInvocationContext<TRequest, TResponse>,
                    Func<CancellationToken, Task>,
                    CancellationToken
                >(
                    (ctx, next, ct) =>
                    {
                        return Middleware.HandleAsync(ctx, next, ct);
                    }
                );
        }

        public Type MiddlewareType
        {
            get => _inner.MiddlewareType;
            set => Mock.SetupGet(m => m.MiddlewareType).Returns(value);
        }

        public int Order
        {
            get => _inner.Order;
            set => Mock.SetupGet(m => m.Order).Returns(value);
        }

        public bool IsApplicable(Type contextType) => _inner.IsApplicable(contextType);

        public Task HandleAsync(
            IInvocationContext<TRequest, TResponse> context,
            Func<CancellationToken, Task> next,
            CancellationToken cancellationToken
        ) => _inner.HandleAsync(context, next, cancellationToken);
    }
}
