namespace Crucible.Mediator.Engine.Resolvers
{
    public class InvocationComponentResolver<TComponent> : IInvocationComponentResolver<TComponent>
    {
        protected virtual TComponent Component { get; private set; }

        protected InvocationComponentResolver()
        {

        }

        public InvocationComponentResolver(TComponent component)
        {
            Component = component;
        }

        public TComponent ResolveComponent() => Component;
    }
}
