namespace Crucible.Mediator.Invocation
{
    /// <summary>
    /// Helper class to determine the order of a <see cref="IInvocationMiddleware{TRequest, TResponse}"/>.
    /// </summary>
    public abstract class InvocationMiddlewareOrder
    {
        /// <summary>
        /// The <see cref="IInvocationMiddleware{TRequest, TResponse}"/> will be invoked before any other.
        /// </summary>
        /// <remarks>
        /// If multiple <see cref="IInvocationMiddleware{TRequest, TResponse}"/> have set <see cref="BeforeAllOthers"/> 
        /// as their <see cref="IInvocationMiddleware{TRequest, TResponse}.Order"/>, the order of registration will be used as second ordering.
        /// </remarks>
        public const int BeforeAllOthers = Int32.MinValue + 10_000;

        /// <summary>
        /// The <see cref="IInvocationMiddleware{TRequest, TResponse}"/> will be invoked amongst the others without a specific ordering.
        /// </summary>
        /// <remarks>
        /// If multiple <see cref="IInvocationMiddleware{TRequest, TResponse}"/> have set <see cref="Default"/> 
        /// as their <see cref="IInvocationMiddleware{TRequest, TResponse}.Order"/>, the order of registration will be used as second ordering.
        /// </remarks>
        public const int Default = 0;

        /// <summary>
        /// The <see cref="IInvocationMiddleware{TRequest, TResponse}"/> will be invoked after any other.
        /// </summary>
        /// <remarks>
        /// If multiple <see cref="IInvocationMiddleware{TRequest, TResponse}"/> have set <see cref="AfterAllOthers"/> 
        /// as their <see cref="IInvocationMiddleware{TRequest, TResponse}.Order"/>, the order of registration will be used as second ordering.
        /// </remarks>
        public const int AfterAllOthers = Int32.MaxValue - 10_000;
    }
}
