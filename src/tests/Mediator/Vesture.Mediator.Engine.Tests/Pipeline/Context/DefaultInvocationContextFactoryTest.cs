using Vesture.Mediator.Engine.Pipeline.Context;
using Vesture.Testing.Annotations;

namespace Vesture.Mediator.Engine.Tests.Pipeline.Context
{
    [ImplementationTest]
    public class DefaultInvocationContextFactoryTest
        : InvocationContextFactoryConformanceTestBase<DefaultInvocationContextFactory>
    {
        protected override DefaultInvocationContextFactory CreateFactory() => new();
    }
}
