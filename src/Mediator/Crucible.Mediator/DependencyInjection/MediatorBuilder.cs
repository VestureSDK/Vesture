using System.Diagnostics.CodeAnalysis;
using Crucible.Mediator.Commands;
using Crucible.Mediator.Engine.Invocation;
using Crucible.Mediator.Engine.Invocation.Strategies;
using Crucible.Mediator.Engine.Pipeline;
using Crucible.Mediator.Engine.Pipeline.Components;
using Crucible.Mediator.Engine.Pipeline.Components.Resolvers;
using Crucible.Mediator.Engine.Pipeline.Context;
using Crucible.Mediator.Events;
using Crucible.Mediator.Invocation;
using Crucible.Mediator.Requests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Crucible.Mediator.DependencyInjection
{
    /// <summary>
    /// Builder to configure the <see cref="IMediator"/> in the <see cref="IServiceCollection"/> instance.
    /// </summary>
    public class MediatorBuilder
    {
        private readonly MediatorConfiguration _configuration = new();

        private readonly HashSet<(Type request, Type response)> _registeredContracts = [];

        /// <summary>
        /// Initializes a new <see cref="MediatorBuilder"/> instance.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> instance.</param>
        /// <param name="setup">The <see cref="MediatorConfiguration"/> setup.</param>
        public MediatorBuilder(IServiceCollection services, Action<MediatorConfiguration>? setup = null)
        {
            Services = services;
            ConfigureInternal(setup);
        }

        /// <summary>
        /// The <see cref="IServiceCollection"/> instance.
        /// </summary>
        public IServiceCollection Services { get; }

        private MediatorBuilder ConfigureInternal(Action<MediatorConfiguration>? setup)
        {
            setup?.Invoke(_configuration);

            Services.TryAddSingleton<IPreHandlerMiddleware>(CatchUnhandledExceptionMiddleware.Instance);
            Services.TryAddSingleton<IInvocationComponentResolver<IPreHandlerMiddleware>, InstanceInvocationComponentResolver<IPreHandlerMiddleware>>();

            Services.TryAddSingleton<IPreInvocationPipelineMiddleware>(CatchUnhandledExceptionMiddleware.Instance);
            Services.TryAddSingleton<IInvocationComponentResolver<IPreInvocationPipelineMiddleware>, InstanceInvocationComponentResolver<IPreInvocationPipelineMiddleware>>();

            Services.TryAddSingleton<IMediator, Mediator.Engine.DefaultMediator>();
            Services.TryAddSingleton<IInvocationContextFactory, DefaultInvocationContextFactory>();

            return this;
        }

        /// <summary>
        /// Configure the <see cref="IMediator"/>.
        /// </summary>
        /// <param name="setup">The <see cref="MediatorConfiguration"/> setup.</param>
        /// <returns>The <see cref="MediatorBuilder"/> for chaining.</returns>
        public MediatorBuilder Configure(Action<MediatorConfiguration> setup)
        {
            return ConfigureInternal(setup);
        }

        /// <summary>
        /// Start defining what to do with the specified <see cref="IRequestHandler{TRequest, TResponse}"/> type.
        /// </summary>
        /// <typeparam name="TRequest">The <see cref="IRequest{TResponse}"/> type.</typeparam>
        /// <typeparam name="TResponse">The expected response from the <see cref="IRequest{TResponse}"/>.</typeparam>
        /// <returns>The <see cref="MediatorRequestBuilder{TRequest, TResponse}"/> for chaining.</returns>
        public MediatorRequestBuilder<TRequest, TResponse> Request<TRequest, TResponse>()
        {
            return new MediatorRequestBuilder<TRequest, TResponse>(this);
        }

        /// <summary>
        /// Start defining what to do with the specified <see cref="ICommand"/> type.
        /// </summary>
        /// <typeparam name="TRequest">The <see cref="ICommand"/> type.</typeparam>
        /// <returns>The <see cref="MediatorRequestBuilder{TRequest, CommandResponse}"/> for chaining.</returns>
        public MediatorRequestBuilder<TRequest, CommandResponse> Command<TRequest>()
        {
            return Request<TRequest, CommandResponse>();
        }

        /// <summary>
        /// Start defining what to do with the specified <see cref="IEvent"/> type.
        /// </summary>
        /// <typeparam name="TEvent">The <see cref="IEvent"/> type.</typeparam>
        /// <returns>The <see cref="MediatorEventBuilder{TEvent}"/> for chaining.</returns>
        public MediatorEventBuilder<TEvent> Event<TEvent>()
        {
            return new MediatorEventBuilder<TEvent>(this);
        }

        /// <summary>
        /// Start defining what to do with the specified <see cref="IRequestHandler{TRequest, TResponse}"/> type.
        /// </summary>
        /// <typeparam name="TRequest">The <see cref="IRequest{TResponse}"/> type.</typeparam>
        /// <typeparam name="TResponse">The expected response from the <see cref="IRequest{TResponse}"/>.</typeparam>
        /// <returns>The <see cref="MediatorRequestBuilder{TRequest, TResponse}"/> for chaining.</returns>
        public MediatorInvocationBuilder<TRequest, TResponse> Invocation<TRequest, TResponse>()
        {
            return new MediatorInvocationBuilder<TRequest, TResponse>(this);
        }

        internal MediatorBuilder AddMiddleware<TRequest, TResponse, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMiddleware>(TMiddleware middleware, int? order = null)
            where TMiddleware : class, IInvocationMiddleware<TRequest, TResponse>
        {
            if (!Services.Any(sd => sd.ServiceType == typeof(TMiddleware)))
            {
                Services.AddSingleton<TMiddleware>(middleware);
                Services.AddSingleton<IInvocationComponentResolver<TMiddleware>>(new InstanceInvocationComponentResolver<TMiddleware>(middleware));
                Services.AddTransient<IMiddlewareInvocationPipelineItem>(sp =>
                {
                    var accessor = sp.GetRequiredService<IInvocationComponentResolver<TMiddleware>>();
                    return new MiddlewareInvocationPipelineItem<TRequest, TResponse>(order ?? 0, accessor);
                });
            }

            return this;
        }

        internal MediatorBuilder AddMiddleware<TRequest, TResponse, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMiddleware>(int? order = null)
            where TMiddleware : class, IInvocationMiddleware<TRequest, TResponse>
        {
            if (!Services.Any(sd => sd.ServiceType == typeof(TMiddleware)))
            {
                Services.AddSingleton<TMiddleware>();
                Services.AddSingleton<IInvocationComponentResolver<TMiddleware>>(sp =>
                {
                    var lazy = new Lazy<TMiddleware>(() => sp.GetRequiredService<TMiddleware>());
                    return new SingletonInvocationComponentResolver<TMiddleware>(lazy);
                });
                Services.AddTransient<IMiddlewareInvocationPipelineItem>(sp =>
                {
                    var accessor = sp.GetRequiredService<IInvocationComponentResolver<TMiddleware>>();
                    return new MiddlewareInvocationPipelineItem<TRequest, TResponse>(order ?? 0, accessor);
                });
            }

            return this;
        }

        internal MediatorBuilder AddHandler<TRequest, TResponse, THandlerService>(THandlerService handler)
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

            Services.AddSingleton<IInvocationComponentResolver<IInvocationHandler<TRequest, TResponse>>>(new InstanceInvocationComponentResolver<THandlerService>(handler));
            _registeredContracts.Add((typeof(TRequest), typeof(TResponse)));
            Services.AddSingleton<IInvocationPipeline, DefaultInvocationPipeline<TRequest, TResponse>>();
            TryAddDefaultRequestHandlerStrategy<TRequest, TResponse>();

            return this;
        }

        internal MediatorBuilder AddHandler<TRequest, TResponse, THandlerService, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THandlerImplementation>()
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

            Services.AddSingleton<IInvocationComponentResolver<IInvocationHandler<TRequest, TResponse>>>((sp) => new TransientInvocationComponentResolver<THandlerService>(() => sp.GetRequiredService<THandlerService>()));
            _registeredContracts.Add((typeof(TRequest), typeof(TResponse)));
            Services.AddSingleton<IInvocationPipeline, DefaultInvocationPipeline<TRequest, TResponse>>();
            TryAddDefaultRequestHandlerStrategy<TRequest, TResponse>();

            return this;
        }

        internal static void PostInitializeWorkflow(IServiceProvider serviceProvider, IInvocationWorkflow workflow)
        {
            workflow.Mediator = serviceProvider.GetRequiredService<IMediator>();
        }

        internal MediatorBuilder TryAddDefaultRequestHandlerStrategy<TRequest, TResponse>()
        {
            if (typeof(TResponse) == EventResponse.Type)
            {
                Services.TryAddSingleton<IInvocationHandlerStrategy<TRequest, TResponse>, ParallelHandlersStrategy<TRequest, TResponse>>();
            }
            else
            {
                Services.TryAddSingleton<IInvocationHandlerStrategy<TRequest, TResponse>, DefaultHandlerStrategy<TRequest, TResponse>>();
            }

            return this;
        }

        internal MediatorBuilder AddHandlerStrategy<TRequest, TResponse, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TStrategy>()
            where TStrategy : class, IInvocationHandlerStrategy<TRequest, TResponse>
        {
            Services.AddSingleton<IInvocationHandlerStrategy<TRequest, TResponse>, TStrategy>();
            return this;
        }
    }
}
