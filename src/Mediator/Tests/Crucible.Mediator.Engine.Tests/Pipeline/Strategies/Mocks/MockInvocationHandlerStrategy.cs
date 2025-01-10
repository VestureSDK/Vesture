using Crucible.Mediator.Engine.Pipeline.Resolvers;
using Crucible.Mediator.Engine.Pipeline.Strategies;
using Crucible.Mediator.Engine.Tests.Pipeline.Resolvers.Mocks;
using Crucible.Mediator.Invocation;
using Moq;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Strategies.Mocks
{
    public class MockInvocationHandlerStrategy<TRequest, TResponse> : IInvocationHandlerStrategy<TRequest, TResponse>
    {
        public Mock<IInvocationHandlerStrategy<TRequest, TResponse>> Mock { get; } = new();

        private IInvocationHandlerStrategy<TRequest, TResponse> _inner => Mock.Object;

        public MockInvocationHandlerStrategy() { }

        public MockInvocationHandlerStrategy(IInvocationHandler<TRequest, TResponse> handler)
            : this(new MockInvocationComponentResolver<IInvocationHandler<TRequest, TResponse>>(handler))
        {

        }

        public MockInvocationHandlerStrategy(IInvocationComponentResolver<IInvocationHandler<TRequest, TResponse>> resolver)
            : this()
        {
            Mock
                .Setup(m => m.HandleAsync(It.IsAny<IInvocationContext<TRequest, TResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
                .Returns<IInvocationContext<TRequest, TResponse>, Func<CancellationToken, Task>, CancellationToken>(async (ctx, next, ct) =>
                {
                    var handler = resolver.ResolveComponent();
                    var response = await handler.HandleAsync(ctx.Request, ct);
                    ctx.SetResponse(response);
                });
        }

        public Task HandleAsync(IInvocationContext<TRequest, TResponse> context, Func<CancellationToken, Task> next, CancellationToken cancellationToken) => _inner.HandleAsync(context, next, cancellationToken);
    }
}
