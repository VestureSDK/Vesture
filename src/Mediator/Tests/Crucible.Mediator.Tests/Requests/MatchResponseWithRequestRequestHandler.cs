using Crucible.Mediator.Requests;

namespace Crucible.Mediator.Tests.Requests
{
    public class MatchResponseWithRequestRequestHandler : RequestHandler<TestMediatorRequest, TestMediatorResponse>
    {
        protected override Task<TestMediatorResponse> ExecuteAsync(TestMediatorRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new TestMediatorResponse
            {
                TestProperty = request.TestProperty,
            });
        }
    }
}
