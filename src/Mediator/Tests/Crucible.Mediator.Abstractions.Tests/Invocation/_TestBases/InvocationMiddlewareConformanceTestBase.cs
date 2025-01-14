using Crucible.Mediator.Invocation;
using Crucible.Mediator.Mocks.Invocation;
using Crucible.Testing.Annotations;
using Moq;

namespace Crucible.Mediator.Abstractions.Tests.Invocation
{
    public abstract class InvocationMiddlewareConformanceTestBase<TRequest, TResponse, TMiddleware>
        where TMiddleware : IInvocationMiddleware<TRequest, TResponse>
    {
        protected Lazy<TMiddleware> MiddlewareInitializer { get; }

        protected TMiddleware Middleware => MiddlewareInitializer.Value;

        protected MockInvocationContext<TRequest, TResponse> Context { get; }

        protected MockNext Next { get; } = new MockNext();

        protected CancellationToken CancellationToken { get; set; } = CancellationToken.None;

        public InvocationMiddlewareConformanceTestBase(TRequest defaultRequest)
        {
            Context = new() { Request = defaultRequest! };

#pragma warning disable IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
            MiddlewareInitializer = new Lazy<TMiddleware>(() => CreateInvocationMiddleware());
#pragma warning restore IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
        }

        protected abstract TMiddleware CreateInvocationMiddleware();

        [Test]
        [ConformanceTest]
        public virtual async Task HandleAsync_CallsNextItemInTheChain()
        {
            // Arrange
            // No arrange required

            // Act
            await Middleware.HandleAsync(Context, Next, CancellationToken);

            // Assert
            Next.Mock.Verify(m => m(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
