using Crucible.Mediator.Requests;

namespace Crucible.Mediator.Tests.Requests
{
    public class MockRequestHandler : RequestHandler<TestMediatorRequest, TestMediatorResponse>
    {
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
        public Func<TestMediatorRequest, TestMediatorResponse> MockExecute { get; set; } = (_) => throw new NotSupportedException();

        public Func<TestMediatorRequest, CancellationToken, Task<TestMediatorResponse>> MockExecuteAsync { get; set; }
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.

        public MockRequestHandler()
        {
            MockExecuteAsync = (r, _) => Task.FromResult(MockExecute(r));
        }

        protected override Task<TestMediatorResponse> ExecuteAsync(TestMediatorRequest request, CancellationToken cancellationToken)
        {
            return MockExecuteAsync.Invoke(request, cancellationToken);
        }
    }
}
