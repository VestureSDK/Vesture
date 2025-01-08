using Crucible.Mediator.Engine.Pipeline.Strategies;
using Crucible.Mediator.Invocation;

namespace Crucible.Mediator.Engine.DependencyInjection
{
    public interface IDependencyInjectionRegistrar
    {
        void RegisterHandler<TRequest, TResponse, THandlerImplementation>(bool singleton = false) 
            where THandlerImplementation : class, IInvocationHandler<TRequest, TResponse>;
        
        void RegisterHandler<TRequest, TResponse>(IInvocationHandler<TRequest, TResponse> handler);
        
        void RegisterHandlerStrategy<TRequest, TResponse, TStrategy>() 
            where TStrategy : class, IInvocationHandlerStrategy<TRequest, TResponse>;
        
        void RegisterHandlerStrategy<TRequest, TResponse>(IInvocationHandlerStrategy<TRequest, TResponse> strategy);
        
        void RegisterMiddleware<TRequest, TResponse, TMiddleware>(int? order = null, bool singleton = false) 
            where TMiddleware : class, IInvocationMiddleware<TRequest, TResponse>;
        
        void RegisterMiddleware<TRequest, TResponse>(IInvocationMiddleware<TRequest, TResponse> middleware, int? order = null);
    }
}
