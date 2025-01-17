namespace Ingot.Mediator.Engine.Pipeline.Resolvers
{
    /// <summary>
    /// The <see cref="SingletonInvocationComponentResolver{TComponent}"/> is an implementation
    /// of <see cref="IInvocationComponentResolver{TComponent}"/> returning an already
    /// initialized instance of <typeparamref name="TComponent"/>.
    /// </summary>
    /// <inheritdoc cref="IInvocationComponentResolver{TComponent}"/>
    public class SingletonInvocationComponentResolver<TComponent> : IInvocationComponentResolver<TComponent>
    {
        private readonly TComponent _component;

        /// <summary>
        /// Initializes a new <see cref="SingletonInvocationComponentResolver{TComponent}"/> instance.
        /// </summary>
        /// <param name="component">The initialized <typeparamref name="TComponent"/> instance.</param>
        public SingletonInvocationComponentResolver(TComponent component)
        {
            _component = component;
        }

        /// <inheritdoc />
        public TComponent ResolveComponent() => _component;
    }
}
