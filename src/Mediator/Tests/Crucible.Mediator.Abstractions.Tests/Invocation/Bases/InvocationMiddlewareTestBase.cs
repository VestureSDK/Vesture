using Crucible.Mediator.Abstractions.Tests.Invocation.Mocks;
using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Abstractions.Tests.Invocation.Bases
{
    public abstract class InvocationMiddlewareTestBase<TRequest, TResponse, TMiddleware>
        where TMiddleware : IInvocationMiddleware<TRequest, TResponse>
    {
        protected Lazy<TMiddleware> MiddlewareInitializer { get; }

        protected TMiddleware Middleware => MiddlewareInitializer.Value;

        protected MockInvocationContext<TRequest, TResponse> InvocationContext { get; }

        protected Func<CancellationToken, Task> Next { get; set; } = (ct) => Task.CompletedTask;

        protected CancellationToken CancellationToken { get; set; } = CancellationToken.None;

        public InvocationMiddlewareTestBase(TRequest defaultRequest)
        {
            InvocationContext = new() { Request = defaultRequest! };

#pragma warning disable IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
            MiddlewareInitializer = new Lazy<TMiddleware>(() => CreateMiddleware());
#pragma warning restore IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
        }

        protected abstract TMiddleware CreateMiddleware();

        [Test]
        public async Task HandleAsync_CallsNextItemInTheChain()
        {
            // Arrange
            var nextExecuted = false;
            Next = (ct) =>
            {
                nextExecuted = true;
                return Task.CompletedTask;
            };

            // Act
            await Middleware.HandleAsync(InvocationContext, Next, CancellationToken);

            // Assert
            Assert.That(nextExecuted, Is.True, message: "Next item in the chain should be called");
        }
    }
}
