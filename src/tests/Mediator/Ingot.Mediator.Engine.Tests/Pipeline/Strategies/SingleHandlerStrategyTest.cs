using Ingot.Mediator.Abstractions.Tests.Data.Annotations.Commands;
using Ingot.Mediator.Engine.Pipeline.Strategies;
using Ingot.Testing;
using Ingot.Testing.Annotations;

namespace Ingot.Mediator.Engine.Tests.Pipeline.Strategies
{
    [ImplementationTest]
    [TestFixtureSource_RequestResponse_All]
    public class SingleHandlerStrategyTest<TRequest, TResponse>
        : EngineInvocationHandlerStrategyTestBase<
            TRequest,
            TResponse,
            SingleHandlerStrategy<TRequest, TResponse>
        >
    {
        protected NUnitTestContextMsLogger<
            SingleHandlerStrategy<TRequest, TResponse>
        > Logger { get; } = new();

        public SingleHandlerStrategyTest(TRequest request, TResponse response)
            : base(request, response) { }

        protected override SingleHandlerStrategy<TRequest, TResponse> CreateStrategy() =>
            new(Logger, Resolver);

        [Test]
        public void Ctor_ArgumentNullException_IfLoggerIsNull()
        {
            // Arrange
            // No arrange required

            // Act / Assert
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>(
                () => new SingleHandlerStrategy<TRequest, TResponse>(null, Resolver)
            );
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Test]
        public void Ctor_ArgumentNullException_IfResolverIsNull()
        {
            // Arrange
            // No arrange required

            // Act / Assert
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>(
                () => new SingleHandlerStrategy<TRequest, TResponse>(Logger, null)
            );
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }
    }
}
