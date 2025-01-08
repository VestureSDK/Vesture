using System.Diagnostics.CodeAnalysis;
using Crucible.Mediator.Engine.DependencyInjection;
using Crucible.Mediator.Engine.Pipeline;
using Crucible.Mediator.Engine.Pipeline.Context;
using Crucible.Mediator.Engine.Pipeline.Internal;
using Crucible.Mediator.Engine.Pipeline.Resolvers;
using Crucible.Mediator.Engine.Pipeline.Strategies;
using Crucible.Mediator.Events;
using Crucible.Mediator.Invocation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Crucible.Mediator.DependencyInjection
{
    public class MSDIDependencyInjectionRegistrar : IDependencyInjectionRegistrar
    {
        private readonly HashSet<(Type request, Type response)> _registeredContracts = [];

        /// <summary>
        /// Initializes a new <see cref="RootMediatorBuilder"/> instance.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> instance.</param>
        public MSDIDependencyInjectionRegistrar(IServiceCollection services)
        {
            Services = services;

            Services.TryAddSingleton<IPreHandlerMiddleware>(DefaultPrePipelineAndHandlerMiddleware.Instance);
            Services.TryAddSingleton<IInvocationComponentResolver<IPreHandlerMiddleware>, SingletonInvocationComponentResolver<IPreHandlerMiddleware>>();

            Services.TryAddSingleton<IPreInvocationPipelineMiddleware>(DefaultPrePipelineAndHandlerMiddleware.Instance);
            Services.TryAddSingleton<IInvocationComponentResolver<IPreInvocationPipelineMiddleware>, SingletonInvocationComponentResolver<IPreInvocationPipelineMiddleware>>();

            Services.TryAddSingleton<IMediator, Mediator.Engine.DefaultMediator>();
            Services.TryAddSingleton<IInvocationContextFactory, DefaultInvocationContextFactory>();
        }

        /// <summary>
        /// The <see cref="IServiceCollection"/> instance.
        /// </summary>
        public IServiceCollection Services { get; }

        public void RegisterMiddleware<TRequest, TResponse>(IInvocationMiddleware<TRequest, TResponse> middleware, int? order = null)
        {
            var middlewareType = middleware.GetType();
            if (!Services.Any(sd => sd.ServiceType == middlewareType))
            {
                var finalOrder = order ?? 0;
                Services.AddSingleton<IMiddlewareInvocationPipelineItem>(sp =>
                {
                    var accessor = new SingletonInvocationComponentResolver<IInvocationMiddleware<TRequest, TResponse>>(middleware);
                    return new DefaultMiddlewareInvocationPipelineItem<TRequest, TResponse>(order ?? 0, accessor);
                });
            }
        }

        public void RegisterMiddleware<TRequest, TResponse, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMiddleware>(int? order = null, bool singleton = false)
            where TMiddleware : class, IInvocationMiddleware<TRequest, TResponse>
        {
            if (!Services.Any(sd => sd.ServiceType == typeof(TMiddleware)))
            {
                var finalOrder = order ?? 0;
                if (singleton)
                {
                    Services.AddSingleton<TMiddleware>();
                    Services.AddSingleton<IInvocationComponentResolver<TMiddleware>>(sp =>
                    {
                        var lazy = new Lazy<TMiddleware>(() => sp.GetRequiredService<TMiddleware>());
                        return new DeferredSingletonInvocationComponentResolver<TMiddleware>(lazy);
                    });
                }
                else
                {
                    Services.AddTransient<TMiddleware>();
                    Services.AddSingleton<IInvocationComponentResolver<TMiddleware>>(sp => new TransientInvocationComponentResolver<TMiddleware>(() => sp.GetRequiredService<TMiddleware>()));
                }

                Services.AddSingleton<IMiddlewareInvocationPipelineItem>(sp =>
                {
                    var accessor = sp.GetRequiredService<IInvocationComponentResolver<TMiddleware>>();
                    return new DefaultMiddlewareInvocationPipelineItem<TRequest, TResponse>(finalOrder, accessor);
                });
            }
        }

        public void RegisterHandler<TRequest, TResponse>(IInvocationHandler<TRequest, TResponse> handler)
        {
            if (handler is IInvocationWorkflow workflow)
            {
                Services.AddSingleton((sp) =>
                {
                    PostInitializeWorkflow(sp, workflow);
                    return handler;
                });
            }
            else
            {
                Services.AddSingleton(handler);
            }

            Services.AddSingleton<IInvocationComponentResolver<IInvocationHandler<TRequest, TResponse>>>(new SingletonInvocationComponentResolver<IInvocationHandler<TRequest, TResponse>>(handler));
            TryAddInvocationPipeline<TRequest, TResponse>();
        }

        public void RegisterHandler<TRequest, TResponse, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THandlerImplementation>(bool singleton = false)
            where THandlerImplementation : class, IInvocationHandler<TRequest, TResponse>
        {
            if (singleton)
            {
                Services.TryAddSingleton<THandlerImplementation>();
                Services.AddSingleton<IInvocationComponentResolver<IInvocationHandler<TRequest, TResponse>>>((sp) =>
                {
                    var lazy = new Lazy<THandlerImplementation>(() =>
                    {
                        var implementation = sp.GetRequiredService<THandlerImplementation>();
                        if (implementation is IInvocationWorkflow workflow)
                        {
                            PostInitializeWorkflow(sp, (IInvocationWorkflow)implementation);
                        }

                        return implementation;
                    });

                    return new DeferredSingletonInvocationComponentResolver<THandlerImplementation>(lazy);
                });
            }
            else
            {
                Services.TryAddTransient<THandlerImplementation>();
                Services.AddSingleton<IInvocationComponentResolver<IInvocationHandler<TRequest, TResponse>>>((sp) => new TransientInvocationComponentResolver<IInvocationHandler<TRequest, TResponse>>(() =>
                {
                    var implementation = sp.GetRequiredService<THandlerImplementation>();
                    if (implementation is IInvocationWorkflow workflow)
                    {
                        PostInitializeWorkflow(sp, (IInvocationWorkflow)implementation);
                    }
                    return implementation;
                }));
            }


            TryAddInvocationPipeline<TRequest, TResponse>();
        }

        internal void TryAddInvocationPipeline<TRequest, TResponse>()
        {
            if (!Services.Any(sd => sd.ImplementationType == typeof(DefaultInvocationPipeline<TRequest, TResponse>)))
            {
                Services.AddSingleton<IInvocationPipeline, DefaultInvocationPipeline<TRequest, TResponse>>();
            }

            if (typeof(TResponse) == EventResponse.Type)
            {
                Services.TryAddSingleton<IInvocationHandlerStrategy<TRequest, TResponse>, ParallelHandlersStrategy<TRequest, TResponse>>();
            }
            else
            {
                Services.TryAddSingleton<IInvocationHandlerStrategy<TRequest, TResponse>, SingleHandlerStrategy<TRequest, TResponse>>();
            }
        }

        internal static void PostInitializeWorkflow(IServiceProvider serviceProvider, IInvocationWorkflow workflow)
        {
            workflow.Mediator = serviceProvider.GetRequiredService<IMediator>();
        }

        public void RegisterHandlerStrategy<TRequest, TResponse, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TStrategy>()
            where TStrategy : class, IInvocationHandlerStrategy<TRequest, TResponse>
        {
            Services.AddSingleton<IInvocationHandlerStrategy<TRequest, TResponse>, TStrategy>();
        }

        public void RegisterHandlerStrategy<TRequest, TResponse>(IInvocationHandlerStrategy<TRequest, TResponse> strategy)
        {
            Services.AddSingleton<IInvocationHandlerStrategy<TRequest, TResponse>>(strategy);
        }
    }
}
