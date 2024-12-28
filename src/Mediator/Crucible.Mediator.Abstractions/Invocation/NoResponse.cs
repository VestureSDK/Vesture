using Crucible.Mediator.Commands;
using Crucible.Mediator.Events;

namespace Crucible.Mediator.Invocation
{
    /// <summary>
    /// Internal type used for specific mediator flows.
    /// </summary>
    /// <remarks>
    /// This is used for middlewares and invocation context to determine if it relates to a <see cref="IEvent"/> or <see cref="ICommand"/>.
    /// For a more fine-grained approach, you can use <see cref="EventResponse"/> and <see cref="CommandResponse"/> for
    /// <see cref="IEvent"/> and <see cref="ICommand"/> respectively.
    /// </remarks>
    public class NoResponse
    {
        /// <summary>
        /// The <see cref="System.Type"/> of <see cref="NoResponse"/>.
        /// </summary>
        /// <remarks>
        /// This property is for ease of access and ensure the code base is lean.
        /// </remarks>
        public static readonly Type Type = typeof(NoResponse);
    }
}
