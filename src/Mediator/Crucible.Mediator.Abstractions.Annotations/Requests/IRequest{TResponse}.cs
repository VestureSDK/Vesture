namespace Crucible.Mediator.Requests
{
    /// <summary>
    /// Defines a mediator related request returning a 
    /// <typeparamref name="TResponse"/> when invoked via a mediator.
    /// </summary>
    /// <remarks>
    /// This is a marker interface to sugarcoat some C# syntax. 
    /// While not necessary to implement it, it will help the developer experience.
    /// </remarks>
    /// <typeparam name="TResponse">The type of response expected from the mediator.</typeparam>
    public interface IRequest<TResponse>
    {

    }
}
