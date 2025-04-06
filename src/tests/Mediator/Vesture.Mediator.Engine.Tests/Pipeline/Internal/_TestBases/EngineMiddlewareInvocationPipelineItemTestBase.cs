using Moq;
using Vesture.Mediator.Abstractions.Tests.Data.Annotations.Requests;
using Vesture.Mediator.Engine.Mocks.Pipeline.Resolvers;
using Vesture.Mediator.Engine.Pipeline.Internal;
using Vesture.Mediator.Invocation;

namespace Vesture.Mediator.Engine.Tests.Pipeline.Internal._TestBases
{
    public abstract class EngineMiddlewareInvocationPipelineItemTestBase<
        TRequest,
        TResponse,
        TMiddlewareItem
    > : MiddlewareInvocationPipelineItemConformanceTestBase<TRequest, TResponse, TMiddlewareItem>
        where TMiddlewareItem : IMiddlewareInvocationPipelineItem<TRequest, TResponse>
    {
        protected MockInvocationComponentResolver<
            IInvocationMiddleware<TRequest, TResponse>
        > Resolver { get; } = new();

        protected EngineMiddlewareInvocationPipelineItemTestBase(TRequest defaultRequest)
            : base(defaultRequest)
        {
            Resolver = new() { Component = Middleware };
        }

        [Test]
        [TestCaseSource_RequestResponse_All]
        public void IsApplicable_DoesNotResolveComponent<TContractRequest, TContractResponse>(
            TContractRequest request,
            TContractResponse response
        )
        {
            // Arrange
            var contextType = typeof(IInvocationContext<TContractRequest, TContractResponse>);

            // Act
            var isApplicable = MiddlewareItem.IsApplicable(contextType);

            // Assert
            Resolver.Mock.Verify(
                m => m.ResolveComponent(),
                Times.Never,
                failMessage: "ResolveComponent should not be called outside of HandleAsync"
            );
        }

        [Test]
        [TestCase(1, Description = "Call HandleAsync once")]
        [TestCase(5, Description = "Call HandleAsync multiple times")]
        public async Task HandleAsync_ResolvesTheComponentFromTheResolverEverytime(
            int iterationCount
        )
        {
            // Arrange
            // No arrange required

            // Act
            for (var i = 0; i < iterationCount; i++)
            {
                await MiddlewareItem.HandleAsync(Context, Next, CancellationToken);
            }

            // Assert
            Resolver.Mock.Verify(
                m => m.ResolveComponent(),
                Times.Exactly(iterationCount),
                failMessage: "ResolveComponent should not be called everytime HandleAsync is invoked"
            );
        }
    }
}
