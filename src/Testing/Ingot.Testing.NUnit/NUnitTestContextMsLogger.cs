using System.Text;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Ingot.Testing
{
    public class NUnitTestContextMsLogger<TCategoryName> :
        NUnitTestContextMsLogger,
        Microsoft.Extensions.Logging.ILogger<TCategoryName>
    {

    }

    public class NUnitTestContextMsLogger : Microsoft.Extensions.Logging.ILogger
    {
        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull => NoOpDisposable.Instance;

        public bool IsEnabled(LogLevel logLevel) => true;
        
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            ArgumentNullException.ThrowIfNull(formatter, nameof(formatter));

            var message = formatter(state, exception);
            var sb = new StringBuilder();
            sb.Append($"[{logLevel}] ");
            if (!string.IsNullOrEmpty(message))
            {
                sb.AppendLine(message);
            }

            if (exception != null)
            {
                sb.AppendLine(exception.ToString());
            }

            TestContext.Out?.Write(sb.ToString());
        }

        private class NoOpDisposable : IDisposable
        {
            public static readonly NoOpDisposable Instance = new();

            public void Dispose() { }
        }
    }
}
