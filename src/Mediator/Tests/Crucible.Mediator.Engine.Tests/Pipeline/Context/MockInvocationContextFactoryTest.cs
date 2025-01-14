using Crucible.Mediator.Engine.Mocks.Pipeline.Context;
using Crucible.Testing.Annotations;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Context
{
    [MockTest]
    public class MockInvocationContextFactoryTest : InvocationContextFactoryConformanceTestBase<MockInvocationContextFactory>
    {
        protected override MockInvocationContextFactory CreateFactory() => new();
    }
}
