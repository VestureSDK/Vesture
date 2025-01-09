using Crucible.Mediator.Engine.Pipeline.Resolvers;
using Crucible.Mediator.Engine.Tests.Pipeline.Resolvers.Bases;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Resolvers
{
    public class DeferredSingletonInvocationComponentResolverTest : InvocationComponentResolverTestBase<object, DeferredSingletonInvocationComponentResolver<object>>
    {
        public object SingletonInstance { get; set; } = new object();

        public Func<object> Factory { get; set; }

        public Lazy<object> Lazy { get; set; }

        public DeferredSingletonInvocationComponentResolverTest()
        {
            Factory = () => SingletonInstance;
            Lazy = new Lazy<object>(() => Factory());
        }

        protected override DeferredSingletonInvocationComponentResolver<object> CreateResolver() => new(Lazy);

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

        [Test]
        public void ResolveComponent_PassesThroughTheFactoryOnlyOnce()
        {
            // Arrange
            var factoryExecutionCount = 0;
            Factory = () =>
            {
                factoryExecutionCount++;
                return SingletonInstance;
            };

            // Act
            _ = Resolver.ResolveComponent();
            _ = Resolver.ResolveComponent();

            // Assert
            Assert.That(factoryExecutionCount, Is.EqualTo(1), message: "Component resolved should be the same singleton instance provided and factory called only once");
        }
    }
}
