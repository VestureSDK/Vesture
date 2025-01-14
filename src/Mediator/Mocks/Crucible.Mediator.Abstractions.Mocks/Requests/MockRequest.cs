using Crucible.Mediator.Mocks.Invocation;
using Crucible.Mediator.Requests;

namespace Crucible.Mediator.Mocks.Requests
{
    /// <summary>
    /// Defines a mock <see cref="IRequest{TResponse}"/> contract.
    /// </summary>
    public class MockRequest : MockContract, IRequest<MockResponse>
    {

    }
}
