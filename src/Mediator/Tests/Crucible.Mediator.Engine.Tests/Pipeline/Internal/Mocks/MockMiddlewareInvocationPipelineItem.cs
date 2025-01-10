using Crucible.Mediator.Engine.Pipeline.Internal;
using Crucible.Mediator.Engine.Pipeline.Resolvers;
using Crucible.Mediator.Engine.Tests.Pipeline.Resolvers.Mocks;
using Crucible.Mediator.Invocation;
using Moq;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Internal.Mocks
{
    public class MockMiddlewareInvocationPipelineItem<TRequest, TResponse> : IMiddlewareInvocationPipelineItem<TRequest, TResponse>
    {
        public Mock<IMiddlewareInvocationPipelineItem<TRequest, TResponse>> Mock { get; } = new();

        private IMiddlewareInvocationPipelineItem<TRequest, TResponse> _inner => Mock.Object;

        public MockMiddlewareInvocationPipelineItem()
        {
            Mock.Setup(m => m.IsApplicable(It.IsAny<Type>()))
                .Returns<Type>(t => typeof(IInvocationContext<TRequest, TResponse>).IsAssignableFrom(t));
        }

        public MockMiddlewareInvocationPipelineItem(IInvocationMiddleware<TRequest, TResponse> middleware)
            : this(new MockInvocationComponentResolver<IInvocationMiddleware<TRequest, TResponse>>(middleware)) { }

        public MockMiddlewareInvocationPipelineItem(IInvocationComponentResolver<IInvocationMiddleware<TRequest, TResponse>> resolver)
            : this()
        {
            Mock.Setup(m => m.HandleAsync(It.IsAny<IInvocationContext<TRequest, TResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
                .Returns<IInvocationContext<TRequest, TResponse>, Func<CancellationToken, Task>, CancellationToken>((ctx, next, ct) =>
                {
                    var middleware = resolver.ResolveComponent();
                    return middleware.HandleAsync(ctx, next, ct);
                });
        }

        public int Order
        {
            get => _inner.Order;
            set => Mock.SetupGet(m => m.Order).Returns(value);
        }

        public bool IsApplicable(Type contextType) => _inner.IsApplicable(contextType);

        public Task HandleAsync(IInvocationContext<TRequest, TResponse> context, Func<CancellationToken, Task> next, CancellationToken cancellationToken) => _inner.HandleAsync(context, next, cancellationToken);
    }
}
