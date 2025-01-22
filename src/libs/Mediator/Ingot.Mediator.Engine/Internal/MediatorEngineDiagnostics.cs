using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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
    internal static partial class MediatorEngineDiagnostics
    {
        internal const string BaseActivitySourceName = "Ingot.Mediator.Engine.";

        internal static readonly ActivitySource s_mediatorActivitySource = new(
            BaseActivitySourceName + "Mediator"
        );

        internal static readonly ActivitySource s_invocationPipelineActivitySource = new(
            BaseActivitySourceName + "Pipeline"
        );

        internal static readonly ActivitySource s_invocationHandlerActivitySource = new(
            BaseActivitySourceName + "Handler"
        );

        internal const int BaseEventId = 10000;

        [LoggerMessage(
            SkipEnabledCheck = true,
            EventId = BaseEventId + 1,
            EventName = nameof(InvocationPipelinesCached),
            Level = LogLevel.Trace,
            Message = "Invocation pipelines cached {Pipelines}"
        )]
        private static partial void InnerInvocationPipelineCompiled(
            this ILogger logger,
            Exception ex,
            IEnumerable<string> pipelines
        );

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void InvocationPipelinesCached(
            this ILogger logger,
            IEnumerable<(Type RequestType, Type ResponseType)> registeredPipelines
        )
        {
            if (logger.IsEnabled(LogLevel.Trace))
            {
                InnerInvocationPipelineCompiled(
                    logger,
                    ex: null,
                    pipelines: registeredPipelines.Select(p =>
                        $"{p.RequestType} -> {p.ResponseType}"
                    )
                );
            }
        }

        [LoggerMessage(
            SkipEnabledCheck = true,
            EventId = BaseEventId + 2,
            EventName = nameof(InvocationPipelineFound),
            Level = LogLevel.Debug,
            Message = "Invocation pipeline found ({RequestType} -> {ResponseType}) for contract {Contract}"
        )]
        private static partial void InnerInvocationPipelineFound(
            this ILogger logger,
            Exception ex,
            Type requestType,
            Type responseType,
            object contract
        );

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void InvocationPipelineFound<TResponse>(this ILogger logger, object request)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                InnerInvocationPipelineFound(
                    logger,
                    ex: null,
                    requestType: request.GetType(),
                    responseType: typeof(TResponse),
                    contract: request
                );
            }
        }

        [LoggerMessage(
            SkipEnabledCheck = true,
            EventId = BaseEventId + 3,
            EventName = nameof(InvocationPipelineNotFound),
            Level = LogLevel.Debug,
            Message = "Invocation pipeline not found ({RequestType} -> {ResponseType}) for contract {Contract}; Falling back to NoOp invocation pipeline"
        )]
        private static partial void InnerInvocationPipelineNotFound(
            this ILogger logger,
            Exception ex,
            Type requestType,
            Type responseType,
            object contract
        );

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void InvocationPipelineNotFound<TResponse>(
            this ILogger logger,
            object request
        )
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                InnerInvocationPipelineNotFound(
                    logger,
                    ex: null,
                    requestType: request.GetType(),
                    responseType: typeof(TResponse),
                    contract: request
                );
            }
        }

        [LoggerMessage(
            SkipEnabledCheck = true,
            EventId = BaseEventId + 4,
            EventName = nameof(InvocationPipelineChainMiddlewareMatches),
            Level = LogLevel.Trace,
            Message = "{MiddlewareType} matches ({RequestType} -> {ResponseType}) and will be added to the invocation pipeline chain at index {MiddlewareIndex}"
        )]
        private static partial void InnerInvocationPipelineChainMiddlewareMatches(
            this ILogger logger,
            Exception ex,
            Type middlewareType,
            Type requestType,
            Type responseType,
            int middlewareIndex
        );

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void InvocationPipelineChainMiddlewareMatches<TRequest, TResponse>(
            this ILogger logger,
            IMiddlewareInvocationPipelineItem middleware,
            IList middlewares
        )
        {
            if (logger.IsEnabled(LogLevel.Trace))
            {
                InnerInvocationPipelineChainMiddlewareMatches(
                    logger,
                    ex: null,
                    middlewareType: middleware.MiddlewareType,
                    requestType: typeof(TRequest),
                    responseType: typeof(TResponse),
                    middlewareIndex: middlewares.Count
                );
            }
        }

        [LoggerMessage(
            SkipEnabledCheck = true,
            EventId = BaseEventId + 5,
            EventName = nameof(InvocationPipelineChainMiddlewareDoesNotMatch),
            Level = LogLevel.Trace,
            Message = "{MiddlewareType} does not match ({RequestType} -> {ResponseType}) and will not be added to the invocation pipeline chain"
        )]
        private static partial void InnerInvocationPipelineChainMiddlewareDoesNotMatch(
            this ILogger logger,
            Exception ex,
            Type middlewareType,
            Type requestType,
            Type responseType
        );

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void InvocationPipelineChainMiddlewareDoesNotMatch<TRequest, TResponse>(
            this ILogger logger,
            IMiddlewareInvocationPipelineItem middleware
        )
        {
            if (logger.IsEnabled(LogLevel.Trace))
            {
                InnerInvocationPipelineChainMiddlewareDoesNotMatch(
                    logger,
                    ex: null,
                    middlewareType: middleware.MiddlewareType,
                    requestType: typeof(TRequest),
                    responseType: typeof(TResponse)
                );
            }
        }

        [LoggerMessage(
            SkipEnabledCheck = true,
            EventId = BaseEventId + 6,
            EventName = nameof(InvocationPipelineChainCreated),
            Level = LogLevel.Trace,
            Message = "Invocation pipeline chain ({RequestType} -> {ResponseType}) created with {MiddlewareCount} middlewares {Middlewares}"
        )]
        private static partial void InnerInvocationPipelineChainCreated(
            this ILogger logger,
            Exception ex,
            Type requestType,
            Type responseType,
            int middlewareCount,
            IEnumerable<Type> middlewares
        );

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void InvocationPipelineChainCreated<TRequest, TResponse>(
            this ILogger logger,
            IList<IMiddlewareInvocationPipelineItem> middlewares
        )
        {
            if (logger.IsEnabled(LogLevel.Trace))
            {
                InnerInvocationPipelineChainCreated(
                    logger,
                    ex: null,
                    requestType: typeof(TRequest),
                    responseType: typeof(TResponse),
                    middlewareCount: middlewares.Count,
                    middlewares: middlewares.Select(m => m.MiddlewareType)
                );
            }
        }

        [LoggerMessage(
            SkipEnabledCheck = true,
            EventId = BaseEventId + 7,
            EventName = nameof(InvokingPipeline),
            Level = LogLevel.Debug,
            Message = "Invoking invocation pipeline ({RequestType} -> {ResponseType})"
        )]
        private static partial void InnerInvokingPipeline(
            this ILogger logger,
            Exception ex,
            Type requestType,
            Type? responseType
        );

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void InvokingPipeline<TRequest, TResponse>(this ILogger logger)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                InnerInvokingPipeline(
                    logger,
                    ex: null,
                    requestType: typeof(TRequest),
                    responseType: typeof(TResponse)
                );
            }
        }

        [LoggerMessage(
            SkipEnabledCheck = true,
            EventId = BaseEventId + 8,
            EventName = nameof(InvokedPipeline),
            Level = LogLevel.Debug,
            Message = "Invoked invocation pipeline ({RequestType} -> {ResponseType})"
        )]
        private static partial void InnerInvokedPipeline(
            this ILogger logger,
            Exception ex,
            Type requestType,
            Type? responseType
        );

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void InvokedPipeline<TRequest, TResponse>(this ILogger logger)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                InnerInvokedPipeline(
                    logger,
                    ex: null,
                    requestType: typeof(TRequest),
                    responseType: typeof(TResponse)
                );
            }
        }

        [LoggerMessage(
            SkipEnabledCheck = true,
            EventId = BaseEventId + 9,
            EventName = nameof(PrePipelineAndHandlerMiddlewareUnhandledException),
            Level = LogLevel.Debug,
            Message = "Invocation pipeline chain ({RequestType} -> {ResponseType}) encountered an unhandled exception; an error has been added to the current invocation context"
        )]
        private static partial void InnerPrePipelineAndHandlerMiddlewareUnhandledException(
            this ILogger logger,
            Exception ex,
            Type requestType,
            Type? responseType
        );

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void PrePipelineAndHandlerMiddlewareUnhandledException(
            this ILogger logger,
            IInvocationContext context,
            Exception exception
        )
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                InnerPrePipelineAndHandlerMiddlewareUnhandledException(
                    logger,
                    ex: exception,
                    requestType: context.RequestType,
                    responseType: context.ResponseType
                );
            }
        }

        [LoggerMessage(
            SkipEnabledCheck = true,
            EventId = BaseEventId + 10,
            EventName = nameof(NoHandlersRegisteredException),
            Level = LogLevel.Debug,
            Message = "Invocation pipeline chain ({RequestType} -> {ResponseType}) does not have the required handler; an error has been added to the current invocation context"
        )]
        private static partial void InnerNoHandlersRegisteredException(
            this ILogger logger,
            Exception ex,
            Type requestType,
            Type? responseType
        );

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void NoHandlersRegisteredException(
            this ILogger logger,
            IInvocationContext context,
            Exception exception
        )
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                InnerPrePipelineAndHandlerMiddlewareUnhandledException(
                    logger,
                    ex: exception,
                    requestType: context.RequestType,
                    responseType: context.ResponseType
                );
            }
        }

        [LoggerMessage(
            SkipEnabledCheck = true,
            EventId = BaseEventId + 11,
            EventName = nameof(InvokingHandler),
            Level = LogLevel.Debug,
            Message = "Invoking {Handler} ({RequestType} -> {ResponseType})"
        )]
        private static partial void InnerInvokingHandler(
            this ILogger logger,
            Exception ex,
            object handler,
            Type requestType,
            Type? responseType
        );

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void InvokingHandler<TRequest, TResponse>(
            this ILogger logger,
            IInvocationHandler<TRequest, TResponse> handler
        )
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                InnerInvokingHandler(
                    logger,
                    ex: null,
                    handler: handler,
                    requestType: typeof(TRequest),
                    responseType: typeof(TResponse)
                );
            }
        }

        [LoggerMessage(
            SkipEnabledCheck = true,
            EventId = BaseEventId + 12,
            EventName = nameof(InvokedHandler),
            Level = LogLevel.Debug,
            Message = "Invoked {Handler} ({RequestType} -> {ResponseType}); response has been set into the current invocation context"
        )]
        private static partial void InnerInvokedHandler(
            this ILogger logger,
            Exception ex,
            object handler,
            Type requestType,
            Type? responseType
        );

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void InvokedHandler<TRequest, TResponse>(
            this ILogger logger,
            IInvocationHandler<TRequest, TResponse> handler
        )
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                InnerInvokedHandler(
                    logger,
                    ex: null,
                    handler: handler,
                    requestType: typeof(TRequest),
                    responseType: typeof(TResponse)
                );
            }
        }
    }
}
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
