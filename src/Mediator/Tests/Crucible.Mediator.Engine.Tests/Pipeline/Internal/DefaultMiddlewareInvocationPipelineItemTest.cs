using Crucible.Mediator.Abstractions.Tests.Invocation.Mocks;
using Crucible.Mediator.Engine.Pipeline.Internal;
using Crucible.Mediator.Engine.Tests.Pipeline.Internal.Bases;
using Crucible.Mediator.Engine.Tests.Pipeline.Resolvers.Mocks;
using Crucible.Mediator.Invocation;
using Moq;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Internal
{
    public class DefaultMiddlewareInvocationPipelineItemTest : MiddlewareInvocationPipelineItemTestBase<DefaultMiddlewareInvocationPipelineItem<MockContract, MockContract>>
    {
        protected MockInvocationContext<MockContract, MockContract> InvocationContext { get; } = new(new());

        protected MockInvocationMiddleware<MockContract, MockContract> InvocationMiddleware { get; } = new();

        protected MockInvocationComponentResolver<IInvocationMiddleware<MockContract, MockContract>> InvocationComponentResolver { get; } = new();

        protected Func<CancellationToken, Task> Next { get; set; } = (ct) => Task.CompletedTask;

        protected CancellationToken CancellationToken { get; set; } = CancellationToken.None;

        public DefaultMiddlewareInvocationPipelineItemTest()
        {
            InvocationComponentResolver = new(InvocationMiddleware);
        }

        protected override DefaultMiddlewareInvocationPipelineItem<MockContract, MockContract> CreateMiddlewareItem(int order) => new DefaultMiddlewareInvocationPipelineItem<MockContract, MockContract>(order, InvocationComponentResolver);

        protected override IMiddlewareInvocationPipelineItem CreateItemForMiddlewareSignature<TRequest, TResponse>()
        {
            var resolver = new MockInvocationComponentResolver<IInvocationMiddleware<TRequest, TResponse>>();
            return new DefaultMiddlewareInvocationPipelineItem<TRequest, TResponse>(order: 0, resolver);
        }

        [Theory]
        [CustomTestCase</*Contract:*/ MockContract, MockContract>(Description = "Applicable contract")]
        [CustomTestCase</*Contract:*/ MockUnmarked, MockUnmarked>(Description = "Unapplicable contract")]
        public void IsApplicable_DoesNotResolveComponent_WhenIsApplicableReturnsTrue<TContractRequest, TContractResponse>()
        {
            // Arrange
            var contextType = typeof(IInvocationContext<TContractRequest, TContractResponse>);

            // Act
            var isApplicable = MiddlewareItem.IsApplicable(contextType);

            // Assert
            InvocationComponentResolver.Mock.Verify(m => m.ResolveComponent(), Times.Never, failMessage: "ResolveComponent should not be called outside of HandleAsync");
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
                await MiddlewareItem.HandleAsync(InvocationContext, Next, CancellationToken);
            }

            // Assert
            InvocationComponentResolver.Mock.Verify(m => m.ResolveComponent(), Times.Exactly(iterationCount), failMessage: "ResolveComponent should not be called everytime HandleAsync is invoked");
        }

        [Test]
        public async Task HandleAsync_ResolvedMiddlewareIsInvoked()
        {
            // Arrange
            // No arrange required

            // Act
            await MiddlewareItem.HandleAsync(InvocationContext, Next, CancellationToken);

            // Assert
            InvocationMiddleware.Mock.Verify(m => m.HandleAsync(It.IsAny<IInvocationContext<MockContract, MockContract>>(), It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Once, failMessage: "HandleAsync should called resolved middleware");
        }

        [Test]
        public async Task HandleAsync_NextItemInChainIsInvoked()
        {
            // Arrange
            var nextExecuted = false;
            Next = (ct) =>
            {
                nextExecuted = true;
                return Task.CompletedTask;
            };

            // Act
            await MiddlewareItem.HandleAsync(InvocationContext, Next, CancellationToken);

            // Assert
            Assert.That(nextExecuted, Is.True, message: "Next item in the chain should be called");
        }
    }
}
