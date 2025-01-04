using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Events
{
    /// <summary>
    /// <para>
    /// The <see cref="EventResponse"/> is a <see cref="System.Type"/> used as a response placeholder 
    /// when handling a <see cref="IEvent"/> via a <see cref="IMediator"/>.
    /// </para>
    /// <para>
    /// The <see cref="EventResponse"/> type should be used when 
    /// implementing an <see cref="IInvocationMiddleware{TRequest, TResponse}"/> for
    /// either all or specific <see cref="IEvent"/> contracts.
    /// </para>
    /// </summary>
    /// <seealso cref="NoResponse"/>
    /// <seealso cref="IEvent"/>
    /// <seealso cref="InvocationMiddleware{TRequest, TResponse}"/>
    /// <seealso cref="IMediator"/>
    public class EventResponse : NoResponse
    {
        /// <summary>
        /// The <see cref="System.Type"/> of <see cref="EventResponse"/>.
        /// This field is for ease of access and avoid typing <c>typeof(EventResponse)</c> in multiple places.
        /// </summary>
        public static new readonly Type Type = typeof(EventResponse);
    }
}
