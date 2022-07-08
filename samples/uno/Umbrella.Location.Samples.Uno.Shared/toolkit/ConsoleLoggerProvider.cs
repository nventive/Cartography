#define TRACE
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Text;

namespace Umbrella.Location.Samples.Uno
{
	public class ConsoleLoggerProvider : ILoggerProvider
	{
		private readonly Func<DateTimeOffset, LogLevel, string, string, string> _consoleFormatter;

		public ConsoleLoggerProvider(Func<DateTimeOffset, LogLevel, string, string, string> consoleFormatter)
		{
			_consoleFormatter = consoleFormatter;
		}

		public ILogger CreateLogger(string categoryName)
		{
			return new ConsoleLogger(categoryName, _consoleFormatter);
		}

		public void Dispose()
		{
		}

		public class ConsoleLogger : ILogger
		{
			private const string Tag = "Console";

			private readonly string _name;
			private readonly Func<DateTimeOffset, LogLevel, string, string, string> _consoleFormatter;

			public ConsoleLogger(string name, Func<DateTimeOffset, LogLevel, string, string, string> consoleFormatter)
			{
				_name = name;
				_consoleFormatter = consoleFormatter;
			}

			public IDisposable BeginScope<TState>(TState state)
			{
				return Disposable.Empty;
			}

			public bool IsEnabled(LogLevel logLevel)
			{
				return logLevel != LogLevel.None;
			}

			public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
			{
				if (!IsEnabled(logLevel))
				{
					return;
				}

				if (formatter == null)
				{
					throw new ArgumentNullException(nameof(formatter));
				}

				var message = formatter(state, exception);

				if (string.IsNullOrEmpty(message))
				{
					return;
				}

				var consoleMessage = _consoleFormatter(DateTimeOffset.Now, logLevel, _name, message);

				if (exception != null)
				{
					consoleMessage += System.Environment.NewLine + System.Environment.NewLine + exception.ToString();
				}

				InnerLog(logLevel, consoleMessage);
			}

			private void InnerLog(LogLevel logLevel, string message)
			{
#if __ANDROID__
				if (logLevel == LogLevel.Trace)
				{
					Android.Util.Log.Verbose(Tag, message);
				}
				else if (logLevel == LogLevel.Debug)
				{
					Android.Util.Log.Debug(Tag, message);
				}
				else if (logLevel == LogLevel.Information)
				{
					Android.Util.Log.Info(Tag, message);
				}
				else if (logLevel == LogLevel.Warning)
				{
					Android.Util.Log.Warn(Tag, message);
				}
				else if (logLevel == LogLevel.Error)
				{
					Android.Util.Log.Error(Tag, message);
				}
				else if (logLevel == LogLevel.Critical)
				{
					Android.Util.Log.Wtf(Tag, message);
				}
#elif __IOS__
				Console.WriteLine(message);
#else
				Trace.WriteLine(message);
#endif
			}
		}
	}
}
