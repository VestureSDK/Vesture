namespace Crucible.Mediator.Invocation
{
    /// <summary>
    /// Defines extension and helpers methods related to <see cref="IInvocationContext"/>.
    /// </summary>
    /// <seealso cref="IInvocationContext"/>
    public static class InvocationContextExtensions
    {
        /// <summary>
        /// Gets the response present in the specified <paramref name="context"/>; or <c>default</c>
        /// of <typeparamref name="TResponse"/> if the reponse is not available.
        /// </summary>
        /// <typeparam name="TResponse">The response type expected.</typeparam>
        /// <param name="context">The <see cref="IInvocationContext"/> instance.</param>
        /// <returns>
        /// The response present in the specified <paramref name="context"/>; or <c>default</c>
        /// of <typeparamref name="TResponse"/> if the reponse is not available.
        /// </returns>
        public static TResponse GetResponseOrDefault<TResponse>(this IInvocationContext context)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return context.HasResponse && context.Response is TResponse tResponse ? tResponse! : default;
#pragma warning restore CS8603 // Possible null reference return.
        }

        /// <summary>
        /// Throws an <see cref="Exception"/> if the specified <paramref name="context"/>
        /// has an error.
        /// </summary>
        /// <typeparam name="TContext">The <see cref="IInvocationContext"/> instance type.</typeparam>
        /// <param name="context">The <see cref="IInvocationContext"/> instance.</param>
        /// <returns>Returns the <paramref name="context"/> for chaining.</returns>
        /// <exception cref="Exception" name="invocation-exception">Any <see cref="Exception"/> that occurred during the invocation pipeline and recorded in the <see cref="IInvocationContext"/>.</exception>
        public static TContext ThrowIfHasError<TContext>(this TContext context)
            where TContext: IInvocationContext
        {
            if (context.HasError)
            {
                // If an error occured, then throw it.
                throw context.Error!;
            }

            return context;
        }
    }
}
