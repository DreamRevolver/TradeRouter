using System;
using System.Reflection;
using log4net;
using log4net.Config;
using Shared.Interfaces;

namespace Shared.Logger
{
    public struct MyErrorLog
    {
        public string Message;
        public string Source;
        public LogPriority Priority;
        public MyErrorLog(LogPriority priority, Exception exception, string source, string description = null)
        {
            Priority = priority;

            if (priority == LogPriority.Debug)
            {
                Message = (description == null ? "" : $"{description} | ") + (exception.InnerException == null ? $"{exception.Message} | {exception.StackTrace}" :
                    $"EXCEPTION: {exception.Message} | {exception.StackTrace} | INNER EXCEPTION: {exception.InnerException.Message} | {exception.InnerException.StackTrace}");
            } else
            {
                Message = (description == null ? "" : $"{description} | ") + (exception.InnerException == null ? $"{exception.Message}" :
                    $"EXCEPTION: {exception.Message} | INNER EXCEPTION: {exception.InnerException.Message}");
            }

            Source = source;
        }
        public override string ToString()
                => Message;
                
    }
    public struct MyLog
    {
        public string Message;
        public string Source;
        public LogPriority Priority;
        public MyLog(LogPriority priority, string message, string source)
        {
            Priority = priority;
            Message = message;
            Source = source;
        }
        public override string ToString()
            => Message;

    }

    public static class BinanceLogLevels
    {
        public static readonly log4net.Core.Level AccountTracker = new log4net.Core.Level(1, "AccountTracker");
        public static readonly log4net.Core.Level BinanceWeight = new log4net.Core.Level(2, "BinanceWeight");

        public static void Register()
        {
            LogManager.GetRepository().LevelMap.Add(AccountTracker);
            LogManager.GetRepository().LevelMap.Add(BinanceWeight);
        }
    }

    public class Logger : ILogger
    {
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public Logger()
        {
            BinanceLogLevels.Register();
            XmlConfigurator.Configure();
        }

        void ILogger.Log(LogPriority priority, string message, string source)
        {
            ThreadContext.Properties["SeverityType"] = priority;
            ThreadContext.Properties["Source"] = source;

            if (priority == LogPriority.Error)
            {
                log.Error(new MyLog(priority, message, source));
            }
            else if (priority == LogPriority.Warning)
            {
                log.Warn(new MyLog(priority, message, source));
            }
            else if (priority == LogPriority.Info)
            {
                log.Info(new MyLog(priority, message, source));
            }
            else if (priority == LogPriority.Debug)
            {
                log.Debug(new MyLog(priority, message, source));
            }
            else if (priority == LogPriority.AccountTracker)
            {
                log.Logger.Log(null, BinanceLogLevels.AccountTracker, new MyLog(priority, message, source), null);
            }
            else if (priority == LogPriority.BinanceWeight)
            {
                log.Logger.Log(null, BinanceLogLevels.BinanceWeight, new MyLog(priority, message, source), null);
            }
        }
        void ILogger.Log(LogPriority priority, Exception exception, string source, string description)
        {
            ThreadContext.Properties["SeverityType"] = priority;
            ThreadContext.Properties["Source"] = source;
            if (priority == LogPriority.Error)
            {
                log.Error(new MyErrorLog(priority, exception, source, description));
            }
            else if (priority == LogPriority.Debug)
            {
                log.Debug(new MyErrorLog(priority, exception, source, description));
            }
        }
    }

    public class Logger<TSource> : Logger, ILogger<TSource>
    {
        public void Log(LogPriority priority, string message)
            => ((ILogger)this).Log(priority, message: message, source: typeof(TSource).FullName);

        public void Log(LogPriority priority, Exception exception, string description)
            => ((ILogger)this).Log(priority, exception, description: description, source: typeof(TSource).FullName);
    }
}
