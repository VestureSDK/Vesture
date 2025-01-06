using System.Diagnostics.CodeAnalysis;
using Crucible.Mediator.Commands;
using Crucible.Mediator.Events;
using Crucible.Mediator.Invocation;
using Crucible.Mediator.Requests;

namespace Crucible.Mediator.DependencyInjection
{
    /// <summary>
    /// Builder to configure the <see cref="IInvocationMiddleware{TInvocationRequest, TInvocationResponse}"/> in the <see cref="IMediator"/> instance.
    /// </summary>
    public class MediatorInvocationBuilder<TInvocationRequest, TInvocationResponse>
    {
        private readonly MediatorBuilder _builder;

        /// <summary>
        /// Initializes a new <see cref="MediatorInvocationBuilder{TInvocationRequest, TInvocationResponse}"/> instance.
        /// </summary>
        /// <param name="builder">The <see cref="MediatorBuilder"/> instance.</param>
        public MediatorInvocationBuilder(MediatorBuilder builder)
        {
            _builder = builder;
        }

        /// <summary>
        /// Adds a <see cref="IInvocationMiddleware{TInvocationRequest, TInvocationResponse}"/> to the execution piepline.
        /// </summary>
        /// <typeparam name="TMiddleware">The <see cref="IInvocationMiddleware{TInvocationRequest, TInvocationResponse}"/> type.</typeparam>
        /// <returns>The <see cref="MediatorInvocationBuilder{TInvocationRequest, TInvocationResponse}"/> for chaining.</returns>
        public MediatorInvocationBuilder<TInvocationRequest, TInvocationResponse> AddMiddleware<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMiddleware>(int? order = null)
            where TMiddleware : class, IInvocationMiddleware<TInvocationRequest, TInvocationResponse>
        {
            _builder.AddMiddleware<TInvocationRequest, TInvocationResponse, TMiddleware>(order);
            return this;
        }

        /// <summary>
        /// Adds a <see cref="IInvocationMiddleware{TInvocationRequest, TInvocationResponse}"/> to the execution piepline.
        /// </summary>
        /// <typeparam name="TMiddleware">The <see cref="IInvocationMiddleware{TInvocationRequest, TInvocationResponse}"/> type.</typeparam>
        /// <returns>The <see cref="MediatorInvocationBuilder{TInvocationRequest, TInvocationResponse}"/> for chaining.</returns>
        public MediatorInvocationBuilder<TInvocationRequest, TInvocationResponse> AddMiddleware<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMiddleware>(TMiddleware middleware, int? order = null)
            where TMiddleware : class, IInvocationMiddleware<TInvocationRequest, TInvocationResponse>
        {
            _builder.AddMiddleware<TInvocationRequest, TInvocationResponse, TMiddleware>(middleware, order);
            return this;
        }

        /// <summary>
        /// Start defining what to do with the specified <see cref="IRequestHandler{TRequest, TResponse}"/> type.
        /// </summary>
        /// <typeparam name="TRequest">The <see cref="IRequest{TResponse}"/> type.</typeparam>
        /// <typeparam name="TResponse">The expected response from the <see cref="IRequest{TResponse}"/>.</typeparam>
        /// <returns>The <see cref="MediatorRequestBuilder{TRequest, TResponse}"/> for chaining.</returns>
        public MediatorInvocationBuilder<TRequest, TResponse> Invocation<TRequest, TResponse>()
        {
            return new MediatorInvocationBuilder<TRequest, TResponse>(_builder);
        }

        /// <summary>
        /// Start defining what to do with the specified <see cref="IRequestHandler{TRequest, TResponse}"/> type.
        /// </summary>
        /// <typeparam name="TRequest">The <see cref="IRequest{TResponse}"/> type.</typeparam>
        /// <typeparam name="TResponse">The expected response from the <see cref="IRequest{TResponse}"/>.</typeparam>
        /// <returns>The <see cref="MediatorRequestBuilder{TRequest, TResponse}"/> for chaining.</returns>
        public MediatorRequestBuilder<TRequest, TResponse> Request<TRequest, TResponse>()
        {
            return new MediatorRequestBuilder<TRequest, TResponse>(_builder);
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
            return new MediatorEventBuilder<TEvent>(_builder);
        }

    }
}
