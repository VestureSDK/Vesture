﻿using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Vesture.Mediator.Engine.Pipeline.Context;
using Vesture.Mediator.Engine.Pipeline.Extensions;
using Vesture.Mediator.Engine.Pipeline.Internal;
using Vesture.Mediator.Engine.Pipeline.Resolvers;
using Vesture.Mediator.Engine.Pipeline.Strategies;
using Vesture.Mediator.Invocation;

namespace Vesture.Mediator.Engine.Pipeline
{
    /// <summary>
    /// <para>
    /// The <see cref="DefaultInvocationPipeline{TRequest, TResponse}"/> provides a default implementation of <see cref="IInvocationPipeline{TResponse}"/>.
    /// </para>
    /// <para>
    /// An <see cref="IInvocationPipeline{TResponse}"/> represents the orchestrated sequence of execution
    /// that processes a specific contract through a series of <see cref="IInvocationMiddleware{TRequest, TResponse}"/>
    /// and ultimately reaches an <see cref="IInvocationHandler{TRequest, TResponse}"/>.
    /// </para>
    /// </summary>
    /// <typeparam name="TRequest">The type of contract handled by this pipeline.</typeparam>
    /// <typeparam name="TResponse"><inheritdoc cref="IInvocationPipeline{TResponse}" path="/typeparam[@name='TResponse']"/></typeparam>
    /// <inheritdoc cref="IInvocationPipeline{TResponse}"/>
    public class DefaultInvocationPipeline<TRequest, TResponse> : IInvocationPipeline<TResponse>
    {
        private readonly ILogger _logger;

        private readonly IInvocationContextFactory _contextFactory;

        private readonly IInvocationComponentResolver<IPrePipelineMiddleware> _preInvocationPipelineMiddlewareResolver;

        private readonly IEnumerable<IMiddlewareInvocationPipelineItem> _middlewares;

        private readonly IInvocationComponentResolver<IPreHandlerMiddleware> _preHandlerMiddlewareResolver;

        private readonly IInvocationHandlerStrategy<TRequest, TResponse> _handlerStrategy;

        /// <inheritdoc />
        public Type RequestType { get; } = typeof(TRequest);

        /// <inheritdoc />
        public Type ResponseType { get; } = typeof(TResponse);

