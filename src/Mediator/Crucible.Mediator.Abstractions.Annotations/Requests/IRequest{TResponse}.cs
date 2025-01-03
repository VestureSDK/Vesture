using System.Runtime.InteropServices;

namespace Crucible.Mediator.Requests
{
    /// <summary>
    /// Defines a request returning a <typeparamref name="TResponse"/> when invoked via a mediator.
    /// </summary>
    /// <remarks>
    /// &#128161; For more informartion about how to use <see cref="IRequest{TResponse}"/>, 
    /// kindly see <see href="${DOC_BASE_URL}/References/Crucible.Mediator.Commands.ICommand.html">the documentation</see>.
    /// </remarks>
    /// <typeparam name="TResponse">The type of response expected from the mediator.</typeparam>
    public interface IRequest<TResponse>
    {
        // Marker interface
    }
}
