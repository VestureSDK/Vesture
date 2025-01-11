using Crucible.Mediator.Abstractions.Tests.Internal;
using Crucible.Mediator.Abstractions.Tests.Invocation.Bases;
using Crucible.Mediator.Abstractions.Tests.Invocation.Mocks;
using Crucible.Mediator.Engine.Pipeline;
using Moq;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Bases
{
    public abstract class PrePipelineMiddlewareConformanceTestBase<TMiddleware> : InvocationMiddlewareConformanceTestBase<MockContract, MockContract, TMiddleware>
        where TMiddleware : IPrePipelineMiddleware
    {
        public PrePipelineMiddlewareConformanceTestBase()
            : base(new()) { }

        [Test]
        [ConformanceTest]
        public async Task HandleAsync_AbsorbAndCaptureException()
        {
            // Arrange
            var error = new Exception("sample exception");
            Next.Mock.Setup(m => m(It.IsAny<CancellationToken>()))
                .ThrowsAsync(error);

            // Act
            await Middleware.HandleAsync(Context, Next, CancellationToken);

            // Assert
            Assert.That(Context.Error, Is.SameAs(error), message: "Error should be absorbed and added to context");
        }
    }
}
