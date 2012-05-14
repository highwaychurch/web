using System;

namespace Highway.Shared.Diagnostics
{
    // ReSharper disable UnusedTypeParameter
    public interface ILog<TClient>
    // ReSharper restore UnusedTypeParameter
    {
        void Debug(string message, params object[] formatArgs);
        void Information(string message, params object[] formatArgs);
        void Warning(Exception exception, string message, params object[] formatArgs);
        void Warning(string message, params object[] formatArgs);
        void Error(Exception exception, string message, params object[] formatArgs);
        void Error(string message, params object[] formatArgs);
        void Fatal(Exception exception, string message, params object[] formatArgs);
    }
}