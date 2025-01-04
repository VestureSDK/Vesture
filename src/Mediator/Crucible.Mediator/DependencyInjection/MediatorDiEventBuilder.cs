using System.Diagnostics.CodeAnalysis;
using Crucible.Mediator.Commands;
using Crucible.Mediator.Events;
using Crucible.Mediator.Invocation;
using Crucible.Mediator.Invocation.Strategies;
using Crucible.Mediator.Requests;

namespace Crucible.Mediator.DependencyInjection
{
    /// <summary>
    /// Builder to configure the <see cref="IEvent"/> in the <see cref="IMediator"/> instance.
    /// </summary>
    public class MediatorDiEventBuilder<TEvent>
    {
        private readonly MediatorDiBuilder _builder;

        /// <summary>
        /// Initializes a new <see cref="MediatorDiEventBuilder{TEvent}"/> instance.
        /// </summary>
        /// <param name="builder">The <see cref="MediatorDiBuilder"/> instance.</param>
        public MediatorDiEventBuilder(MediatorDiBuilder builder)
        {
            _builder = builder;
        }

        /// <summary>
        /// Adds a <see cref="IInvocationMiddleware{TEvent, EventResponse}"/> to the execution piepline.
        /// </summary>
        /// <typeparam name="TMiddleware">The <see cref="IInvocationMiddleware{TEvent, EventResponse}"/> type.</typeparam>
        /// <returns>The <see cref="MediatorDiEventBuilder{TEvent}"/> for chaining.</returns>
        public MediatorDiEventBuilder<TEvent> AddMiddleware<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMiddleware>(int? order = null)
            where TMiddleware : class, IInvocationMiddleware<TEvent, EventResponse>
        {
            _builder.AddMiddleware<TEvent, EventResponse, TMiddleware>(order);
            return this;
        }

        /// <summary>
        /// Defines the <see cref="IEventHandler{TEvent}"/> associated with the <see cref="IEvent"/> type.
        /// </summary>
        /// <typeparam name="THandler">The <see cref="IEventHandler{TEvent}"/> type.</typeparam>
        /// <returns>The <see cref="MediatorDiBuilder"/> for chaining.</returns>
        public MediatorDiEventBuilder<TEvent> AddListener<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THandler>()
            where THandler : class, IEventHandler<TEvent>
        {
            _builder.AddHandler<TEvent, EventResponse, IInvocationHandler<TEvent, EventResponse>, THandler>();
            return this;
        }

        /// <summary>
        /// Sets the execution strategy to <see cref="ParallelMultiRequestHandlerStrategy{TRequest, TResponse}"/>.
        /// </summary>
        /// <returns>The <see cref="MediatorDiBuilder"/> for chaining.</returns>
        public MediatorDiEventBuilder<TEvent> PublishInParallel()
        {
            return WithPublishStrategy<ParallelMultiRequestHandlerStrategy<TEvent, EventResponse>>();
        }

        /// <summary>
        /// Sets the execution strategy to <see cref="SequentialMultiRequestHandlerStrategy{TRequest, TResponse}"/>.
        /// </summary>
        /// <returns>The <see cref="MediatorDiBuilder"/> for chaining.</returns>
        public MediatorDiEventBuilder<TEvent> PublishSequentially()
        {
            return WithPublishStrategy<SequentialMultiRequestHandlerStrategy<TEvent, EventResponse>>();
        }

        /// <summary>
        /// Sets the execution strategy to the specified <typeparamref name="TStrategy"/>.
        /// </summary>
        /// <returns>The <see cref="MediatorDiBuilder"/> for chaining.</returns>
        public MediatorDiEventBuilder<TEvent> WithPublishStrategy<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TStrategy>()
            where TStrategy : class, IRequestHandlerStrategy<TEvent, EventResponse>
        {
            _builder.AddHandlerStrategy<TEvent, EventResponse, TStrategy>();
            return this;
        }

        /// <summary>
        /// Start defining what to do with the specified <see cref="IRequest{TResponse}"/> type.
        /// </summary>
        /// <typeparam name="TRequest">The <see cref="IRequest{TResponse}"/> type.</typeparam>
        /// <typeparam name="TResponse">The expected response from the <see cref="IRequest{TResponse}"/>.</typeparam>
        /// <returns>The <see cref="MediatorDiRequestBuilder{TRequest, TResponse}"/> for chaining.</returns>
        public MediatorDiRequestBuilder<TRequest, TResponse> Request<TRequest, TResponse>()
        {
            return new MediatorDiRequestBuilder<TRequest, TResponse>(_builder);
        }

        /// <summary>
        /// Start defining what to do with the specified <see cref="ICommandHandler{TCommand}"/> type.
        /// </summary>
        /// <typeparam name="TRequest">The <see cref="ICommand"/> type.</typeparam>
        /// <returns>The <see cref="MediatorDiRequestBuilder{TRequest, CommandResponse}"/> for chaining.</returns>
        public MediatorDiRequestBuilder<TRequest, CommandResponse> Command<TRequest>()
        {
            return Request<TRequest, CommandResponse>();
        }
    }
}
