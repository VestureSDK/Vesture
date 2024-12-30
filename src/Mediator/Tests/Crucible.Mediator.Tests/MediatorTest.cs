namespace Crucible.Mediator.Tests
{
    public class MediatorTest : MediatorDiTestBase<IMediator>
    {
        protected IMediator Mediator => Sut;
    }
}
