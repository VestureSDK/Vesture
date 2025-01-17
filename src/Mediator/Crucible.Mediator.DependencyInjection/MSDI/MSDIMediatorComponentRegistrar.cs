using System.Diagnostics.CodeAnalysis;
using Crucible.Mediator.Engine.Pipeline;
using Crucible.Mediator.Engine.Pipeline.Context;
using Crucible.Mediator.Engine.Pipeline.Internal;
using Crucible.Mediator.Engine.Pipeline.Internal.NoOp;
using Crucible.Mediator.Engine.Pipeline.Resolvers;
using Crucible.Mediator.Engine.Pipeline.Strategies;
using Crucible.Mediator.Events;
using Crucible.Mediator.Invocation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Crucible.Mediator.DependencyInjection.MSDI
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
            ArgumentNullException.ThrowIfNull(services, nameof(services));

            _services = services;

            _services.TryAddSingleton<IPreHandlerMiddleware>(DefaultPrePipelineAndHandlerMiddleware.Instance);
            _services.TryAddSingleton<IInvocationComponentResolver<IPreHandlerMiddleware>, SingletonInvocationComponentResolver<IPreHandlerMiddleware>>();

            _services.TryAddSingleton<IPrePipelineMiddleware>(DefaultPrePipelineAndHandlerMiddleware.Instance);
            _services.TryAddSingleton<IInvocationComponentResolver<IPrePipelineMiddleware>, SingletonInvocationComponentResolver<IPrePipelineMiddleware>>();

            _services.TryAddSingleton<IMediator, Engine.DefaultMediator>();
            _services.TryAddSingleton<IInvocationContextFactory, DefaultInvocationContextFactory>();

            _services.TryAddSingleton<INoOpInvocationHandlerStrategyResolver, DefaultNoOpInvocationHandlerStrategyResolver>();
            _services.TryAddSingleton<INoOpInvocationPipelineResolver, DefaultNoOpInvocationPipelineResolver>();
            _services.TryAddSingleton<INoOpInvocationHandlerStrategyResolver, DefaultNoOpInvocationHandlerStrategyResolver>();
        }

        private IServiceCollection _services { get; }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException"><paramref name="middleware"/> is <see langword="null" />.</exception>
        public void RegisterMiddleware<TRequest, TResponse>(IInvocationMiddleware<TRequest, TResponse> middleware, int? order = null)
        {
            ArgumentNullException.ThrowIfNull(middleware, nameof(middleware));

            var middlewareType = middleware.GetType();
            if (!_services.Any(sd => sd.ServiceType == middlewareType))
            {
                var finalOrder = order ?? 0;
                _services.AddSingleton<IMiddlewareInvocationPipelineItem>(sp =>
                {
                    var accessor = new SingletonInvocationComponentResolver<IInvocationMiddleware<TRequest, TResponse>>(middleware);
                    return new DefaultMiddlewareInvocationPipelineItem<TRequest, TResponse>(order ?? 0, accessor);
                });
            }
        }

        /// <inheritdoc />
        public void RegisterMiddleware<TRequest, TResponse, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMiddleware>(int? order = null, bool singleton = false)
            where TMiddleware : class, IInvocationMiddleware<TRequest, TResponse>
        {
            if (!_services.Any(sd => sd.ServiceType == typeof(TMiddleware)))
            {
                var finalOrder = order ?? 0;
                if (singleton)
                {
                    _services.AddSingleton<TMiddleware>();
                    _services.AddSingleton<IInvocationComponentResolver<TMiddleware>, SingletonInvocationComponentResolver<TMiddleware>>();
                }
                else
                {
                    _services.AddTransient<TMiddleware>();
                    _services.AddSingleton<IInvocationComponentResolver<TMiddleware>>(sp => new TransientInvocationComponentResolver<TMiddleware>(() => sp.GetRequiredService<TMiddleware>()));
                }

                _services.AddSingleton<IMiddlewareInvocationPipelineItem>(sp =>
                {
                    var accessor = sp.GetRequiredService<IInvocationComponentResolver<TMiddleware>>();
                    return new DefaultMiddlewareInvocationPipelineItem<TRequest, TResponse>(finalOrder, accessor);
                });
            }
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException"><paramref name="handler"/> is <see langword="null" />.</exception>
        public void RegisterHandler<TRequest, TResponse>(IInvocationHandler<TRequest, TResponse> handler)
        {
            ArgumentNullException.ThrowIfNull(handler, nameof(handler));

            _services.AddSingleton<IInvocationComponentResolver<IInvocationHandler<TRequest, TResponse>>>(new SingletonInvocationComponentResolver<IInvocationHandler<TRequest, TResponse>>(handler));
            TryAddInvocationPipeline<TRequest, TResponse>();
        }

        /// <inheritdoc />
        public void RegisterHandler<TRequest, TResponse, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THandler>(bool singleton = false)
            where THandler : class, IInvocationHandler<TRequest, TResponse>
        {
            if (singleton)
            {
                _services.TryAddSingleton<THandler>();
                _services.AddSingleton<IInvocationComponentResolver<IInvocationHandler<TRequest, TResponse>>, SingletonInvocationComponentResolver<THandler>>();
            }
            else
            {
                _services.TryAddTransient<THandler>();
                _services.AddSingleton<IInvocationComponentResolver<IInvocationHandler<TRequest, TResponse>>>((sp) => new TransientInvocationComponentResolver<IInvocationHandler<TRequest, TResponse>>(() => sp.GetRequiredService<THandler>()));
            }


            TryAddInvocationPipeline<TRequest, TResponse>();
        }

        /// <inheritdoc />
        public void RegisterHandlerStrategy<TRequest, TResponse, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TStrategy>()
            where TStrategy : class, IInvocationHandlerStrategy<TRequest, TResponse>
        {
            _services.AddSingleton<IInvocationHandlerStrategy<TRequest, TResponse>, TStrategy>();
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException"><paramref name="strategy"/> is <see langword="null" />.</exception>
        public void RegisterHandlerStrategy<TRequest, TResponse>(IInvocationHandlerStrategy<TRequest, TResponse> strategy)
        {
            ArgumentNullException.ThrowIfNull(strategy, nameof(strategy));

            _services.AddSingleton(strategy);
        }

        private void TryAddInvocationPipeline<TRequest, TResponse>()
        {
            if (!_services.Any(sd => sd.ImplementationType == typeof(DefaultInvocationPipeline<TRequest, TResponse>)))
            {
                _services.AddSingleton<IInvocationPipeline, DefaultInvocationPipeline<TRequest, TResponse>>();
            }

            if (typeof(TResponse) == EventResponse.Type)
            {
                _services.TryAddSingleton<IInvocationHandlerStrategy<TRequest, TResponse>, ParallelHandlersStrategy<TRequest, TResponse>>();
            }
            else
            {
                _services.TryAddSingleton<IInvocationHandlerStrategy<TRequest, TResponse>, SingleHandlerStrategy<TRequest, TResponse>>();
            }
        }
    }
}
