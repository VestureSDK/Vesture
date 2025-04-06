using Vesture.Mediator.Commands;
using Vesture.Mediator.Events;

namespace Vesture.Mediator.Invocation
{
    /// <summary>
    /// <para>
    /// The <see cref="NoResponse"/> is a <see cref="System.Type"/> used as a response placeholder
    /// when handling a <see cref="ICommand"/> or <see cref="IEvent"/> via a <see cref="IMediator"/>.
    /// </para>
    /// <para>
    /// The <see cref="NoResponse"/> type should be used when
    /// implementing an <see cref="IInvocationMiddleware{TRequest, TResponse}"/> for
    /// either any <see cref="ICommand"/> or <see cref="IEvent"/> contracts.
    /// </para>
    /// </summary>
    /// <seealso cref="CommandResponse"/>
    /// <seealso cref="ICommand"/>
    /// <seealso cref="EventResponse"/>
    /// <seealso cref="IEvent"/>
    /// <seealso cref="InvocationMiddleware{TRequest, TResponse}"/>
    /// <seealso cref="IMediator"/>
    public class NoResponse
    {
        /// <exclude />
        /// <summary>
        /// The <see cref="System.Type"/> of <see cref="NoResponse"/>.
        /// This field is for ease of access and avoid typing <c>typeof(NoResponse)</c> in multiple places.
        /// </summary>
        public static readonly Type Type = typeof(NoResponse);
    }
}
