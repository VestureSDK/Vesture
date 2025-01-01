namespace Crucible.Mediator.Commands
{
    /// <summary>
    /// Defines a command not returning a reponse when invoked via a mediator.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>While not necessary to implement it; This marker interface will enhance the developer experience.</item>
    /// <item>It is strongly suggested the command is also serializable for distributed application scenario.</item>
    /// </list>
    /// </remarks>
    public interface ICommand
    {
        // Marker interface
    }
}
