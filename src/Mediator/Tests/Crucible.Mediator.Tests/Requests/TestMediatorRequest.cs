using Crucible.Mediator.Requests;

namespace Crucible.Mediator.Tests.Requests
{
    public class TestMediatorRequest : IRequest<TestMediatorResponse>
    {
        public string? TestProperty { get; set; }
    }
}
