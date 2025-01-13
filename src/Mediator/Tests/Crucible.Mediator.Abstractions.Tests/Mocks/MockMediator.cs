using System.Diagnostics.CodeAnalysis;
using Crucible.Mediator.Abstractions.Tests.Invocation.Mocks;
using Crucible.Mediator.Commands;
using Crucible.Mediator.Events;
using Crucible.Mediator.Invocation;
using Crucible.Mediator.Requests;
using Moq;

namespace Crucible.Mediator.Abstractions.Tests.Mocks
{
    public class MockMediator : IMediator
    {
        public Mock<IMediator> Mock { get; } = new Mock<IMediator>();

        private IMediator _inner => Mock.Object;

        public MockMediator()
        {

        }

        private ICollection<Pipeline> _pipelines = [];

        private ICollection<Middleware> _middlewares = [];

        private Task<IInvocationContext<TResponse>> InnerHandleAndCaptureAsync<TResponse>(object contract, CancellationToken cancellationToken)
        {
            if (!TryGetPipeline<TResponse>(contract.GetType(), out var pipeline))
            {
                throw new KeyNotFoundException($"No relevant invocation pipeline found for contract '{contract.GetType().Name} -> {typeof(TResponse).Name}'.");
            }

            return pipeline.HandleAndCaptureAsync(contract, cancellationToken);
        }

        private async Task<TResponse> InnerHandleAsync<TResponse>(object contract, CancellationToken cancellationToken)
        {
            var context = await InnerHandleAndCaptureAsync<TResponse>(contract, cancellationToken).ConfigureAwait(false);
            return context.ThrowIfHasError().GetResponseOrDefault<TResponse>();
        }

        public void AddHandler<TRequest, TResponse>(IInvocationHandler<TRequest, TResponse> handler)
        {
            var pipeline = GetOrCreatePipeline<TRequest, TResponse>();
            pipeline.Handlers.Add(handler);
        }

        public void AddMiddleware<TRequest, TResponse>(IInvocationMiddleware<TRequest, TResponse> middleware)
        {
            _middlewares.Add(new Middleware<TRequest, TResponse>(middleware));
        }

        private bool TryGetPipeline<TResponse>(Type requestType, [NotNullWhen(true)] out Pipeline<TResponse>? pipeline)
        {
            pipeline = null;

            var untypedPipeline = _pipelines.FirstOrDefault(p => p.RequestType == requestType && p.ResponseType == typeof(TResponse));
            if (untypedPipeline is not null)
            {
                pipeline = (Pipeline<TResponse>)untypedPipeline;
                return true;
            }

            return false;
        }

        private bool TryGetPipeline<TRequest, TResponse>([NotNullWhen(true)] out Pipeline<TRequest, TResponse>? pipeline)
        {
            pipeline = null;

            var untypedPipeline = _pipelines.FirstOrDefault(p => p.RequestType == typeof(TRequest) && p.ResponseType == typeof(TResponse));
            if (untypedPipeline is not null)
            {
                pipeline = (Pipeline<TRequest, TResponse>)untypedPipeline;
                return true;
            }

            return false;
        }

        private Pipeline<TRequest, TResponse> GetOrCreatePipeline<TRequest, TResponse>()
        {
            if (!TryGetPipeline<TRequest, TResponse>(out var pipeline))
            {
                pipeline = new Pipeline<TRequest, TResponse>(_middlewares);
                _pipelines.Add(pipeline);

                Mock.Setup(m => m.HandleAndCaptureAsync<TResponse>(It.Is<object>((o, _) => o.GetType() == typeof(TRequest)), It.IsAny<CancellationToken>()))
                    .Returns<object, CancellationToken>((request, cancellationToken) => InnerHandleAndCaptureAsync<TResponse>(request, cancellationToken));

                Mock.Setup(m => m.HandleAsync<TResponse>(It.Is<object>((o, _) => o.GetType() == typeof(TRequest)), It.IsAny<CancellationToken>()))
                    .Returns<object, CancellationToken>((request, cancellationToken) => InnerHandleAsync<TResponse>(request, cancellationToken));

                Mock.Setup(m => m.ExecuteAndCaptureAsync<TResponse>(It.Is<IRequest<TResponse>>((o, _) => o.GetType() == typeof(TRequest)), It.IsAny<CancellationToken>()))
                    .Returns<IRequest<TResponse>, CancellationToken>((request, cancellationToken) => InnerHandleAndCaptureAsync<TResponse>(request, cancellationToken));

                Mock.Setup(m => m.ExecuteAsync<TResponse>(It.Is<IRequest<TResponse>>((o, _) => o.GetType() == typeof(TRequest)), It.IsAny<CancellationToken>()))
                    .Returns<IRequest<TResponse>, CancellationToken>((request, cancellationToken) => InnerHandleAsync<TResponse>(request, cancellationToken));

                Mock.Setup(m => m.InvokeAndCaptureAsync(It.Is<ICommand>((o, _) => o.GetType() == typeof(TRequest)), It.IsAny<CancellationToken>()))
                    .Returns<ICommand, CancellationToken>(async (request, cancellationToken) => await InnerHandleAndCaptureAsync<CommandResponse>(request, cancellationToken));

                Mock.Setup(m => m.InvokeAsync(It.Is<ICommand>((o, _) => o.GetType() == typeof(TRequest)), It.IsAny<CancellationToken>()))
                    .Returns<ICommand, CancellationToken>((request, cancellationToken) => InnerHandleAsync<CommandResponse>(request, cancellationToken));

                Mock.Setup(m => m.PublishAndCaptureAsync(It.Is<IEvent>((o, _) => o.GetType() == typeof(TRequest)), It.IsAny<CancellationToken>()))
                    .Returns<IEvent, CancellationToken>(async (request, cancellationToken) => await InnerHandleAndCaptureAsync<EventResponse>(request, cancellationToken));

                Mock.Setup(m => m.PublishAsync(It.Is<IEvent>((o, _) => o.GetType() == typeof(TRequest)), It.IsAny<CancellationToken>()))
                    .Returns<IEvent, CancellationToken>((request, cancellationToken) => InnerHandleAsync<EventResponse>(request, cancellationToken));
            }

            return pipeline;
        }

