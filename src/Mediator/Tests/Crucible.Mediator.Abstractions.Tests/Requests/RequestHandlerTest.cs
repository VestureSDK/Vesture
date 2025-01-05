using Crucible.Mediator.Requests;

namespace Crucible.Mediator.Abstractions.Tests.Requests
{
    public abstract class RequestHandlerTest<THandler, TRequest, TResult>
        where THandler : IRequestHandler<TRequest, TResult>
        where TRequest : IRequest<TResult>
    {
        protected Lazy<IRequestHandler<TRequest, TResult>> RequestHandlerInitializer;

        protected IRequestHandler<TRequest, TResult> RequestHandler => RequestHandlerInitializer.Value;

        protected abstract IRequestHandler<TRequest, TResult> GetRequestHandler();

        protected RequestHandlerTest()
        {
            RequestHandlerInitializer = new Lazy<IRequestHandler<TRequest, TResult>>(GetRequestHandler);
        }
    }
}
