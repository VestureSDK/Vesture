using Crucible.Mediator.Commands;
using Crucible.Mediator.Requests;
using Crucible.Mediator.Invocation;
using Crucible.Mediator.Events;
using System.Diagnostics.CodeAnalysis;

namespace Crucible.Mediator.DependencyInjection
{
    /// <summary>
    /// Builder to configure the <see cref="IInvocationMiddleware{TInvocationRequest, TInvocationResponse}"/> in the <see cref="IMediator"/> instance.
    /// </summary>
    public class MediatorDiInvocationBuilder<TInvocationRequest, TInvocationResponse>
    {
        private readonly MediatorDiBuilder _builder;

        /// <summary>
        /// Initializes a new <see cref="MediatorDiInvocationBuilder{TInvocationRequest, TInvocationResponse}"/> instance.
        /// </summary>
        /// <param name="builder">The <see cref="MediatorDiBuilder"/> instance.</param>
        public MediatorDiInvocationBuilder(MediatorDiBuilder builder)
        {
            _builder = builder;
        }

        /// <summary>
        /// Adds a <see cref="IInvocationMiddleware{TInvocationRequest, TInvocationResponse}"/> to the execution piepline.
        /// </summary>
        /// <typeparam name="TMiddleware">The <see cref="IInvocationMiddleware{TInvocationRequest, TInvocationResponse}"/> type.</typeparam>
        /// <returns>The <see cref="MediatorDiInvocationBuilder{TInvocationRequest, TInvocationResponse}"/> for chaining.</returns>
        public MediatorDiInvocationBuilder<TInvocationRequest, TInvocationResponse> AddMiddleware<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMiddleware>(int? order = null)
            where TMiddleware : class, IInvocationMiddleware<TInvocationRequest, TInvocationResponse>
        {
            _builder.AddMiddleware<TInvocationRequest, TInvocationResponse, TMiddleware>(order);
            return this;
        }

        /// <summary>
        /// Start defining what to do with the specified <see cref="IRequestHandler{TRequest, TResponse}"/> type.
        /// </summary>
        /// <typeparam name="TRequest">The <see cref="IRequest{TResponse}"/> type.</typeparam>
        /// <typeparam name="TResponse">The expected response from the <see cref="IRequest{TResponse}"/>.</typeparam>
        /// <returns>The <see cref="MediatorDiRequestBuilder{TRequest, TResponse}"/> for chaining.</returns>
        public MediatorDiInvocationBuilder<TRequest, TResponse> Invocation<TRequest, TResponse>()
        {
            return new MediatorDiInvocationBuilder<TRequest, TResponse>(_builder);
        }

        /// <summary>
        /// Start defining what to do with the specified <see cref="IRequestHandler{TRequest, TResponse}"/> type.
        /// </summary>
        /// <typeparam name="TRequest">The <see cref="IRequest{TResponse}"/> type.</typeparam>
        /// <typeparam name="TResponse">The expected response from the <see cref="IRequest{TResponse}"/>.</typeparam>
        /// <returns>The <see cref="MediatorDiRequestBuilder{TRequest, TResponse}"/> for chaining.</returns>
        public MediatorDiRequestBuilder<TRequest, TResponse> Request<TRequest, TResponse>()
        {
            return new MediatorDiRequestBuilder<TRequest, TResponse>(_builder);
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
            return new MediatorDiEventBuilder<TEvent>(_builder);
        }

    }
}
