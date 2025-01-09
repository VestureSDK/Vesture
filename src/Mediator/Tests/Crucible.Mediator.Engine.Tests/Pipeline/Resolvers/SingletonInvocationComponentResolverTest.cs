using Crucible.Mediator.Engine.Pipeline.Resolvers;
using Crucible.Mediator.Engine.Tests.Pipeline.Resolvers.Bases;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Resolvers
{
    public class SingletonInvocationComponentResolverTest : InvocationComponentResolverTestBase<object, SingletonInvocationComponentResolver<object>>
    {
        public object SingletonInstance { get; set; }

        protected override SingletonInvocationComponentResolver<object> CreateResolver() => new(SingletonInstance);

        [Test]
        public void ResolveComponent_IsSingletonInstance()
        {
            // Arrange
            SingletonInstance = new object();

            // Act
            var component = Resolver.ResolveComponent();

            // Assert
            Assert.That(component, Is.SameAs(SingletonInstance), message: "Component resolved should be the singleton instance provided");
        }
    }
}
