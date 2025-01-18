using System.Collections;
using System.Diagnostics;
using Ingot.Mediator.Engine.Pipeline.Internal;
using Ingot.Mediator.Invocation;
using Microsoft.Extensions.Logging;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
namespace Ingot.Mediator.Engine.Pipeline.Extensions
{
    // EventID:         0->65535
    // Ingot:           ## {component} + ### {event}
    // Mediator:        10###
    // Mediator.Engine: 10001 -> 10999
    internal static class MediatorLoggerExtensions
    {
        private static EventId Event(int id, string? name = null)
        {
            ThrowIfIdInvalid(id);
            return new EventId(10000 + id, name);
        }

        [Conditional("DEBUG")]
        private static void ThrowIfIdInvalid(int id)
        {
            if (id < 1 || id > 999)
            {
                throw new ArgumentException("An ingot compoent's event id must be in range [1, 99]", nameof(id));
            }
        }

        private static readonly Action<ILogger, IEnumerable<string>, Exception> s_invocationPipelineCompiled =
            LoggerMessage.Define<IEnumerable<string>>(LogLevel.Trace,
            Event(1, nameof(InvocationPipelinesCached)),
            "Invocation pipelines cached {Pipelines}");

        internal static void InvocationPipelinesCached(this ILogger logger, IEnumerable<(Type RequestType, Type ResponseType)> registeredPipelines)
        {
            if (!logger.IsEnabled(LogLevel.Trace))
            {
                // Early exit to avoid extra compute
                return;
            }

            s_invocationPipelineCompiled(logger, registeredPipelines.Select(p => $"{p.RequestType} -> {p.ResponseType}"), null);
        }

        private static readonly Action<ILogger, Type, Type, object, Exception> s_invocationPipelineFound =
            LoggerMessage.Define<Type, Type, object>(LogLevel.Debug,
            Event(2, nameof(InvocationPipelineFound)),
            "Invocation pipeline found ({RequestType} -> {ResponseType}) for contract {Contract}");

        internal static void InvocationPipelineFound<TResponse>(this ILogger logger, object request)
        {
            if (!logger.IsEnabled(LogLevel.Debug))
            {
                // Early exit to avoid extra compute
                return;
            }

            s_invocationPipelineFound(logger, request.GetType(), typeof(TResponse), request, null);
        }

        private static readonly Action<ILogger, Type, Type, object, Exception> s_invocationPipelineNotFound =
            LoggerMessage.Define<Type, Type, object>(LogLevel.Debug,
            Event(3, nameof(InvocationPipelineNotFound)),
            "Invocation pipeline not found ({RequestType} -> {ResponseType}) for contract {Contract}; Falling back to NoOp invocation pipeline");

        internal static void InvocationPipelineNotFound<TResponse>(this ILogger logger, object request)
        {
            if (!logger.IsEnabled(LogLevel.Debug))
            {
                // Early exit to avoid extra compute
                return;
            }

            s_invocationPipelineNotFound(logger, request.GetType(), typeof(TResponse), request, null);
        }

        private static readonly Action<ILogger, Type, Type, Type, int, Exception> s_invocationPipelineChainMiddlewareMatches =
            LoggerMessage.Define<Type, Type, Type, int>(LogLevel.Trace,
            Event(4, nameof(InvocationPipelineChainMiddlewareMatches)),
            "{Middleware} matches ({RequestType} -> {ResponseType}) and will be added to the invocation pipeline chain at index {Index}");

        internal static void InvocationPipelineChainMiddlewareMatches<TRequest, TResponse>(this ILogger logger, IMiddlewareInvocationPipelineItem middleware, IList middlewares)
        {
            if (!logger.IsEnabled(LogLevel.Trace))
            {
                // Early exit to avoid extra compute
                return;
            }

            s_invocationPipelineChainMiddlewareMatches(logger, middleware.MiddlewareType, typeof(TRequest), typeof(TResponse), middlewares.Count, null);
        }


        private static readonly Action<ILogger, Type, Type, Type, Exception> s_invocationPipelineChainMiddlewareDoesNotMatch =
            LoggerMessage.Define<Type, Type, Type>(LogLevel.Trace,
            Event(5, nameof(InvocationPipelineChainMiddlewareDoesNotMatch)),
            "{Middleware} does not match ({RequestType} -> {ResponseType}) and will not be added to the invocation pipeline chain");

