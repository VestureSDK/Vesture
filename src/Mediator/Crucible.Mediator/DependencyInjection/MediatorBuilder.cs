using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using Crucible.Mediator.Commands;
using Crucible.Mediator.Engine;
using Crucible.Mediator.Engine.Accessors;
using Crucible.Mediator.Engine.Strategies;
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

            Services.TryAddSingleton<IMediator, Mediator.Engine.Mediator>();
            Services.TryAddSingleton<IInvocationPipelineProvider, InvocationPipelineProvider>();
            Services.TryAddSingleton<IInvocationMiddlewareProvider, InvocationMiddlewareProvider>();
            Services.TryAddSingleton<IInvocationContextFactory, InvocationContextFactory>();
            Services.TryAddSingleton<IDictionary<(Type request, Type response), InvocationPipeline>>((sp) =>
            {
                var pipelines = new Dictionary<(Type request, Type response), InvocationPipeline>();

                foreach (var (request, response) in _registeredContracts)
                {
                    var pipeline = sp.GetRequiredKeyedService<InvocationPipeline>(request);
                    pipelines.TryAdd((request, response), pipeline);
                }

                return pipelines.ToFrozenDictionary();
            });

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

        internal MediatorBuilder AddMiddleware<TRequest, TResponse, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMiddleware>(int? order = null)
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

            Services.AddSingleton<IInvocationComponentAccessor<IInvocationHandler<TRequest, TResponse>>>(new SingletonInvocationComponentAccessor<THandlerService>(handler));
            _registeredContracts.Add((typeof(TRequest), typeof(TResponse)));
            Services.TryAddKeyedSingleton<InvocationPipeline, InvocationPipeline<TRequest, TResponse>>(typeof(TRequest));
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

            Services.AddSingleton<IInvocationComponentAccessor<IInvocationHandler<TRequest, TResponse>>>((sp) => new InvocationComponentAccessor<THandlerService>(() => sp.GetRequiredService<THandlerService>()));
            _registeredContracts.Add((typeof(TRequest), typeof(TResponse)));
            Services.TryAddKeyedSingleton<InvocationPipeline, InvocationPipeline<TRequest, TResponse>>(typeof(TRequest));
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
                Services.TryAddSingleton<IRequestHandlerStrategy<TRequest, TResponse>, ParallelMultiRequestHandlerStrategy<TRequest, TResponse>>();
            }
            else
            {
                Services.TryAddSingleton<IRequestHandlerStrategy<TRequest, TResponse>, SingleRequestHandlerStrategy<TRequest, TResponse>>();
            }

            return this;
        }

        internal MediatorBuilder AddHandlerStrategy<TRequest, TResponse, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TStrategy>()
            where TStrategy : class, IRequestHandlerStrategy<TRequest, TResponse>
        {
            Services.AddSingleton<IRequestHandlerStrategy<TRequest, TResponse>, TStrategy>();
            return this;
        }
    }
}
