namespace Crucible.Mediator.DependencyInjection
{
    /// <summary>
    /// Defines the <see cref="IMediator"/> configuration.
    /// </summary>
    public class MediatorConfiguration
    {
        /// <summary>
        /// Defines if debug tooling should be added to the <see cref="IMediator"/> pipeline.
        /// </summary>
        public bool Debug { get; set; }
    }
}
