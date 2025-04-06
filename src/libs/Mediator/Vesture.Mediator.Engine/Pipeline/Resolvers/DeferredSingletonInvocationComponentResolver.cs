using System.Diagnostics.CodeAnalysis;

namespace Vesture.Mediator.Engine.Pipeline.Resolvers
{
    /// <summary>
    /// The <see cref="DeferredSingletonInvocationComponentResolver{TComponent}"/> is an implementation
    /// of <see cref="IInvocationComponentResolver{TComponent}"/> using a <see cref="Lazy{T}"/> to resolve
    /// a <typeparamref name="TComponent"/>.
    /// </summary>
    /// <inheritdoc cref="IInvocationComponentResolver{TComponent}"/>
    public class DeferredSingletonInvocationComponentResolver<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
            TComponent
    > : IInvocationComponentResolver<TComponent>
    {
        private readonly Lazy<TComponent> _componentInitializer;

        /// <summary>
        /// Initializes a new <see cref="DeferredSingletonInvocationComponentResolver{TComponent}"/> instance.
        /// </summary>
        /// <param name="componentInitializer">The <see cref="Lazy{T}"/> returning a <typeparamref name="TComponent"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="componentInitializer"/> is <see langword="null" />.</exception>
        public DeferredSingletonInvocationComponentResolver(Lazy<TComponent> componentInitializer)
        {
            if (componentInitializer is null)
            {
                throw new ArgumentNullException(nameof(componentInitializer));
            }

            _componentInitializer = componentInitializer;
        }

        /// <inheritdoc />
        public TComponent ResolveComponent() => _componentInitializer.Value;
    }
}
