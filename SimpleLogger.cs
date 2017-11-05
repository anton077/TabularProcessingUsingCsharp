using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace ConsoleApp1
{
    class SimpleLogger
    {
        private static string Filename;
        private static string DatetimeFormat ="yyyyMMdd HH:mm";

        static SimpleLogger()
        {
            Filename = Assembly.GetExecutingAssembly().GetName().Name + ".log";
            if(!(File.Exists(Filename)))
                {

            }
            else System.IO.File.WriteAllText(Filename, string.Empty);


        }

        public static void Info(string text)
        {
            LogWriter(LogLevel.INFO, text);
        }
        public static void Warning(string text)
        {
            LogWriter(LogLevel.WARNING, text);
        }
        public static void Error(string text)
        {
            LogWriter(LogLevel.ERROR, text);
        }
        public static void Fatal(string text)
        {
            LogWriter(LogLevel.FATAL, text);
        }

        private static void LogWriter(LogLevel level, string text)
        {
            string pretext;
            switch (level)
            {
                case LogLevel.INFO: pretext = DateTime.Now.ToString(DatetimeFormat) + " [INFO]    "; break;
                case LogLevel.WARNING: pretext = DateTime.Now.ToString(DatetimeFormat) + "[WARNING] "; break;
                case LogLevel.ERROR: pretext = DateTime.Now.ToString(DatetimeFormat) + " [ERROR]   "; break;
                case LogLevel.FATAL: pretext = DateTime.Now.ToString(DatetimeFormat) + " [FATAL]   "; break;
                default: pretext = ""; break;
            }
            WriteLine(pretext+ text);
        }

        private static void WriteLine(string text, bool append = true)
        {
            try
            {
                using (StreamWriter Writer = new StreamWriter(Filename, append, Encoding.UTF8))
                {
                    if (text != "") Writer.WriteLine(text);
                    Console.WriteLine(text);
                }
            }
            catch
            {
                throw;
            }
        }

        private enum LogLevel
        {
            INFO,
            WARNING,
            ERROR,
            FATAL
        }

    }
    
}
