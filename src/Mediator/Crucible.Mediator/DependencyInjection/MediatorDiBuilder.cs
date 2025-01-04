using System.Diagnostics.CodeAnalysis;
using Crucible.Mediator.Commands;
using Crucible.Mediator.Events;
using Crucible.Mediator.Invocation;
using Crucible.Mediator.Invocation.Accessors;
using Crucible.Mediator.Invocation.Strategies;
using Crucible.Mediator.Requests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Crucible.Mediator.DependencyInjection
{
    /// <summary>
    /// Builder to configure the <see cref="IMediator"/> in the <see cref="IServiceCollection"/> instance.
    /// </summary>
    public class MediatorDiBuilder
    {
        private readonly MediatorConfiguration _configuration = new();

        /// <summary>
        /// Initializes a new <see cref="MediatorDiBuilder"/> instance.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> instance.</param>
        /// <param name="setup">The <see cref="MediatorConfiguration"/> setup.</param>
        public MediatorDiBuilder(IServiceCollection services, Action<MediatorConfiguration>? setup = null)
        {
            Services = services;
            ConfigureInternal(setup);
        }

        /// <summary>
        /// The <see cref="IServiceCollection"/> instance.
        /// </summary>
        public IServiceCollection Services { get; }

        private MediatorDiBuilder ConfigureInternal(Action<MediatorConfiguration>? setup)
        {
            setup?.Invoke(_configuration);

            Services.TryAddSingleton<IMediator, Mediator>();
            Services.TryAddSingleton<IInvocationPipelineProvider, InvocationPipelineProvider>();
            Services.TryAddSingleton<IInvocationMiddlewareProvider, InvocationMiddlewareProvider>();
            Services.TryAddSingleton<IInvocationContextFactory, InvocationContextFactory>();
            Services.TryAddSingleton<IRequestHandlerStrategyProvider, RequestHandlerStrategyProvider>();

            return this;
        }

        /// <summary>
        /// Configure the <see cref="IMediator"/>.
        /// </summary>
        /// <param name="setup">The <see cref="MediatorConfiguration"/> setup.</param>
        /// <returns>The <see cref="MediatorDiBuilder"/> for chaining.</returns>
        public MediatorDiBuilder Configure(Action<MediatorConfiguration> setup)
        {
            return ConfigureInternal(setup);
        }

        /// <summary>
        /// Start defining what to do with the specified <see cref="IRequestHandler{TRequest, TResponse}"/> type.
        /// </summary>
        /// <typeparam name="TRequest">The <see cref="IRequest{TResponse}"/> type.</typeparam>
        /// <typeparam name="TResponse">The expected response from the <see cref="IRequest{TResponse}"/>.</typeparam>
        /// <returns>The <see cref="MediatorDiRequestBuilder{TRequest, TResponse}"/> for chaining.</returns>
        public MediatorDiRequestBuilder<TRequest, TResponse> Request<TRequest, TResponse>()
        {
            return new MediatorDiRequestBuilder<TRequest, TResponse>(this);
        }

        /// <summary>
        /// Start defining what to do with the specified <see cref="ICommand"/> type.
        /// </summary>
        /// <typeparam name="TRequest">The <see cref="ICommand"/> type.</typeparam>
        /// <returns>The <see cref="MediatorDiRequestBuilder{TRequest, CommandResponse}"/> for chaining.</returns>
        public MediatorDiRequestBuilder<TRequest, CommandResponse> Command<TRequest>()
        {
            return Request<TRequest, CommandResponse>();
        }

        /// <summary>
        /// Start defining what to do with the specified <see cref="IEvent"/> type.
        /// </summary>
        /// <typeparam name="TEvent">The <see cref="IEvent"/> type.</typeparam>
        /// <returns>The <see cref="MediatorDiEventBuilder{TEvent}"/> for chaining.</returns>
        public MediatorDiEventBuilder<TEvent> Event<TEvent>()
        {
            return new MediatorDiEventBuilder<TEvent>(this);
        }

        /// <summary>
        /// Start defining what to do with the specified <see cref="IRequestHandler{TRequest, TResponse}"/> type.
        /// </summary>
        /// <typeparam name="TRequest">The <see cref="IRequest{TResponse}"/> type.</typeparam>
        /// <typeparam name="TResponse">The expected response from the <see cref="IRequest{TResponse}"/>.</typeparam>
        /// <returns>The <see cref="MediatorDiRequestBuilder{TRequest, TResponse}"/> for chaining.</returns>
        public MediatorDiInvocationBuilder<TRequest, TResponse> Invocation<TRequest, TResponse>()
        {
            return new MediatorDiInvocationBuilder<TRequest, TResponse>(this);
        }

        internal MediatorDiBuilder AddMiddleware<TRequest, TResponse, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMiddleware>(int? order = null)
            where TMiddleware : class, IInvocationMiddleware<TRequest, TResponse>
        {
            if (!Services.Any(sd => sd.ServiceType == typeof(TMiddleware)))
            {
                Services.AddSingleton<TMiddleware>();

                Services.AddSingleton<IInvocationComponentAccessor<TMiddleware>>(sp =>
                {
                    var lazy = new Lazy<TMiddleware>(() => sp.GetRequiredService<TMiddleware>());
                    return new LazyInvocationComponentAccessor<TMiddleware>(lazy);
                });

                Services.AddTransient<InvocationMiddlewareWrapper>(sp =>
                {
                    var accessor = sp.GetRequiredService<IInvocationComponentAccessor<TMiddleware>>();
                    return new InvocationMiddlewareWrapper<TRequest, TResponse>(order ?? 0, accessor);
                });
            }

            return this;
        }

        internal MediatorDiBuilder AddHandler<TRequest, TResponse, THandlerService>(THandlerService handler)
            where THandlerService : class, IInvocationHandler<TRequest, TResponse>
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

            Services.AddSingleton<IInvocationComponentAccessor<IInvocationHandler<TRequest, TResponse>>>(new SingletonInvocationComponentAccessor<THandlerService>(handler));
            Services.TryAddKeyedSingleton<InvocationPipeline<TResponse>, InvocationPipeline<TRequest, TResponse>>(typeof(TRequest));
            TryAddDefaultRequestHandlerStrategy<TRequest, TResponse>();

            return this;
        }

        internal MediatorDiBuilder AddHandler<TRequest, TResponse, THandlerService, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THandlerImplementation>()
            where THandlerService : class, IInvocationHandler<TRequest, TResponse>
            where THandlerImplementation : class, THandlerService
        {
            if (typeof(THandlerImplementation).IsAssignableTo(typeof(IInvocationWorkflow)))
            {
                Services.TryAddTransient<THandlerImplementation>();
                Services.AddTransient<THandlerService>((sp) =>
                {
                    var implementation = sp.GetRequiredService<THandlerImplementation>();
                    PostInitializeWorkflow(sp, (IInvocationWorkflow)implementation);
                    return implementation;
                });
            }
            else
            {
                Services.AddTransient<THandlerService, THandlerImplementation>();
            }

            Services.AddSingleton<IInvocationComponentAccessor<IInvocationHandler<TRequest, TResponse>>>((sp) => new InvocationComponentAccessor<THandlerService>(() => sp.GetRequiredService<THandlerService>()));
            Services.TryAddKeyedSingleton<InvocationPipeline<TResponse>, InvocationPipeline<TRequest, TResponse>>(typeof(TRequest));
            TryAddDefaultRequestHandlerStrategy<TRequest, TResponse>();

            return this;
        }

        internal static void PostInitializeWorkflow(IServiceProvider serviceProvider, IInvocationWorkflow workflow)
        {
            workflow.Mediator = serviceProvider.GetRequiredService<IMediator>();
        }

        internal MediatorDiBuilder TryAddDefaultRequestHandlerStrategy<TRequest, TResponse>()
        {
            if (typeof(TResponse) == EventResponse.Type)
            {
                Services.TryAddSingleton<IRequestHandlerStrategy<TRequest, TResponse>, ParallelMultiRequestHandlerStrategy<TRequest, TResponse>>();
            }
            else
            {
                Services.TryAddSingleton<IRequestHandlerStrategy<TRequest, TResponse>, SingleRequestHandlerStrategy<TRequest, TResponse>>();
            }

            return this;
        }

        internal MediatorDiBuilder AddHandlerStrategy<TRequest, TResponse, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TStrategy>()
            where TStrategy : class, IRequestHandlerStrategy<TRequest, TResponse>
        {
            Services.AddSingleton<IRequestHandlerStrategy<TRequest, TResponse>, TStrategy>();
            return this;
        }
    }
}
