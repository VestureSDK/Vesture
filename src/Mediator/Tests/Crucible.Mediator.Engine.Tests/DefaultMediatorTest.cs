using Crucible.Mediator.Abstractions.Tests.Bases;
using Crucible.Mediator.Abstractions.Tests.Internal;
using Crucible.Mediator.Engine.Pipeline.Internal;
using Crucible.Mediator.Engine.Tests.Pipeline.Internal.Mocks;
using Crucible.Mediator.Engine.Tests.Pipeline.Mocks;
using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Engine.Tests
{
    [ImplementationTest]
    public class DefaultMediatorTest : MediatorConformanceTestBase<DefaultMediator>
    {
        protected ICollection<IMiddlewareInvocationPipelineItem> MiddlewareItems { get; } = [];

        protected Dictionary<(Type RequestType, Type ResponseType), MockInvocationPipeline> Pipelines { get; } = new Dictionary<(Type RequestType, Type ResponseType), MockInvocationPipeline>();

        protected override DefaultMediator CreateMediator()
        {
            foreach (var pipeline in Pipelines.Values)
            {
                pipeline.Middlewares = MiddlewareItems;
            }

            return new(Pipelines.Values);
        }

        protected MockInvocationPipeline<TRequest, TResponse> GetOrCreatePipeline<TRequest, TResponse>()
        {
            var pipelineKey = (typeof(TRequest), typeof(TResponse));
            if (!(Pipelines.TryGetValue(pipelineKey, out var p) && p is MockInvocationPipeline<TRequest, TResponse> pipeline))
            {
                pipeline = new MockInvocationPipeline<TRequest, TResponse>();
                Pipelines[pipelineKey] = pipeline;
            }

            return pipeline;
        }

        protected override void RegisterHandler<TRequest, TResponse>(IInvocationHandler<TRequest, TResponse> handler)
        {
            var pipeline = GetOrCreatePipeline<TRequest, TResponse>();
            pipeline.Handlers.Add(handler);
        }

        protected override void RegisterMiddleware<TRequest, TResponse>(IInvocationMiddleware<TRequest, TResponse> middleware)
        {
            var item = new MockMiddlewareInvocationPipelineItem<TRequest, TResponse>()
            {
                Middleware = middleware
            };

            MiddlewareItems.Add(item);
        }
    }
}
