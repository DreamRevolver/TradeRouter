using log4net;

namespace WebRouterApp.Shared.Services
{
    public class Log<TSource>
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(TSource));

        public void Info(string message) => _log.Info(message);
        public void Warn(string message) => _log.Warn(message);
        public void Error(string message) => _log.Error(message);
        public void Debug(string message) => _log.Debug(message);
    }
}
