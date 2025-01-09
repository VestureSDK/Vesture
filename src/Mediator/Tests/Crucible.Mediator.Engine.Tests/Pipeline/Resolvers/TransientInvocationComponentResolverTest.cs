using Crucible.Mediator.Engine.Pipeline.Resolvers;
using Crucible.Mediator.Engine.Tests.Pipeline.Resolvers.Bases;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Resolvers
{
    public class TransientInvocationComponentResolverTest : InvocationComponentResolverTestBase<object, TransientInvocationComponentResolver<object>>
    {
        public Func<object> Factory { get; set; } = () => new object();

        protected override TransientInvocationComponentResolver<object> CreateResolver() => new(Factory);

        [Test]
        public void ResolveComponent_AreNotTheSameWithStraightFactory()
        {
            // Arrange
            // no arrange required

            // Act
            var component1 = Resolver.ResolveComponent();
            var component2 = Resolver.ResolveComponent();

            // Assert
            Assert.That(component1, Is.Not.SameAs(component2), message: "Component resolved should not be the same");
        }

        [Test]
        public void ResolveComponent_AreTheSameEverytime()
        {
            // Arrange
            var component = new object();
            Factory = () => component;

            // Act
            var component1 = Resolver.ResolveComponent();
            var component2 = Resolver.ResolveComponent();

            // Assert
            Assert.That(component1, Is.SameAs(component2), message: "Component resolved should be the same");
        }

        [Test]
        public void ResolveComponent_FactoryIsCalledEverytime()
        {
            // Arrange
            var factoryExecutionCount = 0;
            Factory = () =>
            {
                factoryExecutionCount++;
                return new object();
            };

            // Act
            _ = Resolver.ResolveComponent();
            _ = Resolver.ResolveComponent();

            // Assert
            Assert.That(factoryExecutionCount, Is.EqualTo(2), message: "Factory should be invoked every time");
        }
    }
}
