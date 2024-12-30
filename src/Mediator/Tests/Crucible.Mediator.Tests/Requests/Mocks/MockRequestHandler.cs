using Crucible.Mediator.Requests;

namespace Crucible.Mediator.Tests.Requests.Mocks
{
    public class MockRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    {
        public Func<TRequest, TResponse>? MockExecute { get; set; }

        public Func<TRequest, CancellationToken, Task<TResponse>> MockExecuteAsync { get; set; }

        public MockRequestHandler()
        {
            MockExecuteAsync = (r, _) =>
            {
                var exec = MockExecute ?? throw new NotSupportedException();
                return Task.FromResult(exec.Invoke(r));
            };
        }

        public Task<TResponse> ExecuteAsync(TRequest request, CancellationToken cancellationToken = default)
        {
            return MockExecuteAsync(request, cancellationToken);
        }
    }
}
