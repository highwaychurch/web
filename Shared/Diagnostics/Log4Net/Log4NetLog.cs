using System;
using log4net;

namespace Highway.Shared.Diagnostics.Log4Net
{
    public class Log4NetLog<TClient> : ILog<TClient>
    {
        readonly ILog _log;

        public Log4NetLog()
        {
            _log = LogManager.GetLogger(typeof(TClient));
        }

        public void Debug(string message, params object[] formatArgs)
        {
            _log.DebugFormat(message, formatArgs);
        }

        public void Information(string message, params object[] formatArgs)
        {
            _log.InfoFormat(message, formatArgs);
        }

        public void Warning(Exception exception, string message, params object[] formatArgs)
        {
            if (formatArgs == null || formatArgs.Length == 0)
                _log.Warn(message, exception);
            else
                _log.Warn(string.Format(message, formatArgs), exception);
        }

        public void Warning(string message, params object[] formatArgs)
        {
            _log.WarnFormat(message, formatArgs);
        }

        public void Error(Exception exception, string message, params object[] formatArgs)
        {
            if (formatArgs == null || formatArgs.Length == 0)
                _log.Error(message, exception);
            else
                _log.Error(string.Format(message, formatArgs), exception);
        }

        public void Error(string message, params object[] formatArgs)
        {
            _log.ErrorFormat(message, formatArgs);
        }

        public void Fatal(Exception exception, string message, params object[] formatArgs)
        {
            if (formatArgs == null || formatArgs.Length == 0)
                _log.Fatal(message, exception);
            else
                _log.Fatal(string.Format(message, formatArgs), exception);
        }
    }
}