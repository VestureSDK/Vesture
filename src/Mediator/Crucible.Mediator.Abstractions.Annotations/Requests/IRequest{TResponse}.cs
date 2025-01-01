namespace Crucible.Mediator.Requests
{
    /// <summary>
    /// Defines a request returning a <typeparamref name="TResponse"/> when invoked via a mediator.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>While not necessary to implement it; This marker interface will enhance the developer experience.</item>
    /// <item>It is strongly suggested the request and response are also serializable for distributed application scenario.</item>
    /// </list>
    /// </remarks>
    /// <typeparam name="TResponse">The type of response expected from the mediator.</typeparam>
    public interface IRequest<TResponse>
    {
        // Marker interface
    }
}
