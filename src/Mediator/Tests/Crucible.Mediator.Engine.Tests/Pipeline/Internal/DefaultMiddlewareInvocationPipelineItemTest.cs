using Crucible.Mediator.Abstractions.Tests.Data.Annotations.Commands;
using Crucible.Mediator.Engine.Mocks.Pipeline.Resolvers;
using Crucible.Mediator.Engine.Pipeline.Internal;
using Crucible.Mediator.Engine.Tests.Pipeline.Internal._TestBases;
using Crucible.Mediator.Invocation;
using Crucible.Testing.Annotations;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Internal
{
    [ImplementationTest]
    [TestFixtureSource_RequestResponse_All]
    public class DefaultMiddlewareInvocationPipelineItemTest<TRequest, TResponse> : EngineMiddlewareInvocationPipelineItemTestBase<TRequest, TResponse, DefaultMiddlewareInvocationPipelineItem<TRequest, TResponse>>
    {
        public DefaultMiddlewareInvocationPipelineItemTest(TRequest request, TResponse response)
            : base(request) { }

        protected override DefaultMiddlewareInvocationPipelineItem<TRequest, TResponse> CreateMiddlewareItem(int order) => new(order, Resolver);

        protected override IMiddlewareInvocationPipelineItem CreateItemForMiddlewareSignature<TContractRequest, TContractResponse>()
        {
            var resolver = new MockInvocationComponentResolver<IInvocationMiddleware<TContractRequest, TContractResponse>>();
            return new DefaultMiddlewareInvocationPipelineItem<TContractRequest, TContractResponse>(order: 0, resolver);
        }
    }
}
