using Crucible.Mediator.Abstractions.Tests.Commands.Mocks;
using Crucible.Mediator.Abstractions.Tests.Events.Mocks;
using Crucible.Mediator.Abstractions.Tests.Internal;
using Crucible.Mediator.Abstractions.Tests.Invocation.Mocks;
using Crucible.Mediator.Abstractions.Tests.Requests.Mocks;
using Crucible.Mediator.Commands;
using Crucible.Mediator.Engine.Pipeline.Context;
using Crucible.Mediator.Events;
using Crucible.Mediator.Invocation;

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
        [TestCaseGenericNoParams<MockRequest, MockResponse>()]
        [TestCaseGenericNoParams<MockCommand, CommandResponse>()]
        [TestCaseGenericNoParams<MockEvent, EventResponse>()]
        [TestCaseGenericNoParams<MockUnmarked, MockResponse>()]
        [TestCaseGenericNoParams<MockUnmarked, CommandResponse>()]
        [TestCaseGenericNoParams<MockUnmarked, EventResponse>()]
        [TestCaseGenericNoParams<MockUnmarked, NoResponse>()]
        public virtual void CreateContextForRequest_ReturnedContext_IsNotNull<TContractRequest, TContractResponse>()
            where TContractRequest : new()
        {
            // Arrange
            var request = new TContractRequest();

            // Act
            var context = Factory.CreateContextForRequest<TContractRequest, TContractResponse>(request);

            // Assert
            Assert.That(context, Is.Not.Null);
        }
    }
}
