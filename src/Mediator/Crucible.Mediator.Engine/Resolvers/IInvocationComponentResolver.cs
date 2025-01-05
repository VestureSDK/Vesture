namespace Crucible.Mediator.Engine.Resolvers
{
    public interface IInvocationComponentResolver<TComponent>
    {
        TComponent ResolveComponent();
    }
}
