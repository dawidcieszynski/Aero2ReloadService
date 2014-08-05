namespace Aero2Reload.Service.Loggers
{
    using System;

    public abstract class Logger
    {
        public abstract void Debug(string entry);

        public abstract void DebugException(Exception exception);

        public abstract void Error(string entry);
    }
}
