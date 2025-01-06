namespace Crucible.Mediator.Engine.Pipeline.Components.Resolvers
{
    public class InstanceInvocationComponentResolver<TComponent> : IInvocationComponentResolver<TComponent>
    {
        private readonly TComponent _component;

        public InstanceInvocationComponentResolver(TComponent component)
        {
            _component = component;
        }

        public TComponent ResolveComponent() => _component;
    }
}
