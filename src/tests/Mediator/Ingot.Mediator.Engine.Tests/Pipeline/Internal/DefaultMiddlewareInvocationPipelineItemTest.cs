using Ingot.Mediator.Abstractions.Tests.Data.Annotations.Commands;
using Ingot.Mediator.Engine.Mocks.Pipeline.Resolvers;
using Ingot.Mediator.Engine.Pipeline.Internal;
using Ingot.Mediator.Engine.Tests.Pipeline.Internal._TestBases;
using Ingot.Mediator.Invocation;
using Ingot.Testing.Annotations;

namespace Ingot.Mediator.Engine.Tests.Pipeline.Internal
{
    [ImplementationTest]
    [TestFixtureSource_RequestResponse_All]
    public class DefaultMiddlewareInvocationPipelineItemTest<TRequest, TResponse>
        : EngineMiddlewareInvocationPipelineItemTestBase<
            TRequest,
            TResponse,
            DefaultMiddlewareInvocationPipelineItem<TRequest, TResponse>
        >
    {
        public DefaultMiddlewareInvocationPipelineItemTest(TRequest request, TResponse response)
            : base(request) { }

        protected override DefaultMiddlewareInvocationPipelineItem<
            TRequest,
            TResponse
        > CreateMiddlewareItem(int order) =>
            new(order, typeof(IInvocationMiddleware<TRequest, TResponse>), Resolver);

        protected override IMiddlewareInvocationPipelineItem CreateItemForMiddlewareSignature<
            TContractRequest,
            TContractResponse
        >()
        {
            var resolver =
                new MockInvocationComponentResolver<
                    IInvocationMiddleware<TContractRequest, TContractResponse>
                >();
            return new DefaultMiddlewareInvocationPipelineItem<TContractRequest, TContractResponse>(
                order: 0,
                typeof(IInvocationMiddleware<TContractRequest, TContractResponse>),
                resolver
            );
        }
    }
}
