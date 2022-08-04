using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

using Shared.Interfaces;
using Shared.Logger;
using Utf8Json.Resolvers;

namespace RouterApplication
{
    public static class GlobalExceptionLogHandler
    {

        private static FileStream CreateNewFile(string name)
        {
            if (File.Exists(name))
            {
                File.Delete(name);
            }
            return File.Create(name);
        }

        public static void LogToFile(string filename, string message)
        {
            using (var writer = CreateNewFile(filename))
            {
                var payload = Encoding.UTF8.GetBytes(message);
                writer.Write(payload, 0, payload.Length);
            }
        }

    }
    public class SingleApplication : Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase
    {
        private ILogger _logger;
        public SingleApplication(ILogger Logger)
        {
            _logger = Logger;
            IsSingleInstance = true;
            EnableVisualStyles = true;
            UnhandledException += (sender, args) =>
            {
                GlobalExceptionLogHandler.LogToFile("ThreadExceptionLogFile",$"ThreadException EXCEPTION FROM SENDER: {sender} => Message {args.Exception.Message} | StackTrace {args.Exception.StackTrace}");
                _logger.Log(LogPriority.Error, $"UnhandledException EXCEPTION FROM SENDER: {sender} => Message {args.Exception.Message} | StackTrace {args.Exception.StackTrace}", "UNHANDLED ThreadException");
            };
        }
        protected override void OnCreateMainForm()
            => this.MainForm = new MainForm(_logger);
    }
    internal static class Program
    {
        private static ILogger logger;
        [STAThread]
        private static void Main(string[] args)
        {

            CompositeResolver.RegisterAndSetAsDefault(StandardResolver.AllowPrivateExcludeNull);

            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            logger = new Logger();
            var App = new SingleApplication(logger);
            App.Run(args);
        }
    }
}
