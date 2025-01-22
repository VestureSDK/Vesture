using Ingot.Mediator.Engine.Pipeline.Resolvers;
using Ingot.Testing.Annotations;
using Moq;

namespace Ingot.Mediator.Engine.Tests.Pipeline.Resolvers
{
    [ImplementationTest]
    public class TransientInvocationComponentResolverTest : InvocationComponentResolverConformanceTestBase<object, TransientInvocationComponentResolver<object>>
    {
        public Mock<Func<object>> Factory { get; }

        public TransientInvocationComponentResolverTest()
        {
            Factory = new();
            Factory.Setup(m => m()).Returns(() => new object());
        }

        protected override TransientInvocationComponentResolver<object> CreateInvocationComponentResolver() => new(Factory.Object);

        [Test]
        public void ResolveComponent_AreNotTheSame_WithStraightFactory()
        {
            // Arrange
            // no arrange required

            // Act
            var component1 = Resolver.ResolveComponent();
            var component2 = Resolver.ResolveComponent();

            // Assert
            Assert.That(component1, Is.Not.SameAs(component2));
        }

        [Test]
        public void ResolveComponent_AreTheSameEverytime_WithSingletonFactory()
        {
            // Arrange
            var component = new object();
            Factory.Setup(m => m()).Returns(component);

            // Act
            var component1 = Resolver.ResolveComponent();
            var component2 = Resolver.ResolveComponent();

            // Assert
            Assert.That(component1, Is.SameAs(component2));
        }

        [Test]
        [TestCase(2)]
        [TestCase(10)]
        public void ResolveComponent_FactoryIsCalledEverytime(int iterationCount)
        {
            // Arrange
            // No arrange required

            // Act
            for (var i = 0; i < iterationCount; i++)
            {
                _ = Resolver.ResolveComponent();
            }

            // Assert
            Factory.Verify(m => m(), Times.Exactly(iterationCount));
        }
    }
}
