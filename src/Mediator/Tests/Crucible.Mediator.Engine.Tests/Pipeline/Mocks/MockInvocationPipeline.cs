using Crucible.Mediator.Engine.Pipeline;
using Crucible.Mediator.Engine.Pipeline.Context;
using Crucible.Mediator.Engine.Pipeline.Internal;
using Crucible.Mediator.Engine.Pipeline.Resolvers;
using Crucible.Mediator.Engine.Pipeline.Strategies;
using Crucible.Mediator.Invocation;
using Moq;

namespace Crucible.Mediator.Engine.Tests.Pipeline.Mocks
{
    public class MockInvocationPipeline<TRequest, TResponse> : IInvocationPipeline<TResponse>
    {
        public Mock<IInvocationPipeline<TResponse>> Mock { get; } = new();

        public Type Request
        {
            get => _inner.Request;
            set => Mock.SetupGet(m => m.Request).Returns(value);
        }

        public Type Response
        {
            get => _inner.Response;
            set => Mock.SetupGet(m => m.Response).Returns(value);
        }

        private IInvocationPipeline<TResponse> _inner => Mock.Object;

        public MockInvocationPipeline()
        {
            Request = typeof(TRequest);
            Response = typeof(TResponse);
        }

        public MockInvocationPipeline(
            IInvocationContextFactory contextFactory,
            IInvocationComponentResolver<IPrePipelineMiddleware> preInvocationPipelineMiddlewareResolver,
            IEnumerable<IMiddlewareInvocationPipelineItem> middlewares,
            IInvocationComponentResolver<IPreHandlerMiddleware> preHandlerMiddlewareResolver,
            IInvocationHandlerStrategy<TRequest, TResponse> handlerStrategy)
            : this()
        {
            Mock
                .Setup(m => m.HandleAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .Returns<object, CancellationToken>(async (request, cancellationToken) =>
                {
                    var context = contextFactory.CreateContextForRequest<TRequest, TResponse>(request);
                    var _chainOfResponsibility = CreateChainOfresponsibility(
                        preInvocationPipelineMiddlewareResolver,
                        middlewares,
                        preHandlerMiddlewareResolver,
                        handlerStrategy
                    );
                    await _chainOfResponsibility.Invoke(context, cancellationToken);
                    return context;
                });
        }

        private Func<IInvocationContext<TRequest, TResponse>, CancellationToken, Task> CreateChainOfresponsibility(
            IInvocationComponentResolver<IPrePipelineMiddleware> preInvocationPipelineMiddlewareResolver,
            IEnumerable<IMiddlewareInvocationPipelineItem> _middlewares,
            IInvocationComponentResolver<IPreHandlerMiddleware> preHandlerMiddlewareResolver,
            IInvocationHandlerStrategy<TRequest, TResponse> handlerStrategy)
        {
            var middlewares = new List<IInvocationMiddleware<TRequest, TResponse>>();

            var contextType = typeof(IInvocationContext<TRequest, TResponse>);
            foreach (var middleware in _middlewares.OrderBy(m => m.Order))
            {
                if (middleware.IsApplicable(contextType))
                {
                    middlewares.Add((IInvocationMiddleware<TRequest, TResponse>)middleware);
                }
            }

            Func<IInvocationContext<TRequest, TResponse>, CancellationToken, Task> chain = (ctx, ct) =>
            {
                var preHandlerMiddleware = preHandlerMiddlewareResolver.ResolveComponent();
                return preHandlerMiddleware.HandleAsync((IInvocationContext<object, object>)ctx, (t) => handler(ctx, t), ct);
            };

            // Build the chain of responsibility and return the new root func.
            for (var i = middlewares.Count - 1; i >= 0; i--)
            {
                var nextMiddleware = chain;
                var item = middlewares[i];
                chain = (ctx, ct) => item.HandleAsync(ctx, (t) => nextMiddleware.Invoke(ctx, t), ct);
            }

            var next = chain;
            chain = (ctx, ct) =>
            {
                var preHandlerMiddleware = preInvocationPipelineMiddlewareResolver.ResolveComponent();
                return preHandlerMiddleware.HandleAsync((IInvocationContext<object, object>)ctx, (t) => next.Invoke(ctx, t), ct);
            };

            return chain!;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Task handler(IInvocationContext<TRequest, TResponse> ctx, CancellationToken ct) => handlerStrategy.HandleAsync(ctx, null, ct);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        public Task<IInvocationContext<TResponse>> HandleAsync(object request, CancellationToken cancellationToken) => _inner.HandleAsync(request, cancellationToken);
    }
}
