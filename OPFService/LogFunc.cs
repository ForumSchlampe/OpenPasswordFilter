using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPFService
{
    public class LogFunc
    {
        public static void writeLog(string message, System.Diagnostics.EventLogEntryType level)
        {
            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "OpenPasswordFilter";
                eventLog.WriteEntry(message, level, 100, 1);
            }
        }
    }
}
