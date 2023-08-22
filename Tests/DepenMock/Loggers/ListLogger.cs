using Microsoft.Extensions.Logging;

namespace DepenMock.Loggers
{
	/// <summary>
	/// Custom Logging class used for testing logging
	/// </summary>
	public class ListLogger : ILogger
	{
		/// <summary>
		/// Creates an instance of <see cref="ListLogger"/>
		/// </summary>
		public ListLogger()
		{
			Logs = new Dictionary<LogLevel, List<string>>();
		}


		/// <inheritdoc />
		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
		{
			var message = formatter(state, exception);

			LogMessage(logLevel, message);
		}

		private void LogMessage(LogLevel logLevel, string message)
		{
			if (!Logs.ContainsKey(logLevel))
			{
				Logs.Add(logLevel, new List<string>());
			}

			Logs[logLevel].Add(message);
		}


		/// <inheritdoc />
		public IDisposable BeginScope<TState>(TState state) => NullScope.Instance;


		/// <inheritdoc />
		public bool IsEnabled(LogLevel logLevel) => false;


		/// <summary>
		/// Returns the <see cref="IDictionary{TKey,TValue}"/> of messages that have been logged
		/// </summary>
		public IDictionary<LogLevel, List<string>> Logs { get; }
	}
}
