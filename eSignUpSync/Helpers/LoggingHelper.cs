using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSignUpSync.Helpers
{
    public static class LoggingHelper
    {
        /// <summary>
        /// Create a new ILoggerFactory configured for the app.
        /// Caller is responsible for disposing the returned factory (e.g. using var).
        /// </summary>
        public static ILoggerFactory CreateLoggerFactory(LogLevel minimumLevel = LogLevel.Information)
        {
            return LoggerFactory.Create(builder =>
            {
                builder
                    .ClearProviders()
                    // Register the custom formatter type
                    .AddConsoleFormatter<NoPrefixConsoleFormatter, ConsoleFormatterOptions>()
                    // Tell the console logger to use the "NoPrefix" formatter
                    .AddConsole(options =>
                    {
                        options.FormatterName = "NoPrefix";
                    })
                    .SetMinimumLevel(minimumLevel);
            });
        }

        // Minimal console formatter that writes only the message and exception (no level/category/event id)
        public sealed class NoPrefixConsoleFormatter : ConsoleFormatter
        {
            public NoPrefixConsoleFormatter() : base("NoPrefix") { }

            public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider, TextWriter textWriter)
            {
                // Use the logger's formatter to produce the message (respecting structured state)
                var message = logEntry.Formatter?.Invoke(logEntry.State, logEntry.Exception);

                if (!string.IsNullOrEmpty(message))
                {
                    textWriter.Write(message);
                }

                if (logEntry.Exception != null)
                {
                    // write the exception after the message
                    if (!string.IsNullOrEmpty(message))
                        textWriter.Write(" ");
                    textWriter.Write(logEntry.Exception);
                }

                textWriter.WriteLine();
            }
        }
    }
}
