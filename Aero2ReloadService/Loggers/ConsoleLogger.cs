namespace Aero2Reload.Service.Loggers
{
    using System;

    public class ConsoleLogger : Logger
    {
        public override void Debug(string entry)
        {
            Console.WriteLine(entry);
        }

        public override void DebugException(Exception exception)
        {
            Console.WriteLine(exception.Message);
            Console.WriteLine(exception.StackTrace);
        }

        public override void Error(string entry)
        {
            Console.WriteLine(entry);
        }
    }
}