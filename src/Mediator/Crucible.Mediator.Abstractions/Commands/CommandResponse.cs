using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Commands
{
    /// <summary>
    /// <para>
    /// <see cref="System.Type"/> used as a response placeholder 
    /// when handling a <see cref="ICommand"/> via a <see cref="IMediator"/>.
    /// </para>
    /// <para>
    /// The <see cref="CommandResponse"/> type should be used when 
    /// implementing <see cref="IInvocationMiddleware{TRequest, TResponse}"/> for
    /// either all or specific <see cref="ICommand"/> contracts.
    /// </para>
    /// </summary>
    /// <seealso cref="NoResponse"/>
    /// <seealso cref="InvocationMiddleware{TRequest, TResponse}"/>
    /// <seealso cref="IMediator"/>
    public class CommandResponse : NoResponse
    {
        /// <summary>
        /// The <see cref="System.Type"/> of <see cref="CommandResponse"/>.
        /// </summary>
        /// <remarks>
        /// This property is for ease of access and avoid typing <c>typeof(CommandResponse)</c> in multiple places.
        /// </remarks>
        /// <seealso cref="NoResponse.Type"/>
        public static new readonly Type Type = typeof(CommandResponse);
    }
}
