namespace Aero2Reload.Service.Loggers
{
    using System;
    using System.Diagnostics;

    public class EventLogLogger : Logger
    {
        private readonly EventLog eventLog;

        public EventLogLogger(string eventSourceName, string logName)
        {
            this.eventLog = new EventLog();
            if (!EventLog.SourceExists(eventSourceName))
            {
                EventLog.CreateEventSource(eventSourceName, logName);
            }

            this.eventLog.Source = eventSourceName;
            this.eventLog.Log = logName;
        }

        public override void Debug(string entry)
        {
            this.eventLog.WriteEntry(entry);
        }

        public override void DebugException(Exception exception)
        {
            this.eventLog.WriteEntry(exception.Message, EventLogEntryType.Error);
        }

        public override void Error(string entry)
        {
            this.eventLog.WriteEntry(entry, EventLogEntryType.Error);
        }
    }
}