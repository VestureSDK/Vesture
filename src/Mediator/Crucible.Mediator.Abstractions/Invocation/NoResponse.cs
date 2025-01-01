using Crucible.Mediator.Commands;
using Crucible.Mediator.Events;

namespace Crucible.Mediator.Invocation
{
    /// <summary>
    /// <see cref="System.Type"/> used as a response placeholder when handling an <see cref="IEvent"/> or <see cref="ICommand"/> via a <see cref="IMediator"/>.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>Allows to determine if an <see cref="IInvocationContext{TRequest, TResponse}"/> relates to an <see cref="IEvent"/> or <see cref="ICommand"/>.</item>
    /// <item>You can use <see cref="NoResponse"/> when registering an <see cref="IInvocationMiddleware{TRequest, TResponse}"/> to handle any <see cref="IEvent"/> or <see cref="ICommand"/>.</item>
    /// </list>
    /// </remarks>
    /// <seealso cref="CommandResponse"/>
    /// <seealso cref="EventResponse"/>
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
