using Vesture.Mediator.Mocks.Invocation;
using Vesture.Mediator.Requests;

namespace Vesture.Mediator.Mocks.Requests
{
    /// <summary>
    /// Defines a mock <see cref="IRequest{TResponse}"/> contract.
    /// </summary>
    public class MockRequest : MockContract, IRequest<MockResponse> { }
}
