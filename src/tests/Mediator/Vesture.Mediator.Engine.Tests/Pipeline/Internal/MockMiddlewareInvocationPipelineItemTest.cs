using Vesture.Mediator.Abstractions.Tests.Data.Annotations.Commands;
using Vesture.Mediator.Engine.Mocks.Pipeline.Internal;
using Vesture.Mediator.Engine.Pipeline.Internal;
using Vesture.Mediator.Engine.Tests.Pipeline.Internal._TestBases;
using Vesture.Testing.Annotations;

namespace Vesture.Mediator.Engine.Tests.Pipeline.Internal
{
    [MockTest]
    [TestFixtureSource_RequestResponse_All]
    public class MockMiddlewareInvocationPipelineItemTest<TRequest, TResponse>
        : MiddlewareInvocationPipelineItemConformanceTestBase<
            TRequest,
            TResponse,
            MockMiddlewareInvocationPipelineItem<TRequest, TResponse>
        >
    {
        public MockMiddlewareInvocationPipelineItemTest(TRequest request, TResponse response)
            : base(request) { }

        protected override MockMiddlewareInvocationPipelineItem<
            TRequest,
            TResponse
        > CreateMiddlewareItem(int order) => new() { Order = order, Middleware = Middleware };

        protected override IMiddlewareInvocationPipelineItem CreateItemForMiddlewareSignature<
            TContractRequest,
            TContractResponse
        >()
        {
            return new MockMiddlewareInvocationPipelineItem<TContractRequest, TContractResponse>();
        }
    }
}
