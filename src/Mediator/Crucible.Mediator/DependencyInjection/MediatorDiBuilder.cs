using System.Diagnostics.CodeAnalysis;
using Crucible.Mediator.Commands;
using Crucible.Mediator.Events;
using Crucible.Mediator.Invocation;
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

        private readonly Dictionary<(Type request, Type response), bool> _tracker = new Dictionary<(Type request, Type response), bool>();

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

            // Adds the require services for the mediator
            Services.TryAddSingleton<ICommandInvoker, CommandInvoker>();
            Services.TryAddSingleton<IRequestExecutor, RequestInvoker>();
            Services.TryAddSingleton<IEventPublisher, EventPublisher>();

            Services.TryAddSingleton<IMediator>((sp) => new Mediator(
                sp.GetRequiredService<IInvocationPipelineProvider>(),
                sp.GetRequiredService<ICommandInvoker>(),
                sp.GetRequiredService<IRequestExecutor>(),
                sp.GetRequiredService<IEventPublisher>()
            ));

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
            Services.TryAddTransient<TMiddleware>();

            var trackerTuple = (typeof(TRequest), typeof(TResponse));
            if (!_tracker.ContainsKey(trackerTuple))
            {
                _tracker.Add(trackerTuple, true);
                Services.AddTransient<InvocationMiddlewareWrapper>(sp =>
                {
                    var lazy = new Lazy<IInvocationMiddleware<TRequest, TResponse>>(() => sp.GetRequiredService<TMiddleware>());
                    var wrapper = new InvocationMiddlewareWrapper<TRequest, TResponse>(lazy);
                    if (order != null)
                    {
                        wrapper.Order = order.Value;
                    }

                    return wrapper;
                });
            }

            return this;
        }

        internal MediatorDiBuilder AddHandler<TRequest, TResponse, THandlerService, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THandlerImplementation>()
            where THandlerService : class
            where THandlerImplementation : class, THandlerService
        {
            Services.AddTransient<THandlerService, THandlerImplementation>();
            Services.TryAddKeyedSingleton<InvocationPipeline<TResponse>, InvocationPipeline<TRequest, TResponse>>(typeof(TRequest));
            TryAddDefaultRequestHandlerStrategy<TRequest, TResponse>();

            return this;
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
