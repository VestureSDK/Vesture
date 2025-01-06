namespace Crucible.Mediator.Engine.Pipeline.Components.Resolvers
{
    public class InstanceInvocationComponentResolver<TComponent> : InvocationComponentResolver<TComponent>
    {
        private readonly TComponent _component;

        public InstanceInvocationComponentResolver(TComponent component)
        {
            _component = component;
        }

        protected override TComponent GetComponent() => _component;
    }
}
