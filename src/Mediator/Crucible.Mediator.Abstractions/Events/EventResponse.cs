using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Events
{
    /// <summary>
    /// <see cref="System.Type"/> used as a response placeholder when handling an <see cref="IEvent"/> via a <see cref="IMediator"/>.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>Allows to determine if an <see cref="IInvocationContext{TRequest, TResponse}"/> relates to an <see cref="IEvent"/>.</item>
    /// <item>You can use <see cref="EventResponse"/> when registering an <see cref="IInvocationMiddleware{TRequest, TResponse}"/> to handle any <see cref="IEvent"/>.</item>
    /// </list>
    /// </remarks>
    /// <seealso cref="NoResponse"/>
    public class EventResponse : NoResponse
    {
        /// <summary>
        /// The <see cref="System.Type"/> of <see cref="EventResponse"/>.
        /// </summary>
        /// <remarks>
        /// This property is for ease of access and avoid typing <c>typeof(EventResponse)</c> in multiple places.
        /// </remarks>
        /// <seealso cref="NoResponse.Type"/>
        public static new readonly Type Type = typeof(EventResponse);
    }
}
