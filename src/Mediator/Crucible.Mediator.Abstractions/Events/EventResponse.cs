using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Events
{
    /// <summary>
    /// Type used as a <see cref="NoResponse"/> for <see cref="IEvent"/>.
    /// </summary>
    /// <remarks>
    /// This is used for middlewares and invocation context to determine if it relates to a <see cref="IEvent"/>.
    /// </remarks>
    public class EventResponse : NoResponse
    {
        /// <summary>
        /// The <see cref="System.Type"/> of <see cref="EventResponse"/>.
        /// </summary>
        /// <remarks>
        /// This property is for ease of access and ensure the code base is lean.
        /// </remarks>
        public static readonly new Type Type = typeof(EventResponse);
    }
}
