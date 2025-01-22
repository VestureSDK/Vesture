using Ingot.Mediator.Engine.Mocks.Pipeline.Context;
using Ingot.Mediator.Engine.Mocks.Pipeline.Strategies;
using Ingot.Mediator.Engine.Pipeline;
using Ingot.Mediator.Engine.Pipeline.Context;
using Ingot.Mediator.Engine.Pipeline.Internal;
using Ingot.Mediator.Engine.Pipeline.Strategies;
using Ingot.Mediator.Invocation;
using Ingot.Mediator.Mocks.Invocation;
using Moq;

namespace Ingot.Mediator.Engine.Mocks.Pipeline
{
    public abstract class MockInvocationPipeline : IInvocationPipeline
    {
        public abstract Type RequestType { get; set; }

        public abstract Type ResponseType { get; set; }

        public abstract IPrePipelineMiddleware PrePipelineMiddleware { get; set; }

        public abstract IPreHandlerMiddleware PreHandlerMiddleware { get; set; }

        public abstract ICollection<IMiddlewareInvocationPipelineItem> Middlewares { get; set; }
    }

    public class MockInvocationPipeline<TRequest, TResponse>
        : MockInvocationPipeline,
            IInvocationPipeline<TResponse>
    {
        public Mock<IInvocationPipeline<TResponse>> Mock { get; } = new();

        public override Type RequestType
        {
            get => _inner.RequestType;
            set => Mock.SetupGet(m => m.RequestType).Returns(value);
        }

        public override Type ResponseType
        {
            get => _inner.ResponseType;
            set => Mock.SetupGet(m => m.ResponseType).Returns(value);
        }

        private IInvocationPipeline<TResponse> _inner => Mock.Object;

        private TRequest _request;

        public TRequest Request
        {
            get => _request;
            set
            {
                _request = value;
                _managedContext.Request = value!;
            }
        }

        private TResponse _response;

        public TResponse Response
        {
            get => _response;
            set
            {
                _response = value;
                _managedHandlerStrategy.Response = value!;
            }
        }

        private readonly MockInvocationContext<TRequest, TResponse> _managedContext;

        private IInvocationContext<TRequest, TResponse>? _context;

        public IInvocationContext<TRequest, TResponse> Context
        {
            get => _context ?? _managedContext;
            set
            {
                _context = value;
                _managedContextFactory.Context = value;
            }
        }

        private readonly MockInvocationContextFactory<TRequest, TResponse> _managedContextFactory;

        private IInvocationContextFactory? _contextFactory;

        public IInvocationContextFactory ContextFactory
        {
            get => _contextFactory ?? _managedContextFactory;
            set => _contextFactory = value;
        }

        private readonly MockPrePipelineMiddleware _managedPrePipelineMiddleware;

        private IPrePipelineMiddleware? _prePipelineMiddleware;

        public override IPrePipelineMiddleware PrePipelineMiddleware
        {
            get => _prePipelineMiddleware ?? _managedPrePipelineMiddleware;
            set => _prePipelineMiddleware = value;
        }

        private readonly MockPreHandlerMiddleware _managedPreHandlerMiddleware;

        private IPreHandlerMiddleware? _preHandlerMiddleware;

        public override IPreHandlerMiddleware PreHandlerMiddleware
        {
            get => _preHandlerMiddleware ?? _managedPreHandlerMiddleware;
            set => _preHandlerMiddleware = value;
        }

        private ICollection<IMiddlewareInvocationPipelineItem> _middlewares = [];

        public override ICollection<IMiddlewareInvocationPipelineItem> Middlewares
        {
            get => _middlewares;
            set => _middlewares = value ?? [];
        }

        public IInvocationHandler<TRequest, TResponse> Handler
        {
            get => _managedHandlerStrategy.Handler;
            set => _managedHandlerStrategy.Handler = value;
        }

        public IInvocationHandler<TRequest, TResponse> OtherHandler
        {
            get => _managedHandlerStrategy.OtherHandler;
            set => _managedHandlerStrategy.OtherHandler = value;
        }

        public ICollection<IInvocationHandler<TRequest, TResponse>> Handlers
        {
            get => _managedHandlerStrategy.Handlers;
            set => _managedHandlerStrategy.Handlers = value;
        }

        private readonly MockInvocationHandlerStrategy<TRequest, TResponse> _managedHandlerStrategy;

        private IInvocationHandlerStrategy<TRequest, TResponse>? _handlerStrategy;

        public IInvocationHandlerStrategy<TRequest, TResponse> HandlerStrategy
        {
            get => _handlerStrategy ?? _managedHandlerStrategy;
            set => _handlerStrategy = value;
        }

        public MockInvocationPipeline()
        {
            _request = default!;
            _response = default!;

            _managedContext = new() { Request = _request };

            _managedContextFactory = new() { Context = _managedContext };

            _managedPrePipelineMiddleware = new();
            _managedPreHandlerMiddleware = new();

            _managedHandlerStrategy = new() { Response = Response };

            RequestType = typeof(TRequest);
            ResponseType = typeof(TResponse);

            Mock.Setup(m => m.HandleAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .Returns<object, CancellationToken>(
                    async (request, cancellationToken) =>
                    {
                        var context = ContextFactory.CreateContextForRequest<TRequest, TResponse>(
                            request
                        );
                        var chainOfResponsibility = CreateMockChainOfresponsibility();
                        await chainOfResponsibility.Invoke(context, cancellationToken);
                        return context;
                    }
                );
        }

        public Func<
            IInvocationContext<TRequest, TResponse>,
            CancellationToken,
            Task
        > CreateMockChainOfresponsibility()
        {
            var middlewares = new List<IInvocationMiddleware<TRequest, TResponse>>();

            var contextType = typeof(IInvocationContext<TRequest, TResponse>);
            foreach (var middleware in Middlewares.OrderBy(m => m.Order))
            {
                if (middleware.IsApplicable(contextType))
                {
                    middlewares.Add((IInvocationMiddleware<TRequest, TResponse>)middleware);
                }
            }

            Func<IInvocationContext<TRequest, TResponse>, CancellationToken, Task> chain = (
                ctx,
                ct
            ) =>
            {
                return PreHandlerMiddleware.HandleAsync(
                    (IInvocationContext<object, object>)ctx,
                    (t) => handler(ctx, t),
                    ct
                );
            };

            // Build the chain of responsibility and return the new root func.
            for (var i = middlewares.Count - 1; i >= 0; i--)
            {
                var nextMiddleware = chain;
                var item = middlewares[i];
                chain = (ctx, ct) =>
                    item.HandleAsync(ctx, (t) => nextMiddleware.Invoke(ctx, t), ct);
            }

            var next = chain;
            chain = (ctx, ct) =>
            {
                return PrePipelineMiddleware.HandleAsync(
                    (IInvocationContext<object, object>)ctx,
                    (t) => next.Invoke(ctx, t),
                    ct
                );
            };

            return chain!;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Task handler(IInvocationContext<TRequest, TResponse> ctx, CancellationToken ct) =>
                HandlerStrategy.HandleAsync(ctx, null, ct);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        public Task<IInvocationContext<TResponse>> HandleAsync(
            object request,
            CancellationToken cancellationToken
        ) => _inner.HandleAsync(request, cancellationToken);
    }
}
