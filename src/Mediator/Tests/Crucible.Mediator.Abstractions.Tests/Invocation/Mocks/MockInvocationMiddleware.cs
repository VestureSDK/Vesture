using Crucible.Mediator.Invocation;
using Moq;

namespace Crucible.Mediator.Abstractions.Tests.Invocation.Mocks
{
    public class MockInvocationMiddleware<TRequest, TResponse> : IInvocationMiddleware<TRequest, TResponse>
    {
        public Mock<IInvocationMiddleware<TRequest, TResponse>> Mock { get; } = new Mock<IInvocationMiddleware<TRequest, TResponse>>();

        private IInvocationMiddleware<TRequest, TResponse> _inner => Mock.Object;

        public MockInvocationMiddleware()
        {
            Mock.Setup(m => m.HandleAsync(It.IsAny<IInvocationContext<TRequest, TResponse>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
                .Returns<IInvocationContext<TRequest, TResponse>, Func<CancellationToken, Task>, CancellationToken>((context, next, cancellationtoken) =>
                {
                    return next(cancellationtoken);
                });
        }

        public Task HandleAsync(IInvocationContext<TRequest, TResponse> context, Func<CancellationToken, Task> next, CancellationToken cancellationToken) => _inner.HandleAsync(context, next, cancellationToken);
    }
}
