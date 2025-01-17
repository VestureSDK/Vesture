using Ingot.Mediator.Engine.Pipeline.Internal;
using Ingot.Testing.Annotations;

namespace Ingot.Mediator.Engine.Tests.Pipeline.Internal
{
    [ImplementationTest]
    public class DefaultPrePipelineAndHandlerMiddlewareTest
    {
        [Test]
        public void Instance_IsNotNull()
        {
            // Arrange
            // No arrange required

            // Act
            var instance = DefaultPrePipelineAndHandlerMiddleware.Instance;

            // Assert
            Assert.That(instance, Is.Not.Null);
        }
    }

    [ImplementationTest]
    public class DefaultPrePipelineAndHandlerMiddlewareTest_PreHandler : PreHandlerMiddlewareConformanceTestBase<DefaultPrePipelineAndHandlerMiddleware>
    {
        protected override DefaultPrePipelineAndHandlerMiddleware CreateInvocationMiddleware() => new();
    }

    [ImplementationTest]
    public class DefaultPrePipelineAndHandlerMiddlewareTest_PrePipeline : PrePipelineMiddlewareConformanceTestBase<DefaultPrePipelineAndHandlerMiddleware>
    {
        protected override DefaultPrePipelineAndHandlerMiddleware CreateInvocationMiddleware() => new();
    }
}
