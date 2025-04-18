﻿using Microsoft.Extensions.Logging;
using Vesture.Mediator.Engine.Pipeline.Context;
using Vesture.Mediator.Engine.Pipeline.Resolvers;

namespace Vesture.Mediator.Engine.Pipeline.Internal.NoOp
{
    /// <summary>
    /// Default implementation of <see cref="INoOpInvocationPipelineResolver"/>.
    /// </summary>
    /// <seealso cref="INoOpInvocationPipelineResolver"/>
    public class DefaultNoOpInvocationPipelineResolver : INoOpInvocationPipelineResolver
    {
        private readonly ILoggerFactory _loggerFactory;

        private readonly IInvocationContextFactory _contextFactory;

        private readonly IInvocationComponentResolver<IPrePipelineMiddleware> _preInvocationPipelineMiddlewareResolver;

        private readonly IEnumerable<IMiddlewareInvocationPipelineItem> _middlewares;

        private readonly IInvocationComponentResolver<IPreHandlerMiddleware> _preHandlerMiddlewareResolver;

        private readonly INoOpInvocationHandlerStrategyResolver _handlerStrategyResolver;

        /// <summary>
        /// Initializes a new <see cref="DefaultNoOpInvocationPipelineResolver"/> instance.
        /// </summary>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/> instance.</param>
        /// <param name="contextFactory">The <see cref="IInvocationContextFactory"/> instance.</param>
        /// <param name="preInvocationPipelineMiddlewareResolver">The <see cref="IInvocationComponentResolver{TComponent}"/> instance.</param>
        /// <param name="middlewares">The <see cref="IMiddlewareInvocationPipelineItem"/> instances.</param>
        /// <param name="preHandlerMiddlewareResolver">The <see cref="IInvocationComponentResolver{TComponent}"/> instance.</param>
        /// <param name="handlerStrategyResolver">The <see cref="INoOpInvocationHandlerStrategyResolver"/> instance.</param>
        /// <exception cref="ArgumentNullException"><paramref name="contextFactory"/> is <see langword="null" /> or <paramref name="preInvocationPipelineMiddlewareResolver"/> is <see langword="null" /> or <paramref name="middlewares"/> is <see langword="null" /> or <paramref name="preHandlerMiddlewareResolver"/> is <see langword="null" /> or <paramref name="handlerStrategyResolver"/> is <see langword="null" />.</exception>
        public DefaultNoOpInvocationPipelineResolver(
            ILoggerFactory loggerFactory,
            IInvocationContextFactory contextFactory,
            IInvocationComponentResolver<IPrePipelineMiddleware> preInvocationPipelineMiddlewareResolver,
            IEnumerable<IMiddlewareInvocationPipelineItem> middlewares,
            IInvocationComponentResolver<IPreHandlerMiddleware> preHandlerMiddlewareResolver,
            INoOpInvocationHandlerStrategyResolver handlerStrategyResolver
        )
        {
            if (contextFactory is null)
            {
                throw new ArgumentNullException(nameof(contextFactory));
            }
            if (preInvocationPipelineMiddlewareResolver is null)
            {
                throw new ArgumentNullException(nameof(preInvocationPipelineMiddlewareResolver));
            }
            if (middlewares is null)
            {
                throw new ArgumentNullException(nameof(middlewares));
            }
            if (preHandlerMiddlewareResolver is null)
            {
                throw new ArgumentNullException(nameof(preHandlerMiddlewareResolver));
            }
            if (handlerStrategyResolver is null)
            {
                throw new ArgumentNullException(nameof(handlerStrategyResolver));
            }

            _loggerFactory = loggerFactory;
            _contextFactory = contextFactory;
            _preInvocationPipelineMiddlewareResolver = preInvocationPipelineMiddlewareResolver;
            _middlewares = middlewares;
            _preHandlerMiddlewareResolver = preHandlerMiddlewareResolver;
            _handlerStrategyResolver = handlerStrategyResolver;
        }

        /// <inheritdoc />
        public IInvocationPipeline<TResponse> ResolveNoOpInvocationPipeline<TResponse>()
        {
            var logger = _loggerFactory.CreateLogger<
                DefaultInvocationPipeline<object, TResponse>
            >();
            var handlerStrategy =
                _handlerStrategyResolver.ResolveNoOpInvocationHandlerStrategy<TResponse>();
            return new DefaultInvocationPipeline<object, TResponse>(
                logger,
                _contextFactory,
                _preInvocationPipelineMiddlewareResolver,
                _middlewares,
                _preHandlerMiddlewareResolver,
                handlerStrategy
            );
        }
    }
}
