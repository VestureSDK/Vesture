using Ingot.Mediator.Engine.Mocks.Pipeline.Context;
using Ingot.Testing.Annotations;

namespace Ingot.Mediator.Engine.Tests.Pipeline.Context
{
    [MockTest]
    public class MockInvocationContextFactoryTest : InvocationContextFactoryConformanceTestBase<MockInvocationContextFactory>
    {
        protected override MockInvocationContextFactory CreateFactory() => new();
    }
}
