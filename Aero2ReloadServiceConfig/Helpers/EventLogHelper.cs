namespace Aero2ReloadServiceConfig.Helpers
{
    using System.Diagnostics;

    public static class EventLogHelper
    {
        public static string Format(this EventLogEntry entry)
        {
            return string.Format("{0} {1}\n", entry.TimeGenerated, entry.Message);
        }
    }
}