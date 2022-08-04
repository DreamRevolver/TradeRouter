using System;

namespace Shared.Interfaces
{
    public enum LogPriority
    {
        Undefined,
        Debug,
        BinanceWeight,
        Info,
        Warning,
        Error,
        AccountTracker
    }

    public interface ILogger
    {
        void Log(LogPriority priority, string message, string source);
        void Log(LogPriority priority, Exception exception, string source, string description);
    }

    public interface ILogger<TSource>
    {
        void Log(LogPriority priority, string message);
        void Log(LogPriority priority, Exception exception, string description);
    }

    public static class LoggerExtensions
    {
        public static void Debug(this ILogger log, string message, string source)
            => log.Log(LogPriority.Debug, message: message, source: source);

        public static void Debug<TSource>(this ILogger<TSource> log, string message)
            => log.Log(LogPriority.Debug, message: message);

        public static void Debug(this ILogger log, Exception ex, string source, string description)
            => log.Log(LogPriority.Debug, ex, source: source, description: description);

        public static void Debug<TSource>(this ILogger<TSource> log, Exception ex, string description)
            => log.Log(LogPriority.Debug, ex, description: description);

        public static void Info(this ILogger log, string message, string source)
            => log.Log(LogPriority.Info, message: message, source: source);

        public static void Info<TSource>(this ILogger<TSource> log, string message)
            => log.Log(LogPriority.Info, message: message);

        public static void Warn(this ILogger log, string message, string source)
            => log.Log(LogPriority.Warning, message: message, source: source);

        public static void Warn<TSource>(this ILogger<TSource> log, string message)
            => log.Log(LogPriority.Warning, message: message);

        public static void Error(this ILogger log, string message, string source)
            => log.Log(LogPriority.Error, message: message, source: source);

        public static void Error<TSource>(this ILogger<TSource> log, string message)
            => log.Log(LogPriority.Error, message: message);

        public static void Error(this ILogger log, Exception ex, string source, string description)
            => log.Log(LogPriority.Error, ex, source: source, description: description);

        public static void Error<TSource>(this ILogger<TSource> log, Exception ex, string description)
            => log.Log(LogPriority.Error, ex, description: description);

        public static void BinanceWeight(this ILogger log, string message, string source)
            => log.Log(LogPriority.BinanceWeight, message: message, source: source);

        public static void BinanceWeight<TSource>(this ILogger<TSource> log, string message)
            => log.Log(LogPriority.BinanceWeight, message: message);

        public static void AccountTracker(this ILogger log, string message, string source)
            => log.Log(LogPriority.AccountTracker, message: message, source: source);

        public static void AccountTracker<TSource>(this ILogger<TSource> log, string message)
            => log.Log(LogPriority.AccountTracker, message: message);
    }
}
