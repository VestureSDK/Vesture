using Crucible.Mediator.Abstractions.Tests.Invocation.Mocks;
using Crucible.Mediator.Engine.Pipeline.Internal;
using Crucible.Mediator.Engine.Tests.Pipeline.Resolvers.Mocks;
using Crucible.Mediator.Invocation;
using Moq;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Internal.Bases
{
    public abstract class EngineMiddlewareInvocationPipelineItemTestBase<TRequest, TResponse, TMiddlewareItem> :
        MiddlewareInvocationPipelineItemConformanceTestBase<TRequest, TResponse, TMiddlewareItem>
        where TMiddlewareItem : IMiddlewareInvocationPipelineItem<TRequest, TResponse>
    {
        protected MockInvocationComponentResolver<IInvocationMiddleware<TRequest, TResponse>> Resolver { get; } = new();

        protected EngineMiddlewareInvocationPipelineItemTestBase(TRequest defaultRequest)
            : base(defaultRequest)
        {
            Resolver = new() { Component = Middleware };
        }

        [Theory]
        [TestCaseGenericNoParams</*Contract:*/ MockContract, MockContract>(Description = "Applicable contract")]
        [TestCaseGenericNoParams</*Contract:*/ MockUnmarked, MockUnmarked>(Description = "Unapplicable contract")]
        public void IsApplicable_DoesNotResolveComponent_WhenIsApplicableReturnsTrue<TContractRequest, TContractResponse>()
        {
            // Arrange
            var contextType = typeof(IInvocationContext<TContractRequest, TContractResponse>);

            // Act
            var isApplicable = MiddlewareItem.IsApplicable(contextType);

            // Assert
            Resolver.Mock.Verify(m => m.ResolveComponent(), Times.Never, failMessage: "ResolveComponent should not be called outside of HandleAsync");
        }

        [Theory]
        [TestCase(1, Description = "Call HandleAsync once")]
        [TestCase(5, Description = "Call HandleAsync multiple times")]
        public async Task HandleAsync_ResolvesTheComponentFromTheResolverEverytime(int iterationCount)
        {
            // Arrange
            // No arrange required

            // Act
            for (var i = 0; i < iterationCount; i++)
            {
                await MiddlewareItem.HandleAsync(Context, Next, CancellationToken);
            }

            // Assert
            Resolver.Mock.Verify(m => m.ResolveComponent(), Times.Exactly(iterationCount), failMessage: "ResolveComponent should not be called everytime HandleAsync is invoked");
        }
    }
}
