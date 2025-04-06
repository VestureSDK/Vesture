using Moq;
using Vesture.Mediator.Invocation;

namespace Vesture.Mediator.Mocks.Invocation
{
    /// <summary>
    /// Defines a mock <see cref="IInvocationMiddleware{TRequest, TResponse}"/> contract.
    /// </summary>
    /// <typeparam name="TRequest">The request type.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    public class MockInvocationMiddleware<TRequest, TResponse>
        : IInvocationMiddleware<TRequest, TResponse>
    {
        /// <summary>
        /// The <see cref="Mock{T}"/> instance.
        /// </summary>
        public Mock<IInvocationMiddleware<TRequest, TResponse>> Mock { get; } =
            new Mock<IInvocationMiddleware<TRequest, TResponse>>();

        private IInvocationMiddleware<TRequest, TResponse> _inner => Mock.Object;

        /// <summary>
        /// Initializes a new <see cref="MockInvocationMiddleware{TRequest, TResponse}"/> instance.
        /// </summary>
        public MockInvocationMiddleware()
        {
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
                    (context, next, cancellationtoken) =>
                    {
                        return next(cancellationtoken);
                    }
                );
        }

        /// <inheritdoc/>
        public Task HandleAsync(
            IInvocationContext<TRequest, TResponse> context,
            Func<CancellationToken, Task> next,
            CancellationToken cancellationToken
        ) => _inner.HandleAsync(context, next, cancellationToken);
    }
}
