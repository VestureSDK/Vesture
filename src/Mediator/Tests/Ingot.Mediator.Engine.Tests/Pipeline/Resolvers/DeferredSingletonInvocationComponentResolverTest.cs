using Ingot.Mediator.Engine.Pipeline.Resolvers;
using Ingot.Testing.Annotations;
using Moq;

namespace Ingot.Mediator.Engine.Tests.Pipeline.Resolvers
{
    [ImplementationTest]
    public class DeferredSingletonInvocationComponentResolverTest : InvocationComponentResolverConformanceTestBase<object, DeferredSingletonInvocationComponentResolver<object>>
    {
        public object SingletonInstance { get; set; } = new object();

        public Mock<Func<object>> Factory { get; set; }

        public Lazy<object> Lazy { get; set; }

        public DeferredSingletonInvocationComponentResolverTest()
        {
            Factory = new();
            Factory.Setup(m => m()).Returns(() => SingletonInstance);
            Lazy = new Lazy<object>(() => Factory.Object());
        }

        protected override DeferredSingletonInvocationComponentResolver<object> CreateInvocationComponentResolver() => new(Lazy);

        [Test]
        public void ResolveComponent_IsSingletonInstance()
        {
            // Arrange
            SingletonInstance = new object();

            // Act
            var component = Resolver.ResolveComponent();

            // Assert
            Assert.That(component, Is.SameAs(SingletonInstance));
        }

        [Test]
        [TestCase(2)]
        [TestCase(10)]
        public void ResolveComponent_PassesThroughTheFactoryOnlyOnce(int iterationCount)
        {
            // Arrange
            // No arrange required

            // Act
            for (var i = 0; i < iterationCount; i++)
            {
                _ = Resolver.ResolveComponent();
            }

            // Assert
            Factory.Verify(m => m(), Times.Once);
        }
    }
}