        /// <summary>
        /// Initializes a new <see cref="DefaultInvocationPipeline{TRequest, TResponse}"/> instance.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger{TCategoryName}"/> instance.</param>
        /// <param name="contextFactory">The <see cref="IInvocationContextFactory"/> instance.</param>
        /// <param name="preInvocationPipelineMiddlewareResolver">The <see cref="IInvocationComponentResolver{TComponent}"/> of <see cref="IPrePipelineMiddleware"/> instance.</param>
        /// <param name="middlewares">The <see cref="IMiddlewareInvocationPipelineItem"/> instances.</param>
        /// <param name="preHandlerMiddlewareResolver">The <see cref="IInvocationComponentResolver{TComponent}"/> of <see cref="IPreHandlerMiddleware"/> instance.</param>
        /// <param name="handlerStrategy">The <see cref="IInvocationHandlerStrategy{TRequest, TResponse}"/> instance.</param>
        /// <exception cref="ArgumentNullException">
        /// <list type="bullet">
        /// <item><paramref name="logger"/> is <see langword="null" />.</item>
        /// <item><paramref name="contextFactory"/> is <see langword="null" />.</item>
        /// <item><paramref name="preInvocationPipelineMiddlewareResolver"/> is <see langword="null" />.</item>
        /// <item><paramref name="middlewares"/> is <see langword="null" />.</item>
        /// <item><paramref name="preHandlerMiddlewareResolver"/> is <see langword="null" />.</item>
        /// <item><paramref name="handlerStrategy"/> is <see langword="null" />.</item>
        /// </list>
        /// </exception>
        public DefaultInvocationPipeline(
            ILogger<DefaultInvocationPipeline<TRequest, TResponse>> logger,
            IInvocationContextFactory contextFactory,
            IInvocationComponentResolver<IPrePipelineMiddleware> preInvocationPipelineMiddlewareResolver,
            IEnumerable<IMiddlewareInvocationPipelineItem> middlewares,
            IInvocationComponentResolver<IPreHandlerMiddleware> preHandlerMiddlewareResolver,
            IInvocationHandlerStrategy<TRequest, TResponse> handlerStrategy
        )
        {
            if (contextFactory is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
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
            if (handlerStrategy is null)
            {
                throw new ArgumentNullException(nameof(handlerStrategy));
            }

            _logger = logger;
            _contextFactory = contextFactory;
            _preInvocationPipelineMiddlewareResolver = preInvocationPipelineMiddlewareResolver;
            _middlewares = middlewares;
            _preHandlerMiddlewareResolver = preHandlerMiddlewareResolver;
            _handlerStrategy = handlerStrategy;
            _chainOfResponsibility = new Lazy<
                Func<IInvocationContext<TRequest, TResponse>, CancellationToken, Task>
            >(CreateChainOfResponsibility);
        }

        private readonly Lazy<
            Func<IInvocationContext<TRequest, TResponse>, CancellationToken, Task>
        > _chainOfResponsibility;

        private Func<
            IInvocationContext<TRequest, TResponse>,
            CancellationToken,
            Task
        > CreateChainOfResponsibility()
        {
            using var activity =
                MediatorEngineDiagnostics.s_invocationPipelineActivitySource.StartActivity(
                    "Pipeline Chain Creation"
                );

            // Set the activity status as error since it will be switched
            // back to "OK" if no errors are thrown.
            activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error);

            var middlewares = new List<IMiddlewareInvocationPipelineItem>();

            var contextType = typeof(IInvocationContext<TRequest, TResponse>);
            foreach (var middleware in _middlewares.OrderBy(m => m.Order))
            {
                if (middleware.IsApplicable(contextType))
                {
                    _logger.InvocationPipelineChainMiddlewareMatches<TRequest, TResponse>(
                        middleware,
                        middlewares
                    );
                    middlewares.Add(middleware);
                }
                else
                {
                    _logger.InvocationPipelineChainMiddlewareDoesNotMatch<TRequest, TResponse>(
                        middleware
                    );
                }
            }

            Func<IInvocationContext<TRequest, TResponse>, CancellationToken, Task> chain = (
                ctx,
                ct
            ) =>
            {
                var preHandlerMiddleware = _preHandlerMiddlewareResolver.ResolveComponent();
                return preHandlerMiddleware.HandleAsync(
                    (IInvocationContext<object, object>)ctx,
                    (t) => handler(ctx, t),
                    ct
                );
            };

            // Build the chain of responsibility and return the new root func.
            for (var i = middlewares.Count - 1; i >= 0; i--)
            {
                var nextMiddleware = chain;
                var item = (IInvocationMiddleware<TRequest, TResponse>)middlewares[i];
                chain = (ctx, ct) =>
                    item.HandleAsync(ctx, (t) => nextMiddleware.Invoke(ctx, t), ct);
            }

            var next = chain;
            chain = (ctx, ct) =>
            {
                var preHandlerMiddleware =
                    _preInvocationPipelineMiddlewareResolver.ResolveComponent();
                return preHandlerMiddleware.HandleAsync(
                    (IInvocationContext<object, object>)ctx,
                    (t) => next.Invoke(ctx, t),
                    ct
                );
            };

            _logger.InvocationPipelineChainCreated<TRequest, TResponse>(middlewares);

            // Set the activity status as "OK" since no error has been thrown
            activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Ok);

            return chain!;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Task handler(IInvocationContext<TRequest, TResponse> ctx, CancellationToken ct) =>
                _handlerStrategy.HandleAsync(ctx, null, ct);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        /// <inheritdoc />
        public async Task<IInvocationContext<TResponse>> HandleAsync(
            object request,
            CancellationToken cancellationToken
        )
        {
            using var activity =
                MediatorEngineDiagnostics.s_invocationPipelineActivitySource.StartActivity(
                    "Pipeline Invocation"
                );

            _logger.InvokingPipeline<TRequest, TResponse>();

            var context = _contextFactory.CreateContextForRequest<TRequest, TResponse>(request);
            await _chainOfResponsibility.Value.Invoke(context, cancellationToken);

            _logger.InvokedPipeline<TRequest, TResponse>();

            if (activity is not null)
            {
                if (context.HasError)
                {
                    activity.SetStatus(ActivityStatusCode.Error);
                }
                else
                {
                    activity.SetStatus(ActivityStatusCode.Ok);
                }
            }

            return context;
        }
    }
}
