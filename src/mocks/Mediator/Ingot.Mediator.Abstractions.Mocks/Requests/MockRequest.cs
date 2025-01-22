using Ingot.Mediator.Mocks.Invocation;
using Ingot.Mediator.Requests;

namespace Ingot.Mediator.Mocks.Requests
{
    /// <summary>
    /// Defines a mock <see cref="IRequest{TResponse}"/> contract.
    /// </summary>
    public class MockRequest : MockContract, IRequest<MockResponse> { }
}