        internal static void InvocationPipelineChainMiddlewareDoesNotMatch<TRequest, TResponse>(this ILogger logger, IMiddlewareInvocationPipelineItem middleware)
        {
            if (!logger.IsEnabled(LogLevel.Trace))
            {
                // Early exit to avoid extra compute
                return;
            }

            s_invocationPipelineChainMiddlewareDoesNotMatch(logger, middleware.MiddlewareType, typeof(TRequest), typeof(TResponse), null);
        }

        private static readonly Action<ILogger, Type, Type, int, IEnumerable<Type>, Exception> s_invocationPipelineChainCreated =
            LoggerMessage.Define<Type, Type, int, IEnumerable<Type>>(LogLevel.Trace,
            Event(6, nameof(InvocationPipelineChainCreated)),
            "Invocation pipeline chain ({RequestType} -> {ResponseType}) created with {MiddlewaresCount} middlewares {Middlewares}");

        internal static void InvocationPipelineChainCreated<TRequest, TResponse>(this ILogger logger, IList<IMiddlewareInvocationPipelineItem> middlewares)
        {
            if (!logger.IsEnabled(LogLevel.Trace))
            {
                // Early exit to avoid extra compute
                return;
            }

            s_invocationPipelineChainCreated(logger, typeof(TRequest), typeof(TResponse), middlewares.Count, middlewares.Select(m => m.MiddlewareType), null);
        }

        private static readonly Action<ILogger, Type, Type?, Exception> s_prePipelineAndHandlerMiddlewareUnhandledException =
            LoggerMessage.Define<Type, Type?>(LogLevel.Debug,
            Event(7, nameof(PrePipelineAndHandlerMiddlewareUnhandledException)),
            "Invocation pipeline chain ({RequestType} -> {ResponseType}) encountered an unhandled exception; an error has been added to the current invocation context");

        internal static void PrePipelineAndHandlerMiddlewareUnhandledException(this ILogger logger, IInvocationContext context, Exception exception)
        {
            if (!logger.IsEnabled(LogLevel.Debug))
            {
                // Early exit to avoid extra compute
                return;
            }

            s_prePipelineAndHandlerMiddlewareUnhandledException(logger, context.RequestType, context.ResponseType, exception);
        }

        private static readonly Action<ILogger, Type, Type?, Exception> s_noHandlersRegisteredException =
            LoggerMessage.Define<Type, Type?>(LogLevel.Debug,
            Event(8, nameof(NoHandlersRegisteredException)),
            "Invocation pipeline chain ({RequestType} -> {ResponseType}) does not have the required handler; an error has been added to the current invocation context");

        internal static void NoHandlersRegisteredException(this ILogger logger, IInvocationContext context, Exception exception)
        {
            if (!logger.IsEnabled(LogLevel.Debug))
            {
                // Early exit to avoid extra compute
                return;
            }

            s_noHandlersRegisteredException(logger, context.RequestType, context.ResponseType, exception);
        }

        private static readonly Action<ILogger, Type, Type, Type, Exception> s_invokingHandler =
            LoggerMessage.Define<Type, Type, Type>(LogLevel.Debug,
            Event(9, nameof(InvokingHandler)),
            "Invoking {Handler} ({RequestType} -> {ResponseType})");

        internal static void InvokingHandler<TRequest, TResponse>(this ILogger logger, IInvocationHandler<TRequest, TResponse> handler)
        {
            if (!logger.IsEnabled(LogLevel.Debug))
            {
                // Early exit to avoid extra compute
                return;
            }

            s_invokingHandler(logger, typeof(TRequest), typeof(TResponse), handler.GetType(), null);
        }

        private static readonly Action<ILogger, Type, Type, Type, Exception> s_settingHandlerResultInContext =
            LoggerMessage.Define<Type, Type, Type>(LogLevel.Trace,
            Event(9, nameof(InvokedHandler)),
            "Invoked {Handler} ({RequestType} -> {ResponseType}); response has been set into the current invocation context");

        internal static void InvokedHandler<TRequest, TResponse>(this ILogger logger, IInvocationHandler<TRequest, TResponse> handler)
        {
            if (!logger.IsEnabled(LogLevel.Trace))
            {
                // Early exit to avoid extra compute
                return;
            }

            s_settingHandlerResultInContext(logger, typeof(TRequest), typeof(TResponse), handler.GetType(), null);
        }
    }
}
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
