using System.Diagnostics;
using Microsoft.Extensions.Logging;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
namespace Ingot.Mediator.Engine.Pipeline.Extensions
{
    // EventID:         0->65535
    // Ingot:           1## {component} + ## {event}
    // Mediator.Engine: 11001 -> 11099
    internal static class MediatorLoggerExtensions
    {
        private static EventId Event(int id, string? name = null)
        {
            ThrowIfIdInvalid(id);
            return new EventId(11000 + id, name);
        }

        [Conditional("DEBUG")]
        private static void ThrowIfIdInvalid(int id)
        {
            if (id < 1 || id > 99)
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
            "Invocation pipeline not found ({RequestType} -> {ResponseType}) for contract {Contract}");

        internal static void InvocationPipelineNotFound<TResponse>(this ILogger logger, object request)
        {
            if (!logger.IsEnabled(LogLevel.Debug))
            {
                // Early exit to avoid extra compute
                return;
            }

            s_invocationPipelineNotFound(logger, request.GetType(), typeof(TResponse), request, null);
        }
    }
}
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
