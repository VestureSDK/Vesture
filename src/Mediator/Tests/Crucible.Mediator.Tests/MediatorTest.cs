namespace Crucible.Mediator.Tests
{
    public class MediatorTest : MediatorDiTestBase<IMediator>
    {
        protected CancellationToken CancellationToken { get; set; } = CancellationToken.None;

        protected IMediator Mediator => Sut;
    }
}
