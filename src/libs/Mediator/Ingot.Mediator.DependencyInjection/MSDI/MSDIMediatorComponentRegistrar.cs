using System.Diagnostics.CodeAnalysis;
using Ingot.Mediator.Engine.Pipeline;
using Ingot.Mediator.Engine.Pipeline.Context;
using Ingot.Mediator.Engine.Pipeline.Internal;
using Ingot.Mediator.Engine.Pipeline.Internal.NoOp;
using Ingot.Mediator.Engine.Pipeline.Resolvers;
using Ingot.Mediator.Engine.Pipeline.Strategies;
using Ingot.Mediator.Events;
using Ingot.Mediator.Invocation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ingot.Mediator.DependencyInjection.MSDI
{
    /// <summary>
    /// <para>
    /// The <see cref="MSDIMediatorComponentRegistrar"/> is the
    /// <c>Microsoft.Extensions.DependencyInjection</c>
    /// implementation of <see cref="IMediatorComponentRegistrar"/>.
    /// </para>
    /// <para>
    /// It allows to register in the <see cref="IServiceCollection"/>
    /// (an subsequently the <see cref="IServiceProvider"/>) the
    /// <see cref="IInvocationPipeline{TResult}"/> components.
    /// </para>
    /// </summary>
    /// <seealso cref="IMediatorComponentRegistrar"/>
    public class MSDIMediatorComponentRegistrar : IMediatorComponentRegistrar
    {
        /// <summary>
        /// Initializes a new <see cref="MSDIMediatorComponentRegistrar"/> instance.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> instance.</param>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null" />.</exception>
        public MSDIMediatorComponentRegistrar(IServiceCollection services)
        {
            if (services is null) { throw new ArgumentNullException(nameof(services)); }

            _services = services;

            _services.AddLogging();

            _services.TryAddSingleton<
                IPreHandlerMiddleware,
                DefaultPrePipelineAndHandlerMiddleware
            >();
            _services.TryAddSingleton<
                IInvocationComponentResolver<IPreHandlerMiddleware>,
                SingletonInvocationComponentResolver<IPreHandlerMiddleware>
            >();

            _services.TryAddSingleton<
                IPrePipelineMiddleware,
                DefaultPrePipelineAndHandlerMiddleware
            >();
            _services.TryAddSingleton<
                IInvocationComponentResolver<IPrePipelineMiddleware>,
                SingletonInvocationComponentResolver<IPrePipelineMiddleware>
            >();

            _services.TryAddSingleton<IMediator, Engine.DefaultMediator>();
            _services.TryAddSingleton<IInvocationContextFactory, DefaultInvocationContextFactory>();

            _services.TryAddSingleton<
                INoOpInvocationHandlerStrategyResolver,
                DefaultNoOpInvocationHandlerStrategyResolver
            >();
            _services.TryAddSingleton<
                INoOpInvocationPipelineResolver,
                DefaultNoOpInvocationPipelineResolver
            >();
            _services.TryAddSingleton<
                INoOpInvocationHandlerStrategyResolver,
                DefaultNoOpInvocationHandlerStrategyResolver
            >();
        }

        private IServiceCollection _services { get; }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException"><paramref name="middleware"/> is <see langword="null" />.</exception>
        public void RegisterMiddleware<TRequest, TResponse>(
            IInvocationMiddleware<TRequest, TResponse> middleware,
            int? order = null
        )
        {
            if (middleware is null) { throw new ArgumentNullException(nameof(middleware)); }

            var middlewareType = middleware.GetType();
            if (!_services.Any(sd => sd.ServiceType == middlewareType))
            {
                var finalOrder = order ?? 0;
                _services.AddSingleton<IMiddlewareInvocationPipelineItem>(sp =>
                {
                    var accessor = new SingletonInvocationComponentResolver<
                        IInvocationMiddleware<TRequest, TResponse>
                    >(middleware);
                    return new DefaultMiddlewareInvocationPipelineItem<TRequest, TResponse>(
                        order ?? 0,
                        middleware.GetType(),
                        accessor
                    );
                });
            }
        }

        /// <inheritdoc />
        public void RegisterMiddleware<
            TRequest,
            TResponse,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
                TMiddleware
        >(int? order = null, bool singleton = false)
            where TMiddleware : class, IInvocationMiddleware<TRequest, TResponse>
        {
            if (!_services.Any(sd => sd.ServiceType == typeof(TMiddleware)))
            {
                var finalOrder = order ?? 0;
                if (singleton)
                {
                    _services.AddSingleton<TMiddleware>();
                    _services.AddSingleton<
                        IInvocationComponentResolver<TMiddleware>,
                        SingletonInvocationComponentResolver<TMiddleware>
                    >();
                }
                else
                {
                    _services.AddTransient<TMiddleware>();
                    _services.AddSingleton<IInvocationComponentResolver<TMiddleware>>(
                        sp => new TransientInvocationComponentResolver<TMiddleware>(
                            () => sp.GetRequiredService<TMiddleware>()
                        )
                    );
                }

                _services.AddSingleton<IMiddlewareInvocationPipelineItem>(sp =>
                {
                    var accessor = sp.GetRequiredService<
                        IInvocationComponentResolver<TMiddleware>
                    >();
                    return new DefaultMiddlewareInvocationPipelineItem<TRequest, TResponse>(
                        finalOrder,
                        typeof(TMiddleware),
                        accessor
                    );
                });
            }
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException"><paramref name="handler"/> is <see langword="null" />.</exception>
        public void RegisterHandler<TRequest, TResponse>(
            IInvocationHandler<TRequest, TResponse> handler
        )
        {
            if (handler is null) { throw new ArgumentNullException(nameof(handler)); }

            _services.AddSingleton<
                IInvocationComponentResolver<IInvocationHandler<TRequest, TResponse>>
            >(
                new SingletonInvocationComponentResolver<IInvocationHandler<TRequest, TResponse>>(
                    handler
                )
            );
            TryAddInvocationPipeline<TRequest, TResponse>();
        }

        /// <inheritdoc />
        public void RegisterHandler<
            TRequest,
            TResponse,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THandler
        >(bool singleton = false)
            where THandler : class, IInvocationHandler<TRequest, TResponse>
        {
            if (singleton)
            {
                _services.TryAddSingleton<THandler>();
                _services.AddSingleton<
                    IInvocationComponentResolver<IInvocationHandler<TRequest, TResponse>>,
                    SingletonInvocationComponentResolver<THandler>
                >();
            }
            else
            {
                _services.TryAddTransient<THandler>();
                _services.AddSingleton<
                    IInvocationComponentResolver<IInvocationHandler<TRequest, TResponse>>
                >(
                    (sp) =>
                        new TransientInvocationComponentResolver<
                            IInvocationHandler<TRequest, TResponse>
                        >(() => sp.GetRequiredService<THandler>())
                );
            }

            TryAddInvocationPipeline<TRequest, TResponse>();
        }

        /// <inheritdoc />
        public void RegisterHandlerStrategy<
            TRequest,
            TResponse,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
                TStrategy
        >()
            where TStrategy : class, IInvocationHandlerStrategy<TRequest, TResponse>
        {
            _services.AddSingleton<IInvocationHandlerStrategy<TRequest, TResponse>, TStrategy>();
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException"><paramref name="strategy"/> is <see langword="null" />.</exception>
        public void RegisterHandlerStrategy<TRequest, TResponse>(
            IInvocationHandlerStrategy<TRequest, TResponse> strategy
        )
        {
            if (strategy is null) { throw new ArgumentNullException(nameof(strategy)); }

            _services.AddSingleton(strategy);
        }

        private void TryAddInvocationPipeline<TRequest, TResponse>()
        {
            if (
                !_services.Any(sd =>
                    sd.ImplementationType == typeof(DefaultInvocationPipeline<TRequest, TResponse>)
                )
            )
            {
                _services.AddSingleton<
                    IInvocationPipeline,
                    DefaultInvocationPipeline<TRequest, TResponse>
                >();
            }

            if (typeof(TResponse) == EventResponse.Type)
            {
                _services.TryAddSingleton<
                    IInvocationHandlerStrategy<TRequest, TResponse>,
                    ParallelHandlersStrategy<TRequest, TResponse>
                >();
            }
            else
            {
                _services.TryAddSingleton<
                    IInvocationHandlerStrategy<TRequest, TResponse>,
                    SingleHandlerStrategy<TRequest, TResponse>
                >();
            }
        }
    }
}
