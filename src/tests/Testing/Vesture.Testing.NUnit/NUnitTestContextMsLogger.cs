using System.Text;
using Microsoft.Extensions.Logging;

namespace Vesture.Testing
{
    /// <summary>
    /// NUnit stderr console implementation of <see cref="ILoggerProvider"/>.
    /// </summary>
    public class NUnitTestContextLoggingProvider : ILoggerProvider
    {
        /// <inheritdoc/>
        public ILogger CreateLogger(string categoryName) => new NUnitTestContextMsLogger();

        /// <inheritdoc/>
        public void Dispose() { }
    }

    /// <summary>
    /// NUnit stderr console implementation of <see cref="ILogger{TCategoryName}"/>.
    /// </summary>
    public class NUnitTestContextMsLogger<TCategoryName>
        : NUnitTestContextMsLogger,
            Microsoft.Extensions.Logging.ILogger<TCategoryName> { }

    /// <summary>
    /// NUnit stderr console implementation of <see cref="ILogger"/>.
    /// </summary>
    public class NUnitTestContextMsLogger : Microsoft.Extensions.Logging.ILogger
    {
        /// <inheritdoc/>
        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull => NoOpDisposable.Instance;

        /// <inheritdoc/>
        public bool IsEnabled(LogLevel logLevel) => true;

        /// <inheritdoc/>
        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter
        )
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter is null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

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
