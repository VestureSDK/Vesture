using Ingot.Mediator.Engine.Pipeline.Context;
using Ingot.Testing.Annotations;

namespace Ingot.Mediator.Engine.Tests.Pipeline.Context
{
    [ImplementationTest]
    public class DefaultInvocationContextFactoryTest : InvocationContextFactoryConformanceTestBase<DefaultInvocationContextFactory>
    {
        protected override DefaultInvocationContextFactory CreateFactory() => new();
    }
}
