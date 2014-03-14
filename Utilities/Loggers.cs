using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SunDofus.Utilities
{
    class Logger
    {
        public enum LoggerType
        {
            Debug = ConsoleColor.Magenta,
            Errors = ConsoleColor.Red,
            Infos = ConsoleColor.Green,
            Load = ConsoleColor.White
        }

        public static void Initialize()
        {
            if (File.Exists("logs.log"))
                File.Delete("logs.log");
        }

        public static void Write(LoggerType type, string message, bool line = true)
        {
            lock (MyConsole.ConsoleLock)
            {
                if (type == LoggerType.Debug && !Config.GetBool("DEBUG"))
                    return;

                Console.ForegroundColor = (ConsoleColor)type;
                var high = false;

                message = string.Format("[{0}] -> {1}", type.ToString(), message);

                foreach (var c in message.ToCharArray())
                {
                    if (high & c == '@')
                    {
                        Console.ForegroundColor = (ConsoleColor)type;
                        high = false;
                    }
                    else if (!high && c == '@')
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        high = true;
                    }
                    else
                        Console.Write(c);
                }

                if (line)
                    Console.WriteLine();
            }

            File.AppendAllText("logs.log", string.Format("{0} [{1}]-> {2}{3}", DateTime.Now, type.ToString(), message, Environment.NewLine));
        }

        public static void Write(LoggerType type, string format, params object[] args)
        {
            Write(type, string.Format(format, args));
        }
    }
}
