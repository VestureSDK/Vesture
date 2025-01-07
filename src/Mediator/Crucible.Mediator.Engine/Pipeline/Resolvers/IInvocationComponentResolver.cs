namespace Crucible.Mediator.Engine.Pipeline.Resolvers
{
    /// <summary>
    /// An <see cref="IInvocationComponentResolver{TComponent}"/> allows to
    /// defer the resolution of components and construct <see cref="IInvocationPipeline{TResponse}"/>
    /// pre-emptively.
    /// </summary>
    /// <typeparam name="TComponent">The type of component to resolve.</typeparam>
    public interface IInvocationComponentResolver<out TComponent>
    {
        /// <summary>
        /// Resolves the component.
        /// </summary>
        /// <returns>The resolved <typeparamref name="TComponent"/> instance.</returns>
        TComponent ResolveComponent();
    }
}
