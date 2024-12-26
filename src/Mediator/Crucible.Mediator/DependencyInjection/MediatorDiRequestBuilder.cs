using Crucible.Mediator.Commands;
using Crucible.Mediator.Invocation;
using Crucible.Mediator.Requests;
using System.Diagnostics.CodeAnalysis;

namespace Crucible.Mediator.DependencyInjection
{
    /// <summary>
    /// Builder to configure the <see cref="IRequestHandler{TRequest, TResponse}"/> and <see cref="ICommandHandler{TCommand}"/> in the <see cref="IMediator"/> instance.
    /// </summary>
    public class MediatorDiRequestBuilder<TRequest, TResponse>
    {
        private readonly MediatorDiBuilder _builder;

        /// <summary>
        /// Initializes a new <see cref="MediatorDiRequestBuilder{TRequest, TResponse}"/> instance.
        /// </summary>
        /// <param name="builder">The <see cref="MediatorDiBuilder"/> instance.</param>
        public MediatorDiRequestBuilder(MediatorDiBuilder builder)
        {
            _builder = builder;
        }

        /// <summary>
        /// Adds a <see cref="IInvocationMiddleware{TRequest, TResponse}"/> to the execution piepline.
        /// </summary>
        /// <typeparam name="TMiddleware">The <see cref="IInvocationMiddleware{TRequest, TResponse}"/> type.</typeparam>
        /// <returns>The <see cref="MediatorDiRequestBuilder{TRequest, TResponse}"/> for chaining.</returns>
        public MediatorDiRequestBuilder<TRequest, TResponse> AddMiddleware<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMiddleware>(int? order = null)
            where TMiddleware : class, IInvocationMiddleware<TRequest, TResponse>
        {
            _builder.AddMiddleware<TRequest, TResponse, TMiddleware>(order);
            return this;
        }

        /// <summary>
        /// Defines the <see cref="IRequestHandler{TRequest, TResponse}"/> or <see cref="ICommandHandler{TCommand}"/> associated with the <see cref="IRequest{TResponse}"/> or <see cref="ICommand"/> type.
        /// </summary>
        /// <typeparam name="THandler">The <see cref="IRequestHandler{TRequest, TResponse}"/> or <see cref="ICommandHandler{TCommand}"/> type.</typeparam>
        /// <returns>The <see cref="MediatorDiBuilder"/> for chaining.</returns>
        public MediatorDiBuilder HandleWith<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THandler>()
            where THandler : class, IRequestHandler<TRequest, TResponse>
        {
            return _builder.AddHandler<TRequest, TResponse, IRequestHandler<TRequest, TResponse>, THandler>();
        }
    }
}
