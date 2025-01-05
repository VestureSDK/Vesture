using System.Diagnostics.CodeAnalysis;
using Crucible.Mediator.Commands;
using Crucible.Mediator.Invocation;
using Crucible.Mediator.Requests;

namespace Crucible.Mediator.DependencyInjection
{
    /// <summary>
    /// Builder to configure the <see cref="IRequestHandler{TRequest, TResponse}"/> and <see cref="ICommandHandler{TCommand}"/> in the <see cref="IMediator"/> instance.
    /// </summary>
    public class MediatorRequestBuilder<TRequest, TResponse>
    {
        private readonly MediatorBuilder _builder;

        /// <summary>
        /// Initializes a new <see cref="MediatorRequestBuilder{TRequest, TResponse}"/> instance.
        /// </summary>
        /// <param name="builder">The <see cref="MediatorBuilder"/> instance.</param>
        public MediatorRequestBuilder(MediatorBuilder builder)
        {
            _builder = builder;
        }

        /// <summary>
        /// Adds a <see cref="IInvocationMiddleware{TRequest, TResponse}"/> to the execution piepline.
        /// </summary>
        /// <typeparam name="TMiddleware">The <see cref="IInvocationMiddleware{TRequest, TResponse}"/> type.</typeparam>
        /// <returns>The <see cref="MediatorRequestBuilder{TRequest, TResponse}"/> for chaining.</returns>
        public MediatorRequestBuilder<TRequest, TResponse> AddMiddleware<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMiddleware>(int? order = null)
            where TMiddleware : class, IInvocationMiddleware<TRequest, TResponse>
        {
            _builder.AddMiddleware<TRequest, TResponse, TMiddleware>(order);
            return this;
        }

        /// <summary>
        /// Defines the <see cref="IRequestHandler{TRequest, TResponse}"/> or <see cref="ICommandHandler{TCommand}"/> associated with the <see cref="IRequest{TResponse}"/> or <see cref="ICommand"/> type.
        /// </summary>
        /// <typeparam name="THandler">The <see cref="IRequestHandler{TRequest, TResponse}"/> or <see cref="ICommandHandler{TCommand}"/> type.</typeparam>
        /// <returns>The <see cref="MediatorBuilder"/> for chaining.</returns>
        public MediatorBuilder HandleWith<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THandler>()
            where THandler : class, IInvocationHandler<TRequest, TResponse>
        {
            return _builder.AddHandler<TRequest, TResponse, IInvocationHandler<TRequest, TResponse>, THandler>();
        }

        /// <summary>
        /// Defines the <see cref="IRequestHandler{TRequest, TResponse}"/> or <see cref="ICommandHandler{TCommand}"/> associated with the <see cref="IRequest{TResponse}"/> or <see cref="ICommand"/> type.
        /// </summary>
        /// <param name="handler">The <see cref="IRequestHandler{TRequest, TResponse}"/> instance.</param>
        /// <returns>The <see cref="MediatorBuilder"/> for chaining.</returns>
        public MediatorBuilder HandleWith(IInvocationHandler<TRequest, TResponse> handler)
        {
            return _builder.AddHandler<TRequest, TResponse, IInvocationHandler<TRequest, TResponse>>(handler);
        }
    }
}
