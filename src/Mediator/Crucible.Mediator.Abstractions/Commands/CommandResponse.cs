using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Commands
{
    /// <summary>
    /// <see cref="System.Type"/> used as a response placeholder when handling a <see cref="ICommand"/> via a <see cref="IMediator"/>.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>Allows to determine if an <see cref="IInvocationContext{TRequest, TResponse}"/> relates to a <see cref="ICommand"/>.</item>
    /// <item>You can use <see cref="CommandResponse"/> when registering an <see cref="IInvocationMiddleware{TRequest, TResponse}"/> to handle any <see cref="ICommand"/>.</item>
    /// </list>
    /// </remarks>
    /// <seealso cref="NoResponse"/>
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
