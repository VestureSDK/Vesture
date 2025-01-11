using Crucible.Mediator.Abstractions.Tests.Invocation.Bases;
using Crucible.Mediator.Abstractions.Tests.Invocation.Mocks;
using Crucible.Mediator.Engine.Pipeline;
using Moq;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Bases
{
    public abstract class PreHandlerMiddlewareTestBase<TMiddleware> : InvocationMiddlewareConformanceTestBase<MockContract, MockContract, TMiddleware>
        where TMiddleware : IPreHandlerMiddleware
    {
        public PreHandlerMiddlewareTestBase()
            : base(new()) { }

        [Test]
        public async Task HandleAsync_AbsorbAndCaptureException()
        {
            // Arrange
            var error = new Exception("sample exception");
            Next.Mock.Setup(m => m(It.IsAny<CancellationToken>()))
                .ThrowsAsync(error);

            // Act
            await Middleware.HandleAsync(InvocationContext, Next, CancellationToken);

            // Assert
            Assert.That(InvocationContext.Error, Is.SameAs(error), message: "Error should be absorbed and added to context");
        }
    }
}
