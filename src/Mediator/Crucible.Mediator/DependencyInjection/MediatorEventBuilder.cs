using System.Diagnostics.CodeAnalysis;
using Crucible.Mediator.Commands;
using Crucible.Mediator.Engine.Invocation.Strategies;
using Crucible.Mediator.Events;
using Crucible.Mediator.Invocation;
using Crucible.Mediator.Requests;

namespace Crucible.Mediator.DependencyInjection
{
    /// <summary>
    /// Builder to configure the <see cref="IEvent"/> in the <see cref="IMediator"/> instance.
    /// </summary>
    public class MediatorEventBuilder<TEvent>
    {
        private readonly MediatorBuilder _builder;

        /// <summary>
        /// Initializes a new <see cref="MediatorEventBuilder{TEvent}"/> instance.
        /// </summary>
        /// <param name="builder">The <see cref="MediatorBuilder"/> instance.</param>
        public MediatorEventBuilder(MediatorBuilder builder)
        {
            _builder = builder;
        }

        /// <summary>
        /// Adds a <see cref="IInvocationMiddleware{TEvent, EventResponse}"/> to the execution piepline.
        /// </summary>
        /// <typeparam name="TMiddleware">The <see cref="IInvocationMiddleware{TEvent, EventResponse}"/> type.</typeparam>
        /// <returns>The <see cref="MediatorEventBuilder{TEvent}"/> for chaining.</returns>
        public MediatorEventBuilder<TEvent> AddMiddleware<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMiddleware>(int? order = null)
            where TMiddleware : class, IInvocationMiddleware<TEvent, EventResponse>
        {
            _builder.AddMiddleware<TEvent, EventResponse, TMiddleware>(order);
            return this;
        }

        /// <summary>
        /// Defines the <see cref="IEventHandler{TEvent}"/> associated with the <see cref="IEvent"/> type.
        /// </summary>
        /// <typeparam name="THandler">The <see cref="IEventHandler{TEvent}"/> type.</typeparam>
        /// <returns>The <see cref="MediatorBuilder"/> for chaining.</returns>
        public MediatorEventBuilder<TEvent> AddListener<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THandler>()
            where THandler : class, IEventHandler<TEvent>
        {
            _builder.AddHandler<TEvent, EventResponse, IInvocationHandler<TEvent, EventResponse>, THandler>();
            return this;
        }

        /// <summary>
        /// Sets the execution strategy to <see cref="ParallelMultiRequestHandlerStrategy{TRequest, TResponse}"/>.
        /// </summary>
        /// <returns>The <see cref="MediatorBuilder"/> for chaining.</returns>
        public MediatorEventBuilder<TEvent> PublishInParallel()
        {
            return WithPublishStrategy<ParallelHandlersStrategy<TEvent, EventResponse>>();
        }

        /// <summary>
        /// Sets the execution strategy to <see cref="SequentialMultiRequestHandlerStrategy{TRequest, TResponse}"/>.
        /// </summary>
        /// <returns>The <see cref="MediatorBuilder"/> for chaining.</returns>
        public MediatorEventBuilder<TEvent> PublishSequentially()
        {
            return WithPublishStrategy<SequentialHandlersStrategy<TEvent, EventResponse>>();
        }

        /// <summary>
        /// Sets the execution strategy to the specified <typeparamref name="TStrategy"/>.
        /// </summary>
        /// <returns>The <see cref="MediatorBuilder"/> for chaining.</returns>
        public MediatorEventBuilder<TEvent> WithPublishStrategy<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TStrategy>()
            where TStrategy : class, IInvocationHandlerStrategy<TEvent, EventResponse>
        {
            _builder.AddHandlerStrategy<TEvent, EventResponse, TStrategy>();
            return this;
        }

        /// <summary>
        /// Start defining what to do with the specified <see cref="IRequest{TResponse}"/> type.
        /// </summary>
        /// <typeparam name="TRequest">The <see cref="IRequest{TResponse}"/> type.</typeparam>
        /// <typeparam name="TResponse">The expected response from the <see cref="IRequest{TResponse}"/>.</typeparam>
        /// <returns>The <see cref="MediatorRequestBuilder{TRequest, TResponse}"/> for chaining.</returns>
        public MediatorRequestBuilder<TRequest, TResponse> Request<TRequest, TResponse>()
        {
            return new MediatorRequestBuilder<TRequest, TResponse>(_builder);
        }

        /// <summary>
        /// Start defining what to do with the specified <see cref="ICommandHandler{TCommand}"/> type.
        /// </summary>
        /// <typeparam name="TRequest">The <see cref="ICommand"/> type.</typeparam>
        /// <returns>The <see cref="MediatorRequestBuilder{TRequest, CommandResponse}"/> for chaining.</returns>
        public MediatorRequestBuilder<TRequest, CommandResponse> Command<TRequest>()
        {
            return Request<TRequest, CommandResponse>();
        }
    }
}
