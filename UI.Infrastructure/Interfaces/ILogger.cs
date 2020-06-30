using System;

namespace UI.Infrastructure.Interfaces
{
    public interface ILogger
    {
        void Trace(string message);

        void Info(string message);

        void Debug(string message);

        void Warning(string message);

        void Error(string message);

        void Exception(Exception ex);
    }
}
