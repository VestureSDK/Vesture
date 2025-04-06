using Moq;
using Vesture.Mediator.Invocation;

namespace Vesture.Mediator.Mocks.Invocation
{
    /// <summary>
    /// Defines a mock <see cref="IInvocationHandler{TRequest, TResponse}"/> contract.
    /// </summary>
    /// <typeparam name="TRequest">The request type.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    public class MockInvocationHandler<TRequest, TResponse>
        : IInvocationHandler<TRequest, TResponse>
    {
        /// <summary>
        /// The <see cref="Mock{T}"/> instance.
        /// </summary>
        public Mock<IInvocationHandler<TRequest, TResponse>> Mock { get; } = new();

        private IInvocationHandler<TRequest, TResponse> _inner => Mock.Object;

        /// <summary>
        /// The handler's response if not overriden by a mock setup.
        /// </summary>
        public TResponse Response { get; set; } = default!;

        /// <summary>
        /// Initializes a new <see cref="MockInvocationHandler{TRequest, TResponse}"/> instance.
        /// </summary>
        public MockInvocationHandler()
        {
            Mock.Setup(m => m.HandleAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()))
                .Returns(() => Task.FromResult(Response!));
        }

        /// <inheritdoc/>
        public Task<TResponse> HandleAsync(
            TRequest request,
            CancellationToken cancellationToken = default
        ) => _inner.HandleAsync(request, cancellationToken);
    }
}
