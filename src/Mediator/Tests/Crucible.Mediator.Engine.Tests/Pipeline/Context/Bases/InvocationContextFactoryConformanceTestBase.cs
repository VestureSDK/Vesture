using Crucible.Mediator.Abstractions.Tests.Data.Annotations.Requests;
using Crucible.Mediator.Abstractions.Tests.Internal;
using Crucible.Mediator.Engine.Pipeline.Context;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Context.Bases
{
    public abstract class InvocationContextFactoryConformanceTestBase<TFactory>
        where TFactory : IInvocationContextFactory
    {
        protected Lazy<TFactory> FactoryInitializer { get; }

        protected TFactory Factory => FactoryInitializer.Value;

        protected abstract TFactory CreateFactory();

        public InvocationContextFactoryConformanceTestBase()
        {
#pragma warning disable IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
            FactoryInitializer = new Lazy<TFactory>(() => CreateFactory());
#pragma warning restore IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
        }

        [Theory]
        [ConformanceTest]
        [TestCaseSource_RequestResponse_All]
        public virtual void CreateContextForRequest_ReturnedContext_IsNotNull<TRequest, TResponse>(TRequest request, TResponse response)
        {
            // Arrange
            // No arrange required

            // Act
            var context = Factory.CreateContextForRequest<TRequest, TResponse>(request!);

            // Assert
            Assert.That(context, Is.Not.Null);
        }
    }
}
