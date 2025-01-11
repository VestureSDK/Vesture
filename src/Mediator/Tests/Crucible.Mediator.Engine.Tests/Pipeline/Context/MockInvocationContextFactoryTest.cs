using Crucible.Mediator.Engine.Tests.Pipeline.Context.Bases;
using Crucible.Mediator.Engine.Tests.Pipeline.Context.Mocks;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Context
{
    public class MockInvocationContextFactoryTest : InvocationContextFactoryTestBase<MockInvocationContextFactory>
    {
        protected override MockInvocationContextFactory CreateFactory() => new();
    }
}
