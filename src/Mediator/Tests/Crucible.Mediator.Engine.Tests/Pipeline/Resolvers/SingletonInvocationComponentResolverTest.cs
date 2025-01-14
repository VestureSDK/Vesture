using Crucible.Mediator.Engine.Pipeline.Resolvers;
using Crucible.Testing.Annotations;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Resolvers
{
    [ImplementationTest]
    public class SingletonInvocationComponentResolverTest : InvocationComponentResolverConformanceTestBase<object, SingletonInvocationComponentResolver<object>>
    {
        public object SingletonInstance { get; set; } = new object();

        protected override SingletonInvocationComponentResolver<object> CreateInvocationComponentResolver() => new(SingletonInstance);

        [Test]
        public void ResolveComponent_IsSingletonInstance()
        {
            // Arrange
            // No arrange required

            // Act
            var component = Resolver.ResolveComponent();

            // Assert
            Assert.That(component, Is.SameAs(SingletonInstance));
        }

        [Test]
        public void ResolveComponent_AreTheSameEverytime()
        {
            // Arrange
            // No arrange required

            // Act
            var component1 = Resolver.ResolveComponent();
            var component2 = Resolver.ResolveComponent();

            // Assert
            Assert.That(component1, Is.SameAs(component2));
        }
    }
}
