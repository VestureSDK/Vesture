using Crucible.Mediator.Invocation;
using Moq;

namespace Crucible.Mediator.Abstractions.Tests.Invocation.Mocks
{
    public class MockInvocationHandler<TRequest, TResponse> : IInvocationHandler<TRequest, TResponse>
    {
        public Mock<IInvocationHandler<TRequest, TResponse>> Mock { get; } = new();

        private IInvocationHandler<TRequest, TResponse> _inner => Mock.Object;

        public TResponse Response { get; set; } = default!;

        public MockInvocationHandler()
        {
            Mock.Setup(m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>())).Returns(() => Task.FromResult(Response!));
        }

        public Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken = default) => _inner.HandleAsync(request, cancellationToken);
    }
}
