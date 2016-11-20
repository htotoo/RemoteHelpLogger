using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteHelpLogger
{
    class LoggerLogic
    {
        public static void LogSessionStart() { LogSessionStart(DateTime.Now);  }
        public static void LogSessionStart(DateTime startTime)
        {
            //todo
        }

        public static void LogSessionStop(DateTime startTime) { LogSessionStop(startTime, DateTime.Now);  }
        public static void LogSessionStop(DateTime startTime, DateTime stopTime)
        {
            //todo
        }
    }
}
