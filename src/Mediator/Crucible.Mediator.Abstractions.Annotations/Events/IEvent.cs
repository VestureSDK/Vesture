namespace Crucible.Mediator.Events
{
    /// <summary>
    /// Defines an event to be published via a mediator.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>While not necessary to implement it; This marker interface will enhance the developer experience.</item>
    /// <item>It is strongly suggested the event is also serializable for distributed application scenario.</item>
    /// </list>
    /// </remarks>
    public interface IEvent
    {
        // Marker interface
    }
}
