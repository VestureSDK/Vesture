namespace Crucible.Mediator.Commands
{
    /// <summary>
    /// Defines a command not returning a reponse when invoked via a mediator.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>
    ///     <term>&#9989; DO mark your commands with <c>ICommand</c></term>
    ///     <description><para>To enhance the developer experience, ensure your commands implement the <c>ICommand</c> marker interface.</para></description>
    /// </item>
    /// <item>
    ///     <term>&#9989; DO make your command serializable</term>
    ///     <description><para>Ensure your command class is serializable so you can easily re-use it via with a distributed <c>IMediator</c>.</para></description>
    /// </item>
    /// <item>
    ///     <term>&#11093; AVOID use <c>ICommand</c> for events</term>
    ///     <description><para>Ensure to be familiar with the semantics and use cases of `ICommand` and <c>IEvent</c> to avoid implementing the wrong marker.</para></description>
    /// </item>
    /// <item>
    ///     <term>&#10060; DON'T use <c>ICommand</c> for events</term>
    ///     <description><para>Ensure to be familiar with the semantics and use cases of `ICommand` and <c>IEvent</c> to avoid implementing the wrong marker.</para></description>
    /// </item>
    /// </list>
    /// </remarks>
    public interface ICommand
    {
        // Marker interface
    }
}
