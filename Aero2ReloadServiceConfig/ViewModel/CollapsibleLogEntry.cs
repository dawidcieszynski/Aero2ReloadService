namespace Aero2Reload.Config.ViewModel
{
    using System.Collections.Generic;

    public class CollapsibleLogEntry : LogEntry
    {
        public List<LogEntry> Contents { get; set; }
    }
}