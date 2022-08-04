using System;
using Shared.Interfaces;
namespace SharedBinance.Broker
{
	public class ConsoleLogger : ILogger
	{
		public void Log(LogPriority priority, string message, string source)
			=> Console.WriteLine($"{DateTime.Now.ToShortTimeString()} | {priority.ToString()} | {source} | {message}");
		public void Log(LogPriority priority, Exception exception, string source, string description)
			=> Console.WriteLine($"{DateTime.Now.ToShortTimeString()} | {priority.ToString()} | {source} | {exception.Message} | {exception.StackTrace}");

	}
}
