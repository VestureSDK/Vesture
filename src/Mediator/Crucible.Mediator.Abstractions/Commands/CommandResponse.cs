using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Commands
{
    /// <summary>
    /// Type used as a <see cref="NoResponse"/> for <see cref="ICommand"/>.
    /// </summary>
    /// <remarks>
    /// This is used for middlewares and invocation context to determine if it relates to a <see cref="ICommand"/>.
    /// </remarks>
    public class CommandResponse : NoResponse
    {
        /// <summary>
        /// The <see cref="System.Type"/> of <see cref="CommandResponse"/>.
        /// </summary>
        /// <remarks>
        /// This property is for ease of access and ensure the code base is lean.
        /// </remarks>
        public static readonly new Type Type = typeof(CommandResponse);
    }
}
