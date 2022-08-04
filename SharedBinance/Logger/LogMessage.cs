using log4net.Core;
using Shared.Interfaces;
namespace Shared.Logger
{
    public class LogMessage
    {
        public LogMessage(LoggingEvent loggingEvent)
        {
            Severity = (LogPriority)loggingEvent.GetProperties()["SeverityType"];
            Time = loggingEvent.TimeStampUtc.ToLocalTime().ToString("MMM dd  HH:mm:ss.fff");
            Message = loggingEvent.MessageObject.ToString();
            Source = (string)loggingEvent.GetProperties()["Source"];
        }

        public LogPriority Severity { get; set; }
        public string Time { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
    }
}
