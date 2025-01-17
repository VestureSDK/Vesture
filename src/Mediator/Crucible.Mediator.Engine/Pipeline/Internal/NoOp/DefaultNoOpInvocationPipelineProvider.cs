using Crucible.Mediator.Engine.Pipeline.Context;
using Crucible.Mediator.Engine.Pipeline.Resolvers;

namespace Crucible.Mediator.Engine.Pipeline.Internal.NoOp
{
    public class DefaultNoOpInvocationPipelineResolver : INoOpInvocationPipelineResolver
    {
        private readonly IInvocationContextFactory _contextFactory;

        private readonly IInvocationComponentResolver<IPrePipelineMiddleware> _preInvocationPipelineMiddlewareResolver;

        private readonly IEnumerable<IMiddlewareInvocationPipelineItem> _middlewares;

        private readonly IInvocationComponentResolver<IPreHandlerMiddleware> _preHandlerMiddlewareResolver;

        private readonly INoOpInvocationHandlerStrategyResolver _handlerStrategyResolver;

        public DefaultNoOpInvocationPipelineResolver(
            IInvocationContextFactory contextFactory,
            IInvocationComponentResolver<IPrePipelineMiddleware> preInvocationPipelineMiddlewareResolver,
            IEnumerable<IMiddlewareInvocationPipelineItem> middlewares,
            IInvocationComponentResolver<IPreHandlerMiddleware> preHandlerMiddlewareResolver,
            INoOpInvocationHandlerStrategyResolver handlerStrategyResolver)
        {
            ArgumentNullException.ThrowIfNull(contextFactory, nameof(contextFactory));
            ArgumentNullException.ThrowIfNull(preInvocationPipelineMiddlewareResolver, nameof(preInvocationPipelineMiddlewareResolver));
            ArgumentNullException.ThrowIfNull(middlewares, nameof(middlewares));
            ArgumentNullException.ThrowIfNull(preHandlerMiddlewareResolver, nameof(preHandlerMiddlewareResolver));
            ArgumentNullException.ThrowIfNull(handlerStrategyResolver, nameof(handlerStrategyResolver));

            _contextFactory = contextFactory;
            _preInvocationPipelineMiddlewareResolver = preInvocationPipelineMiddlewareResolver;
            _middlewares = middlewares;
            _preHandlerMiddlewareResolver = preHandlerMiddlewareResolver;
            _handlerStrategyResolver = handlerStrategyResolver;
        }

        public IInvocationPipeline<TResponse> ResolveNoOpInvocationPipeline<TResponse>()
        {
            var handlerStrategy = _handlerStrategyResolver.ResolveNoOpInvocationHandlerStrategy<TResponse>(); ;
            return new DefaultInvocationPipeline<object, TResponse>(
                _contextFactory,
                _preInvocationPipelineMiddlewareResolver,
                _middlewares,
                _preHandlerMiddlewareResolver,
                handlerStrategy);
        }
    }
}
