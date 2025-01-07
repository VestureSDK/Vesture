namespace Crucible.Mediator.Engine.Pipeline.Resolvers
{
    /// <summary>
    /// The <see cref="DeferredSingletonInvocationComponentResolver{TComponent}"/> is an implementation
    /// of <see cref="IInvocationComponentResolver{TComponent}"/> using a <see cref="Lazy{T}"/> to resolve
    /// a <typeparamref name="TComponent"/>.
    /// </summary>
    /// <inheritdoc cref="IInvocationComponentResolver{TComponent}"/>
    public class DeferredSingletonInvocationComponentResolver<TComponent> : IInvocationComponentResolver<TComponent>
    {
        private readonly Lazy<TComponent> _componentInitializer;

        /// <summary>
        /// Initializes a new <see cref="DeferredSingletonInvocationComponentResolver{TComponent}"/> instance.
        /// </summary>
        /// <param name="componentInitializer">The <see cref="Lazy{T}"/> returning a <typeparamref name="TComponent"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="componentInitializer"/> is <see langword="null" />.</exception>
        public DeferredSingletonInvocationComponentResolver(Lazy<TComponent> componentInitializer)
        {
            ArgumentNullException.ThrowIfNull(componentInitializer, nameof(componentInitializer));

            _componentInitializer = componentInitializer;
        }

        /// <inheritdoc />
        public TComponent ResolveComponent() => _componentInitializer.Value;
    }
}
