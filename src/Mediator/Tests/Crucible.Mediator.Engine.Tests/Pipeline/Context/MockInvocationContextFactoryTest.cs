using Crucible.Mediator.Abstractions.Tests.Internal;
using Crucible.Mediator.Engine.Tests.Pipeline.Context.Bases;
using Crucible.Mediator.Engine.Tests.Pipeline.Context.Mocks;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Context
{
    [MockTest]
    public class MockInvocationContextFactoryTest : InvocationContextFactoryConformanceTestBase<MockInvocationContextFactory>
    {
        protected override MockInvocationContextFactory CreateFactory() => new();

        //[Theory]
        //[ConformanceTest]
        //[TestCaseSource_RequestResponse_Unmarked]
        //public override void CreateContextForRequest_ReturnedContext_IsNotNull<TContractRequest, TContractResponse>(TContractRequest request, TContractResponse response)
        //{
        //    base.CreateContextForRequest_ReturnedContext_IsNotNull<TContractRequest, TContractResponse>(request, response);
        //}
    }
}
