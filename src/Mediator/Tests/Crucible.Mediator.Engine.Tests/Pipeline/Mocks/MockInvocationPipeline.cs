using Crucible.Mediator.Abstractions.Tests.Invocation.Mocks;
using Crucible.Mediator.Engine.Pipeline;
using Crucible.Mediator.Engine.Pipeline.Context;
using Crucible.Mediator.Engine.Pipeline.Internal;
using Crucible.Mediator.Engine.Pipeline.Resolvers;
using Crucible.Mediator.Engine.Pipeline.Strategies;
using Crucible.Mediator.Engine.Tests.Pipeline.Context.Mocks;
using Crucible.Mediator.Engine.Tests.Pipeline.Resolvers.Mocks;
using Crucible.Mediator.Engine.Tests.Pipeline.Strategies.Mocks;
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

        private TRequest _request;

        public TRequest RequestC
        {
            get => _request;
            set
            {
                _request = value;
                _managedContext.Request = value!;
            }
        }

        private TResponse _response;

        public TResponse ResponseC
        {
            get => _response;
            set
            {
                _response = value;
                _managedHandler.Response = value!;
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

        public IPrePipelineMiddleware PrePipelineMiddleware
        {
            get => _prePipelineMiddleware ?? _managedPrePipelineMiddleware;
            set
            {
                _prePipelineMiddleware = value;
                _managedPrePipelineMiddlewareResolver.Component = value ?? _managedPrePipelineMiddleware;
            }
        }

        private readonly MockInvocationComponentResolver<IPrePipelineMiddleware> _managedPrePipelineMiddlewareResolver;

        private IInvocationComponentResolver<IPrePipelineMiddleware>? _prePipelineMiddlewareResolver;

        public IInvocationComponentResolver<IPrePipelineMiddleware> PrePipelineMiddlewareResolver
        {
            get => _prePipelineMiddlewareResolver ?? _managedPrePipelineMiddlewareResolver;
            set => _prePipelineMiddlewareResolver = value;
        }

        private readonly MockPreHandlerMiddleware _managedPreHandlerMiddleware;

        private IPreHandlerMiddleware? _preHandlerMiddleware;

        public IPreHandlerMiddleware PreHandlerMiddleware
        {
            get => _preHandlerMiddleware ?? _managedPreHandlerMiddleware;
            set
            {
                _preHandlerMiddleware = value;
                _managedPreHandlerMiddlewareResolver.Component = value ?? _managedPreHandlerMiddleware;
            }
        }

        private readonly MockInvocationComponentResolver<IPreHandlerMiddleware> _managedPreHandlerMiddlewareResolver;

        private IInvocationComponentResolver<IPreHandlerMiddleware>? _preHandlerMiddlewareResolver;

        public IInvocationComponentResolver<IPreHandlerMiddleware> PreHandlerMiddlewareResolver
        {
            get => _preHandlerMiddlewareResolver ?? _managedPreHandlerMiddlewareResolver;
            set => _preHandlerMiddlewareResolver = value;
        }

        private ICollection<IMiddlewareInvocationPipelineItem> _middlewares = [];

        public ICollection<IMiddlewareInvocationPipelineItem> Middlewares 
        {
            get => _middlewares;
            set => _middlewares = value ?? [];
        }

        private readonly MockInvocationHandler<TRequest, TResponse> _managedHandler;

        private IInvocationHandler<TRequest, TResponse>? _handler;

        public IInvocationHandler<TRequest, TResponse> Handler
        {
            get => _handler ?? _managedHandler;
            set
            {
                _handler = value;
                _managedHandlerResolver.Component = value ?? _managedHandler;
            }
        }

        private readonly MockInvocationComponentResolver<IInvocationHandler<TRequest, TResponse>> _managedHandlerResolver;

        private IInvocationComponentResolver<IInvocationHandler<TRequest, TResponse>>? _handlerResolver;

        public IInvocationComponentResolver<IInvocationHandler<TRequest, TResponse>> HandlerResolver
        {
            get => _handlerResolver ?? _managedHandlerResolver;
            set
            {
                _handlerResolver = value;
                _managedHandlerStrategy.Resolver = value ?? _managedHandlerResolver;
            }
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

            _managedContext = new()
            {
                Request = _request,
            };

            _managedContextFactory = new()
            {
                Context = _managedContext,
            };

            _managedPrePipelineMiddleware = new();
            _managedPrePipelineMiddlewareResolver = new()
            {
                Component = _managedPrePipelineMiddleware
            };

            _managedPreHandlerMiddleware = new();
            _managedPreHandlerMiddlewareResolver = new()
            {
                Component = _managedPreHandlerMiddleware
            };

            _managedHandler = new()
            {
                Response = _response,
            };
            _managedHandlerResolver = new()
            {
                Component = _managedHandler
            };

            _managedHandlerStrategy = new()
            {
                Resolver = _managedHandlerResolver
            };

            Request = typeof(TRequest);
            Response = typeof(TResponse);

            Mock
                .Setup(m => m.HandleAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .Returns<object, CancellationToken>(async (request, cancellationToken) =>
                {
                    var context = ContextFactory.CreateContextForRequest<TRequest, TResponse>(request);
                    var chainOfResponsibility = CreateMockChainOfresponsibility();
                    await chainOfResponsibility.Invoke(context, cancellationToken);
                    return context;
                });
        }

        public Func<IInvocationContext<TRequest, TResponse>, CancellationToken, Task> CreateMockChainOfresponsibility()
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

            Func<IInvocationContext<TRequest, TResponse>, CancellationToken, Task> chain = (ctx, ct) =>
            {
                var preHandlerMiddleware = PreHandlerMiddlewareResolver.ResolveComponent();
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
                var preHandlerMiddleware = PrePipelineMiddlewareResolver.ResolveComponent();
                return preHandlerMiddleware.HandleAsync((IInvocationContext<object, object>)ctx, (t) => next.Invoke(ctx, t), ct);
            };

            return chain!;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Task handler(IInvocationContext<TRequest, TResponse> ctx, CancellationToken ct) => HandlerStrategy.HandleAsync(ctx, null, ct);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        public Task<IInvocationContext<TResponse>> HandleAsync(object request, CancellationToken cancellationToken) => _inner.HandleAsync(request, cancellationToken);
    }
}
