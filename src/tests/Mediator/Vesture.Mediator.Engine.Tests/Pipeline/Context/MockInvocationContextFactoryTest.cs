using Vesture.Mediator.Engine.Mocks.Pipeline.Context;
using Vesture.Testing.Annotations;

namespace Vesture.Mediator.Engine.Tests.Pipeline.Context
{
    [MockTest]
    public class MockInvocationContextFactoryTest
        : InvocationContextFactoryConformanceTestBase<MockInvocationContextFactory>
    {
        protected override MockInvocationContextFactory CreateFactory() => new();
    }
}
