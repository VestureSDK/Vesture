using Crucible.Mediator.Abstractions.Tests.Invocation.Mocks;
using Crucible.Mediator.Engine.Pipeline.Strategies;
using Crucible.Mediator.Invocation;
using Moq;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Strategies.Mocks
{
    public class MockInvocationHandlerStrategy<TRequest, TResponse> : IInvocationHandlerStrategy<TRequest, TResponse>
    {
        public Mock<IInvocationHandlerStrategy<TRequest, TResponse>> Mock { get; } = new();

        private IInvocationHandlerStrategy<TRequest, TResponse> _inner => Mock.Object;

        private readonly MockInvocationHandler<TRequest, TResponse> _managedHandler;

        private IInvocationHandler<TRequest, TResponse>? _handler;

        public IInvocationHandler<TRequest, TResponse> Handler 
        { 
            get => _handler ?? _managedHandler; 
            set => _handler = value;
        }

        public MockInvocationHandlerStrategy()
        {
            _managedHandler = new MockInvocationHandler<TRequest, TResponse>();
            
            Mock
                .Setup(m => m.HandleAsync(It.IsAny<IInvocationContext<TRequest, TResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
                .Returns<IInvocationContext<TRequest, TResponse>, Func<CancellationToken, Task>, CancellationToken>(async (ctx, next, ct) =>
                {
                    var response = await Handler.HandleAsync(ctx.Request, ct);
                    ctx.SetResponse(response);
                });
        }

        public Task HandleAsync(IInvocationContext<TRequest, TResponse> context, Func<CancellationToken, Task> next, CancellationToken cancellationToken) => _inner.HandleAsync(context, next, cancellationToken);
    }
}
