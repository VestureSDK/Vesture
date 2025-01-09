using Crucible.Mediator.Invocation;
using Moq;

namespace Crucible.Mediator.Abstractions.Tests.Invocation.Mocks
{
    public class MockInvocationHandler<TRequest, TResponse> : IInvocationHandler<TRequest, TResponse>
    {
        public Mock<IInvocationHandler<TRequest, TResponse>> Mock { get; } = new();

        private IInvocationHandler<TRequest, TResponse> _inner => Mock.Object;

        public TResponse Response { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public MockInvocationHandler()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        {
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
            Mock.Setup(m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(Response));
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
        }

        public Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken = default) => _inner.HandleAsync(request, cancellationToken);
    }
}
