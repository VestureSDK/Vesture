using Crucible.Mediator.Engine.Pipeline.Context;
using Crucible.Testing.Annotations;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Context
{
    [ImplementationTest]
    public class DefaultInvocationContextFactoryTest : InvocationContextFactoryConformanceTestBase<DefaultInvocationContextFactory>
    {
        protected override DefaultInvocationContextFactory CreateFactory() => new();
    }
}
