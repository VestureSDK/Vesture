using Crucible.Mediator.Engine.Pipeline.Context;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Context.Bases
{
    public abstract class InvocationContextFactoryTestBase<TFactory>
        where TFactory : IInvocationContextFactory
    {
        protected Lazy<TFactory> FactoryInitializer { get; }

        protected TFactory Factory => FactoryInitializer.Value;

        protected abstract TFactory CreateFactory();

        public InvocationContextFactoryTestBase()
        {
#pragma warning disable IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
            FactoryInitializer = new Lazy<TFactory>(() => CreateFactory());
#pragma warning restore IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
        }

        [Test]
        public void CreateContextForRequest_ReturnedContext_IsNotNull()
        {
            // Arrange
            var request = new object();

            // Act
            var context = Factory.CreateContextForRequest<object, object>(request);

            // Assert
            Assert.That(context, Is.Not.Null, message: $"Created context should not be null");
        }
    }
}
