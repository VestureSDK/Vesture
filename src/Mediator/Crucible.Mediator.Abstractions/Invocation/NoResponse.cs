using Crucible.Mediator.Commands;
using Crucible.Mediator.Events;

namespace Crucible.Mediator.Invocation
{
    /// <summary>
    /// <para>
    /// <see cref="System.Type"/> used as a response placeholder 
    /// when handling a <see cref="ICommand"/> or <see cref="IEvent"/> via a <see cref="IMediator"/>.
    /// </para>
    /// <para>
    /// The <see cref="NoResponse"/> type should be used when 
    /// implementing <see cref="IInvocationMiddleware{TRequest, TResponse}"/> for
    /// either any <see cref="ICommand"/> or <see cref="IEvent"/> contracts.
    /// </para>
    /// </summary>
    /// <seealso cref="CommandResponse"/>
    /// <seealso cref="EventResponse"/>
    /// <seealso cref="InvocationMiddleware{TRequest, TResponse}"/>
    /// <seealso cref="IMediator"/>
    public class NoResponse
    {
        /// <summary>
        /// The <see cref="System.Type"/> of <see cref="NoResponse"/>.
        /// </summary>
        /// <remarks>
        /// This property is for ease of access and avoid typing <c>typeof(NoResponse)</c> in multiple places.
        /// </remarks>
        /// <seealso cref="CommandResponse.Type"/>
        /// <seealso cref="EventResponse.Type"/>
        public static readonly Type Type = typeof(NoResponse);
    }
}
