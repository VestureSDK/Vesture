﻿using Vesture.Mediator.Invocation;

namespace Vesture.Mediator.Engine.Pipeline.Internal
{
    /// <summary>
    /// An <see cref="IMiddlewareInvocationPipelineItem" /> defines a way to register
    /// a <see cref="IInvocationMiddleware{TRequst, TResult}"/> with an order.
    /// </summary>
    public interface IMiddlewareInvocationPipelineItem
    {
        /// <summary>
        /// The underlying <see cref="IInvocationMiddleware{TRequest, TResponse}"/> type.
        /// </summary>
        Type MiddlewareType { get; }

        /// <summary>
        /// The order of the <see cref="IInvocationHandler{TRequest, TResponse}"/> compare
        /// to the others. The lower, the earliest in the pipeline, the greater the closest
        /// to the handler.
        /// </summary>
        int Order { get; }

        /// <summary>
        /// Defines if the <see cref="IInvocationHandler{TRequest, TResponse}"/> is applicable for
        /// the specified <paramref name="contextType"/>.
        /// </summary>
        /// <param name="contextType">The <see cref="Type"/> of the <see cref="IInvocationContext{TResponse}"/>.</param>
        /// <returns><see langword="true"/> if applicable; otherwise <see langword="false"/>.</returns>
        bool IsApplicable(Type contextType);
    }

    /// <inheritdoc cref="IMiddlewareInvocationPipelineItem"/>
    /// <inheritdoc cref="IInvocationMiddleware{TRequest, TResponse}"/>
    /// <seealso cref="IInvocationMiddleware{TRequest, TResponse}"/>
    public interface IMiddlewareInvocationPipelineItem<TRequest, TResponse>
        : IMiddlewareInvocationPipelineItem,
            IInvocationMiddleware<TRequest, TResponse> { }
}
