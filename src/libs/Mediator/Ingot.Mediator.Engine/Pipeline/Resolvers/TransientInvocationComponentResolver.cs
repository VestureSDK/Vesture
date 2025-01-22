namespace Ingot.Mediator.Engine.Pipeline.Resolvers
{
    /// <summary>
    /// The <see cref="TransientInvocationComponentResolver{TComponent}"/> is an implementation
    /// of <see cref="IInvocationComponentResolver{TComponent}"/> using a <see cref="Func{TResult}"/> to resolve
    /// a <typeparamref name="TComponent"/>.
    /// </summary>
    /// <inheritdoc cref="IInvocationComponentResolver{TComponent}"/>
    public class TransientInvocationComponentResolver<TComponent>
        : IInvocationComponentResolver<TComponent>
    {
        private readonly Func<TComponent> _componentFactory;

        /// <summary>
        /// Initializes a new <see cref="DeferredSingletonInvocationComponentResolver{TComponent}"/> instance.
        /// </summary>
        /// <param name="componentFactory">The <see cref="Func{TResult}"/> returning a <typeparamref name="TComponent"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="componentFactory"/> is <see langword="null" />.</exception>
        public TransientInvocationComponentResolver(Func<TComponent> componentFactory)
        {
            ArgumentNullException.ThrowIfNull(componentFactory, nameof(componentFactory));

            _componentFactory = componentFactory;
        }

        /// <inheritdoc />
        public TComponent ResolveComponent() => _componentFactory.Invoke();
    }
}
