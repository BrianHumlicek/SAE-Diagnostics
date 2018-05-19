using System;

namespace J1979.Common
{
    public interface ILogger
    {
        void Trace(string message);
        void Info(string message);
        void Warn(string message, Exception exception = null);
        void Error(string message, Exception exception = null);
    }
}
