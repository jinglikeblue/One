using System;
using System.Threading;

namespace One
{
    public class OneLog
    {
        /// <summary>
        /// Log输出事件
        /// </summary>
        public static event Action<string> onLog;

        public enum ELogLevel
        {
            NONE,
            ERROR,
            WARNING,
            INFO,                                              
        }

        /// <summary>
        /// 默认的控制台颜色
        /// </summary>
        public static ConsoleColor defaultColor = ConsoleColor.DarkGreen;

        /// <summary>
        /// 日志打印等级
        /// </summary>
        public static ELogLevel logLevel = ELogLevel.INFO;

        /// <summary>
        /// 是否Log打印到控制台，否则仅派送事件
        /// </summary>
        public static bool isConsoleOutput = true;

        /// <summary>
        /// 日志文件
        /// </summary>
        static OneLogFile _logFile;

        public static void UseLogFile(string outputDir, int logKeepDays)
        {
            _logFile = new OneLogFile(outputDir, logKeepDays);
        }

        public static void LogLine(object message)
        {
            var now = DateTime.Now;
            var logContent = string.Format("[Thread:{0}, Date:{1}, UTC:{2}] {3}", Thread.CurrentThread.ManagedThreadId.ToString("000"), now.ToString("yyyy-MM-dd HH:mm:ss"), now.ToFileTimeUtc() / 10000, message);
            if (isConsoleOutput)
            {
                Console.WriteLine(logContent);
            }

            _logFile?.OutputLog(logContent);

            onLog?.Invoke(logContent);
        }

        /// <summary>
        /// 打印信息
        /// </summary>
        /// <param name="message"></param>
        public static void I(object message)
        {
            if(logLevel < ELogLevel.INFO)
            {
                return;
            }

            ColorInfo(defaultColor, message);
        }

        /// <summary>
        /// 打印信息
        /// </summary>
        public static void I(string format, params object[] args)
        {
            if (logLevel < ELogLevel.INFO)
            {
                return;
            }

            if (args.Length > 0)
            {
                ColorInfo(defaultColor, string.Format(format, args));
            }
            else
            {
                ColorInfo(defaultColor, format);
            }
        }

        /// <summary>
        /// 打印彩色信息
        /// </summary>
        /// <param name="color"></param>
        /// <param name="message"></param>
        public static void I(ConsoleColor color, object message)
        {
            if (logLevel < ELogLevel.INFO)
            {
                return;
            }

            ColorInfo(color, message);
        }

        /// <summary>
        /// 打印彩色信息
        /// </summary>
        /// <param name="color"></param>
        /// <param name="message"></param>
        public static void I(ConsoleColor color, string format, params object[] args)
        {
            if (logLevel < ELogLevel.INFO)
            {
                return;
            }

            ColorInfo(color, format, args);
        }


        /// <summary>
        /// 打印警告
        /// </summary>
        public static void W(object message)
        {
            if (logLevel < ELogLevel.WARNING)
            {
                return;
            }

            ColorInfo(ConsoleColor.DarkYellow, message);
        }

        /// <summary>
        /// 打印警告
        /// </summary>
        public static void W(string format, params object[] args)
        {
            if (logLevel < ELogLevel.WARNING)
            {
                return;
            }

            ColorInfo(ConsoleColor.DarkYellow, format, args);
        }

        /// <summary>
        /// 打印错误
        /// </summary>
        public static void E(object message)
        {
            if (logLevel < ELogLevel.ERROR)
            {
                return;
            }

            ColorInfo(ConsoleColor.DarkRed, message);
        }

        /// <summary>
        /// 打印错误
        /// </summary>
        public static void E(string format, params object[] args)
        {
            if (logLevel < ELogLevel.ERROR)
            {
                return;
            }

            ColorInfo(ConsoleColor.DarkRed, format, args);
        }

        /// <summary>
        /// 打印调试信息
        /// </summary>
        public static void D(string format, params object[] args)
        {
            if (logLevel < ELogLevel.INFO)
            {
                return;
            }

            ColorInfo(ConsoleColor.DarkGreen, format, args);
        }

        /// <summary>
        /// 打印彩色信息
        /// </summary>
        /// <param name="color"></param>
        /// <param name="message"></param>
        private static void ColorInfo(ConsoleColor color, object message)
        {
            var old = Console.ForegroundColor;
            Console.ForegroundColor = color;
            LogLine(message);
            Console.ForegroundColor = old;
        }

        /// <summary>
        /// 打印彩色信息
        /// </summary>
        /// <param name="color"></param>
        /// <param name="message"></param>
        private static void ColorInfo(ConsoleColor color, string format, params object[] args)
        {
            var old = Console.ForegroundColor;
            Console.ForegroundColor = color;
            var s = args.Length > 0 ? string.Format(format, args) : format;
            LogLine(s);            
            Console.ForegroundColor = old;
        }
    }
}
