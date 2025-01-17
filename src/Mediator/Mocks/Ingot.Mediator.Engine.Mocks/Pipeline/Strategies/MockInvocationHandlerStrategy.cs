using Ingot.Mediator.Engine.Pipeline.Strategies;
using Ingot.Mediator.Invocation;
using Ingot.Mediator.Mocks.Invocation;
using Moq;

namespace Ingot.Mediator.Engine.Mocks.Pipeline.Strategies
{
    public class MockInvocationHandlerStrategy<TRequest, TResponse> : IInvocationHandlerStrategy<TRequest, TResponse>
    {
        public Mock<IInvocationHandlerStrategy<TRequest, TResponse>> Mock { get; } = new();

        private IInvocationHandlerStrategy<TRequest, TResponse> _inner => Mock.Object;

        private TResponse _response = default!;

        public TResponse Response
        {
            get => _response;
            set
            {
                _response = value;
                ManagedHandler.Response = value;
                ManagedOtherHandler.Response = value;
            }
        }

        public MockInvocationHandler<TRequest, TResponse> ManagedHandler { get; }

        private IInvocationHandler<TRequest, TResponse>? _handler;

        public IInvocationHandler<TRequest, TResponse> Handler
        {
            get => _handler ?? ManagedHandler;
            set
            {
                _handler = value;
                _managedHandlers[1] = value ?? ManagedOtherHandler;
            }
        }

        public MockInvocationHandler<TRequest, TResponse> ManagedOtherHandler { get; }

        private IInvocationHandler<TRequest, TResponse>? _otherHandler;

        public IInvocationHandler<TRequest, TResponse> OtherHandler
        {
            get => _otherHandler ?? ManagedOtherHandler;
            set
            {
                _otherHandler = value;
                _managedHandlers[0] = value ?? ManagedOtherHandler;
            }
        }

        private List<IInvocationHandler<TRequest, TResponse>> _managedHandlers;

        private ICollection<IInvocationHandler<TRequest, TResponse>>? _handlers;

        public ICollection<IInvocationHandler<TRequest, TResponse>> Handlers
        {
            get => _handlers ?? _managedHandlers;
            set
            {
                _handlers = value;
                _managedHandlers = value?.ToList() ?? [OtherHandler, Handler];
            }
        }

        public MockInvocationHandlerStrategy()
        {
            ManagedHandler = new MockInvocationHandler<TRequest, TResponse>();
            ManagedOtherHandler = new MockInvocationHandler<TRequest, TResponse>();
            _managedHandlers = [ManagedOtherHandler, ManagedHandler];

            Mock
                .Setup(m => m.HandleAsync(It.IsAny<IInvocationContext<TRequest, TResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
                .Returns<IInvocationContext<TRequest, TResponse>, Func<CancellationToken, Task>, CancellationToken>(async (ctx, next, ct) =>
                {
                    foreach (var handler in Handlers)
                    {
                        var response = await handler.HandleAsync(ctx.Request, ct);
                        ctx.SetResponse(response);
                    }
                });
        }

        public Task HandleAsync(IInvocationContext<TRequest, TResponse> context, Func<CancellationToken, Task> next, CancellationToken cancellationToken) => _inner.HandleAsync(context, next, cancellationToken);
    }
}
