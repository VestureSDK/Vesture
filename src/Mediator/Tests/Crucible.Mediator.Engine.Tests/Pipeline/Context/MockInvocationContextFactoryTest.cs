using Crucible.Mediator.Abstractions.Tests.Invocation.Mocks;
using Crucible.Mediator.Engine.Tests.Pipeline.Context.Bases;
using Crucible.Mediator.Engine.Tests.Pipeline.Context.Mocks;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Context
{
    public class MockInvocationContextFactoryTest : InvocationContextFactoryTestBase<MockInvocationContextFactory>
    {
        protected override MockInvocationContextFactory CreateFactory() => new();

        [Theory]
        [CustomTestCase<MockContract, MockContract>()]
        public override void CreateContextForRequest_ReturnedContext_IsNotNull<TContractRequest, TContractResponse>()
        {
            base.CreateContextForRequest_ReturnedContext_IsNotNull<TContractRequest, TContractResponse>();
        }
    }
}
