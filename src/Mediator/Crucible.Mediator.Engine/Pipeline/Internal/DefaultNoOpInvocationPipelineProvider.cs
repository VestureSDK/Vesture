using Crucible.Mediator.Engine.Pipeline.Context;
using Crucible.Mediator.Engine.Pipeline.Resolvers;
using Crucible.Mediator.Engine.Pipeline.Strategies;
using Crucible.Mediator.Invocation;
using Crucible.Mediator.Requests;
using System.Reflection.Metadata;

namespace Crucible.Mediator.Engine.Pipeline.Internal
{
    public class DefaultNoOpInvocationPipelineProvider : INoOpInvocationPipelineProvider
    {
        private readonly IInvocationContextFactory _contextFactory;

        private readonly IInvocationComponentResolver<IPrePipelineMiddleware> _preInvocationPipelineMiddlewareResolver;

        private readonly IEnumerable<IMiddlewareInvocationPipelineItem> _middlewares;

        private readonly IInvocationComponentResolver<IPreHandlerMiddleware> _preHandlerMiddlewareResolver;

        private readonly IInvocationHandlerStrategy<TRequest, TResponse> _handler;

        public DefaultNoOpInvocationPipelineProvider(
            IInvocationContextFactory contextFactory,
            IInvocationComponentResolver<IPrePipelineMiddleware> preInvocationPipelineMiddlewareResolver,
            IEnumerable<IMiddlewareInvocationPipelineItem> middlewares,
            IInvocationComponentResolver<IPreHandlerMiddleware> preHandlerMiddlewareResolver,
            IInvocationHandlerStrategy<TRequest, TResponse> handlerStrategy)
        {
            ArgumentNullException.ThrowIfNull(contextFactory, nameof(contextFactory));
            ArgumentNullException.ThrowIfNull(preInvocationPipelineMiddlewareResolver, nameof(preInvocationPipelineMiddlewareResolver));
            ArgumentNullException.ThrowIfNull(middlewares, nameof(middlewares));
            ArgumentNullException.ThrowIfNull(preHandlerMiddlewareResolver, nameof(preHandlerMiddlewareResolver));
            ArgumentNullException.ThrowIfNull(handlerStrategy, nameof(handlerStrategy));

            _contextFactory = contextFactory;
            _preInvocationPipelineMiddlewareResolver = preInvocationPipelineMiddlewareResolver;
            _middlewares = middlewares;
            _preHandlerMiddlewareResolver = preHandlerMiddlewareResolver;
            _handler = handlerStrategy;
        }

        public IInvocationPipeline<TResponse> GetNoOpInvocationPipeline<TResponse>()
        {
            return new DefaultInvocationPipeline<object, TResponse>(
                _contextFactory,
                _preInvocationPipelineMiddlewareResolver,
                _middlewares,
                _handler);
        }
    }
}
