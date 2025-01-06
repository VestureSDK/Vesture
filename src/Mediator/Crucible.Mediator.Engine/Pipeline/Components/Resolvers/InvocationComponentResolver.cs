namespace Crucible.Mediator.Engine.Pipeline.Components.Resolvers
{
    public abstract class InvocationComponentResolver<TComponent> : IInvocationComponentResolver<TComponent>
    {
        protected TComponent Component => GetComponent();

        protected abstract TComponent GetComponent();

        public TComponent ResolveComponent() => Component;
    }
}
