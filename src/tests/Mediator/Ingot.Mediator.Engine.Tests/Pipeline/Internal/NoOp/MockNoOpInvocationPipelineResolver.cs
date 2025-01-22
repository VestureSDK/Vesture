using Ingot.Mediator.Engine.Mocks.Pipeline;
using Ingot.Mediator.Engine.Pipeline;
using Ingot.Mediator.Engine.Pipeline.Internal.NoOp;
using Ingot.Mediator.Events;
using Ingot.Mediator.Mocks.Invocation;
using Moq;

namespace Ingot.Mediator.Engine.Tests.Pipeline.Internal.NoOp
{
    public class MockNoOpInvocationPipelineResolver : INoOpInvocationPipelineResolver
    {
        private readonly HashSet<Type> _setupResponses = [];

        public Mock<INoOpInvocationPipelineResolver> Mock { get; } = new();

        private INoOpInvocationPipelineResolver _inner => Mock.Object;

        public IInvocationPipeline<TResponse> ResolveNoOpInvocationPipeline<TResponse>()
        {
            if (_setupResponses.Add(typeof(TResponse)))
            {
                var pipeline = new MockInvocationPipeline<object, TResponse>();
                var handler = new MockInvocationHandler<object, TResponse>();

                handler
                    .Mock.Setup(m =>
                        m.HandleAsync(It.IsAny<object>(), It.IsAny<CancellationToken>())
                    )
                    .Returns<object, CancellationToken>(
                        (request, _) =>
                        {
                            if (typeof(TResponse) != EventResponse.Type)
                            {
                                throw new KeyNotFoundException(
                                    $"No relevant invocation pipeline found for contract '??? -> {typeof(TResponse).Name}'."
                                );
                            }

                            return Task.FromResult<TResponse>(default!);
                        }
                    );

                pipeline.Handlers.Add(handler);

                Mock.Setup(m => m.ResolveNoOpInvocationPipeline<TResponse>()).Returns(pipeline);
            }

            return _inner.ResolveNoOpInvocationPipeline<TResponse>();
        }
    }
}
