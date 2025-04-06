using Vesture.Mediator.Invocation;

namespace Vesture.Mediator.Commands
{
    /// <summary>
    /// <para>
    /// The <see cref="CommandResponse"/> is a <see cref="System.Type"/> used as a response placeholder
    /// when handling a <see cref="ICommand"/> via a <see cref="IMediator"/>.
    /// </para>
    /// <para>
    /// The <see cref="CommandResponse"/> type should be used when
    /// implementing an <see cref="IInvocationMiddleware{TRequest, TResponse}"/> for
    /// either all or specific <see cref="ICommand"/> contracts.
    /// </para>
    /// </summary>
    /// <seealso cref="NoResponse"/>
    /// <seealso cref="ICommand"/>
    /// <seealso cref="InvocationMiddleware{TRequest, TResponse}"/>
    /// <seealso cref="IMediator"/>
    public class CommandResponse : NoResponse
    {
        /// <exclude />
        /// <summary>
        /// The <see cref="System.Type"/> of <see cref="CommandResponse"/>.
        /// This field is for ease of access and avoid typing <c>typeof(CommandResponse)</c> in multiple places.
        /// </summary>
        public static new readonly Type Type = typeof(CommandResponse);
    }
}
