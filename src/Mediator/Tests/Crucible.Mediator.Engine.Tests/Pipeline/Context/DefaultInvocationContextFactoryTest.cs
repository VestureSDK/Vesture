using Crucible.Mediator.Engine.Pipeline.Context;
using Crucible.Mediator.Engine.Tests.Pipeline.Context.Bases;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Context
{
    public class DefaultInvocationContextFactoryTest : InvocationContextFactoryTestBase<DefaultInvocationContextFactory>
    {
        protected override DefaultInvocationContextFactory CreateFactory() => new DefaultInvocationContextFactory();
    }
}
