using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AtriumWebApp.Web.Base.Library
{
    public class ApplicationLogging
    {
        private static ILoggerFactory _Factory = null;
        private static ILogger Logger = null;

        public static void ConfigureLogger(ILoggerFactory factory)
        {
            factory.AddDebug(LogLevel.None);
            factory.CreateLogger("Debugger");
            Logger = factory.CreateLogger("DEBUG");
        }

        public static ILoggerFactory LoggerFactory
        {
            get
            {
                if (_Factory == null)
                {
                    _Factory = new LoggerFactory();
                    ConfigureLogger(_Factory);
                }
                return _Factory;
            }
            set { _Factory = value; }
        }

        public static void Log(string message)
        {
            Logger.LogInformation("INFO", message);
        }

        public static void Log(string message, Exception ex)
        {
            Logger.LogTrace("STACK TRACE", ex, message);
        }
        public static void Log(Exception ex)
        {
            Logger.LogWarning("STACK TRACE", ex);
        }
    }
}
