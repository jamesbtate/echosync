using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SharedLibrary
{
    public static class Logger
    {
        public enum LogLevel { CRIT, ERROR, WARN, NOTICE, INFO, DEBUG, DEBUG2 };
        private static LogLevel defaultLogLevel = LogLevel.INFO;
        private static StreamWriter writer;
        private static String filename;
        //this should be changed to include the date and time and shoulb be initialized in Init()
        private static String defaultFileName = Path.Combine("C:\\Temp", "log.log");
        //true when Init has been calld. used to prevent multiple inits
        private static bool inited = false;

        /// <summary>
        /// Initializes the logger with a custom path to the log file.
        /// </summary>
        /// <param name="path"></param>
        public static void Init(string path)
        {
            if (inited) return;
            filename = path;
            privateInit();
        }
        /// <summary>
        /// Initializes the logger with the default path.
        /// </summary>
        public static void Init()
        {
            if (inited) return;
            filename = defaultFileName;
            privateInit();
        }

        private static void privateInit()
        {
            writer = new StreamWriter(filename, false);
            inited = true;
            Logger.Log("Opened log file: " + filename);
        }

        public static string getLogFilename()
        {
            return filename;
        }

        /// <summary>
        /// Logs a string to file with the default LogLevel (Logger constant member)
        /// </summary>
        /// <param name="s">The string to be logged</param>
        public static void Log(String s,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            Logger.Log(s, defaultLogLevel, memberName, sourceFilePath, sourceLineNumber);
        }

        public static void Crit(String s,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            Logger.Log(s, LogLevel.CRIT, memberName, sourceFilePath, sourceLineNumber);
        }
        public static void Error(String s,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            Logger.Log(s, LogLevel.ERROR, memberName, sourceFilePath, sourceLineNumber);
        }
        public static void Warn(String s,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            Logger.Log(s, LogLevel.WARN, memberName, sourceFilePath, sourceLineNumber);
        }
        public static void Notice(String s,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            Logger.Log(s, LogLevel.NOTICE, memberName, sourceFilePath, sourceLineNumber);
        }
        public static void Info(String s,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            Logger.Log(s, LogLevel.INFO, memberName, sourceFilePath, sourceLineNumber);
        }
        public static void Debug(String s,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            Logger.Log(s, LogLevel.DEBUG, memberName, sourceFilePath, sourceLineNumber);
        }
        public static void Debug2(String s,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            Logger.Log(s, LogLevel.DEBUG2, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <summary>
        /// Logs a string to the log file. Prepends the log entry with the date, time and level followed by a space and appends with a newline.
        /// </summary>
        /// <param name="s">The string to be logged</param>
        /// <param name="level">The level this messaged should be logged as (enum Logger.LogLevel.*)</param>
        public static void Log(String s, LogLevel level,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            String dateString = DateTime.Now.ToString();
            FileInfo fi = new FileInfo(sourceFilePath);
            String path = fi.Directory.Name + '\\' + fi.Name;
            writer.WriteLine(dateString + " " + level + " " + memberName + "() " + path + ':' + sourceLineNumber + " " + s);
            Logger.Flush();
        }

        public static void Flush()
        {
            writer.Flush();
        }

        public static void Close()
        {
            writer.Close();
        }
    }
}
