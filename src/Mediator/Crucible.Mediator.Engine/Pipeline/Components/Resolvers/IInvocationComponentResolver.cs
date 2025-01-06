namespace Crucible.Mediator.Engine.Pipeline.Components.Resolvers
{
    public interface IInvocationComponentResolver<out TComponent>
    {
        TComponent ResolveComponent();
    }
}
