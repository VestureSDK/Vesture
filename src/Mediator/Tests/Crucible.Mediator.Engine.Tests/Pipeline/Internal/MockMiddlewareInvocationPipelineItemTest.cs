using Crucible.Mediator.Abstractions.Tests.Data.Annotations.Commands;
using Crucible.Mediator.Engine.Mocks.Pipeline.Internal;
using Crucible.Mediator.Engine.Pipeline.Internal;
using Crucible.Mediator.Engine.Tests.Pipeline.Internal._TestBases;
using Crucible.Testing.Annotations;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Internal
{
    [MockTest]
    [TestFixtureSource_RequestResponse_All]
    public class MockMiddlewareInvocationPipelineItemTest<TRequest, TResponse> : MiddlewareInvocationPipelineItemConformanceTestBase<TRequest, TResponse, MockMiddlewareInvocationPipelineItem<TRequest, TResponse>>
    {
        public MockMiddlewareInvocationPipelineItemTest(TRequest request, TResponse response)
            : base(request) { }

        protected override MockMiddlewareInvocationPipelineItem<TRequest, TResponse> CreateMiddlewareItem(int order) =>
            new()
            {
                Order = order,
                Middleware = Middleware
            };

        protected override IMiddlewareInvocationPipelineItem CreateItemForMiddlewareSignature<TContractRequest, TContractResponse>()
        {
            return new MockMiddlewareInvocationPipelineItem<TContractRequest, TContractResponse>();
        }
    }
}
