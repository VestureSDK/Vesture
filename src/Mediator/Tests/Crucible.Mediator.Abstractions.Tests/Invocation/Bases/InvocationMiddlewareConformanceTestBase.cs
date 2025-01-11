using Crucible.Mediator.Abstractions.Tests.Internal;
using Crucible.Mediator.Abstractions.Tests.Invocation.Mocks;
using Crucible.Mediator.Invocation;
using Moq;

namespace Crucible.Mediator.Abstractions.Tests.Invocation.Bases
{
    public abstract class InvocationMiddlewareConformanceTestBase<TRequest, TResponse, TMiddleware>
        where TMiddleware : IInvocationMiddleware<TRequest, TResponse>
    {
        protected Lazy<TMiddleware> MiddlewareInitializer { get; }

        protected TMiddleware Middleware => MiddlewareInitializer.Value;

        protected MockInvocationContext<TRequest, TResponse> InvocationContext { get; }

        protected MockNext Next { get; } = new MockNext();

        protected CancellationToken CancellationToken { get; set; } = CancellationToken.None;

        public InvocationMiddlewareConformanceTestBase(TRequest defaultRequest)
        {
            InvocationContext = new() { Request = defaultRequest! };

#pragma warning disable IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
            MiddlewareInitializer = new Lazy<TMiddleware>(() => CreateMiddleware());
#pragma warning restore IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
        }

        protected abstract TMiddleware CreateMiddleware();

        [Test]
        [ConformanceTest]
        public virtual async Task HandleAsync_CallsNextItemInTheChain()
        {
            // Arrange
            // No arrange required

            // Act
            await Middleware.HandleAsync(InvocationContext, Next, CancellationToken);

            // Assert
            Next.Mock.Verify(m => m(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