        private abstract class Middleware
        {
            public abstract bool IsApplicable(Type contextType);
        }

        private class Middleware<TRequest, TResponse> : Middleware, IInvocationMiddleware<TRequest, TResponse>
        {
            public IInvocationMiddleware<TRequest, TResponse> _inner;

            public Middleware(IInvocationMiddleware<TRequest, TResponse> inner) => _inner = inner;

            public Task HandleAsync(IInvocationContext<TRequest, TResponse> context, Func<CancellationToken, Task> next, CancellationToken cancellationToken) => _inner.HandleAsync(context, next, cancellationToken);
            
            public override bool IsApplicable(Type contextType) => typeof(IInvocationContext<TRequest, TResponse>).IsAssignableFrom(contextType);
        }

        private abstract class Pipeline
        {
            public abstract Type RequestType { get; }

            public abstract Type ResponseType { get; }
        }

        private abstract class Pipeline<TResponse> : Pipeline
        {
            public abstract Task<IInvocationContext<TResponse>> HandleAndCaptureAsync(object contract, CancellationToken cancellationToken);
        }

        private class Pipeline<TRequest, TResponse> : Pipeline<TResponse>
        {
            private IEnumerable<Middleware> _middlewares;

            public Pipeline(IEnumerable<Middleware> middlewares) => _middlewares = middlewares;

            public ICollection<IInvocationHandler<TRequest, TResponse>> Handlers { get; } = [];

            public override Type RequestType => typeof(TRequest);

            public override Type ResponseType => typeof(TResponse);

            public override async Task<IInvocationContext<TResponse>> HandleAndCaptureAsync(object contract, CancellationToken cancellationToken)
            {
                var context = new MockInvocationContext<TRequest, TResponse>
                {
                    Request = contract
                };

                var middlewares = new List<IInvocationMiddleware<TRequest, TResponse>>();

                var contextType = typeof(IInvocationContext<TRequest, TResponse>);
                foreach (var middleware in _middlewares)
                {
                    if (middleware.IsApplicable(contextType))
                    {
                        middlewares.Add((IInvocationMiddleware<TRequest, TResponse>)middleware);
                    }
                }

                Func<CancellationToken, Task> chain = async (ct) =>
                {
                    try
                    {
                        foreach (var handler in Handlers)
                        {
                            var response = await handler.HandleAsync((TRequest)context.Request, cancellationToken);
                            context.SetResponse(response);
                        }
                    }
                    catch (Exception ex)
                    {
                        context.SetError(ex);
                    }
                };

                // Build the chain of responsibility and return the new root func.
                for (var i = middlewares.Count - 1; i >= 0; i--)
                {
                    var nextMiddleware = chain;
                    var item = middlewares[i];
                    chain = (ct) => item.HandleAsync(context, nextMiddleware, ct);
                }

                var next = chain;
                chain = async (ct) =>
                {
                    try
                    {
                        await next.Invoke(ct);
                    }
                    catch (Exception ex)
                    {
                        context.SetError(ex);
                    }
                };

                await chain.Invoke(cancellationToken);

                return context;
            }
        }

        public Task<IInvocationContext<TResponse>> ExecuteAndCaptureAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default) => _inner.ExecuteAndCaptureAsync(request, cancellationToken);
        
        public Task<TResponse> ExecuteAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default) => _inner.ExecuteAsync(request, cancellationToken);
        
        public Task<IInvocationContext<TResponse>> HandleAndCaptureAsync<TResponse>(object contract, CancellationToken cancellationToken = default) => _inner.HandleAndCaptureAsync<TResponse>(contract, cancellationToken);
        
        public Task<TResponse> HandleAsync<TResponse>(object contract, CancellationToken cancellationToken = default) => _inner.HandleAsync<TResponse>(contract, cancellationToken);
        
        public Task<IInvocationContext> InvokeAndCaptureAsync(ICommand command, CancellationToken cancellationToken = default) => _inner.InvokeAndCaptureAsync(command, cancellationToken);
        
        public Task InvokeAsync(ICommand command, CancellationToken cancellationToken = default) => _inner.InvokeAsync(command, cancellationToken);
        
        public Task<IInvocationContext> PublishAndCaptureAsync(IEvent @event, CancellationToken cancellationToken = default) => _inner.PublishAndCaptureAsync(@event, cancellationToken);
        
        public Task PublishAsync(IEvent @event, CancellationToken cancellationToken = default) => _inner.PublishAsync(@event, cancellationToken);
    }
}
