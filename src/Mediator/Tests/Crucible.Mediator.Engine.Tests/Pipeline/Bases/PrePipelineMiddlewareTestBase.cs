using Crucible.Mediator.Abstractions.Tests.Invocation.Bases;
using Crucible.Mediator.Abstractions.Tests.Invocation.Mocks;
using Crucible.Mediator.Engine.Pipeline;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Bases
{
    public abstract class PrePipelineMiddlewareTestBase<TMiddleware> : InvocationMiddlewareTestBase<MockContract, MockContract, TMiddleware>
        where TMiddleware : IPrePipelineMiddleware
    {
        public PrePipelineMiddlewareTestBase()
            : base(new()) { }

        [Test]
        public async Task HandleAsync_AbsorbAndCaptureException()
        {
            // Arrange
            var error = new Exception("sample exception");
            Next = (ct) => throw error;

            // Act
            await Middleware.HandleAsync(InvocationContext, Next, CancellationToken);

            // Assert
            Assert.That(InvocationContext.Error, Is.SameAs(error), message: "Error should be absorbed and added to context");
        }
    }
}
