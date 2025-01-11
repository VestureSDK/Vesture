using Crucible.Mediator.Abstractions.Tests.Internal;
using Crucible.Mediator.Engine.Pipeline.Context;
using Crucible.Mediator.Engine.Tests.Pipeline.Context.Bases;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Context
{
    [ImplementationTest]
    public class DefaultInvocationContextFactoryTest : InvocationContextFactoryConformanceTestBase<DefaultInvocationContextFactory>
    {
        protected override DefaultInvocationContextFactory CreateFactory() => new();
    }
}
