using System;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Targets.Wrappers;

namespace ZerO.Log
{
    public class Log : ILogger
    {
        private readonly Logger _logger;

        public Log()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }

        public void Info(string message)
        {
            _logger.Info(message);
        }

        public void Warn(string message)
        {
            _logger.Warn(message);
        }

        public void Init(string message)
        {
            _logger.Trace(message);
        }

        public void Error(string message)
        {
            _logger.Error(message);
        }

        public void Error(Exception x)
        {
            _logger.Error(this.BuildExceptionMessage(x));
        }

        public void Fatal(string message)
        {
            _logger.Fatal(message);
        }

        public void Fatal(Exception x)
        {
            _logger.Fatal(this.BuildExceptionMessage(x));
        }

        public void ConfigureLogger()
        {
            // Step 1. Create configuration object 
            var config = new LoggingConfiguration();

            // Step 2. Create targets and add them to the configuration 
            var target = new WpfRichTextBoxTarget
            {
                Name = "console",
                Layout = @"[${time:format=HH:mm:ss}] ${level}: ${message}",
                ControlName = "logRichTextBox",
                FormName = "Main",
                AutoScroll = true,
                MaxLines = 100000,
                UseDefaultRowColoringRules = true
            };
            var asyncWrapper = new AsyncTargetWrapper {Name = "console", WrappedTarget = target};
            SimpleConfigurator.ConfigureForTargetLogging(asyncWrapper, LogLevel.Trace);

            var fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);

            var dir = AppDomain.CurrentDomain.BaseDirectory;

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            fileTarget.FileName = dir + "/log.txt";
            fileTarget.Layout = @"${date}|${level:uppercase=true}|${message}";

            var rule1 = new LoggingRule("*", LogLevel.Trace, fileTarget);
            config.LoggingRules.Add(rule1);

            var rule2 = new LoggingRule("*", LogLevel.Trace, target);
            config.LoggingRules.Add(rule2);

            // Step 5. Activate the configuration
            LogManager.Configuration = config;
        }
    }

    public static class LoggerExt
    {
        public static string BuildExceptionMessage(this ILogger logger, Exception x)
        {
            Exception logException = x;

            if (x.InnerException != null)
                logException = x.InnerException;

            string strErrorMsg = Environment.NewLine + "Message :" + logException.Message;
            strErrorMsg += Environment.NewLine + "Source :" + logException.Source;
            strErrorMsg += Environment.NewLine + "Stack Trace :" + logException.StackTrace;
            strErrorMsg += Environment.NewLine + "TargetSite :" + logException.TargetSite;

            return strErrorMsg;
        }
    }

    public interface ILogger
    {
        void Init(string message);
        void Info(string message);
        void Warn(string message);
        //void Debug(string message);
        void Error(string message);
        void Error(Exception x);
        void Fatal(string message);
        void Fatal(Exception x);
    }
}
