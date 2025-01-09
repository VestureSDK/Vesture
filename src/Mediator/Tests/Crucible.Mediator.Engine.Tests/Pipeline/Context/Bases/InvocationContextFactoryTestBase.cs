using Crucible.Mediator.Abstractions.Tests.Commands;
using Crucible.Mediator.Abstractions.Tests.Events;
using Crucible.Mediator.Abstractions.Tests.Invocation.Mocks;
using Crucible.Mediator.Abstractions.Tests.Requests;
using Crucible.Mediator.Commands;
using Crucible.Mediator.Engine.Pipeline.Context;
using Crucible.Mediator.Events;
using Crucible.Mediator.Invocation;

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

        [Theory]
        [CustomTestCase<MockRequest, MockResponse>()]
        [CustomTestCase<MockCommand, CommandResponse>()]
        [CustomTestCase<MockEvent, EventResponse>()]
        [CustomTestCase<MockUnmarked, MockResponse>()]
        [CustomTestCase<MockUnmarked, CommandResponse>()]
        [CustomTestCase<MockUnmarked, EventResponse>()]
        [CustomTestCase<MockUnmarked, NoResponse>()]
        public void CreateContextForRequest_ReturnedContext_IsNotNull<TContractRequest, TContractResponse>()
        {
            // Arrange
            var request = new MockContract();

            // Act
            var context = Factory.CreateContextForRequest<MockContract, MockContract>(request);

            // Assert
            Assert.That(context, Is.Not.Null, message: $"Created context should not be null");
        }
    }
}
