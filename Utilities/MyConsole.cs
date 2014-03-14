using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SunDofus.Utilities
{
    class MyConsole
    {
        public static object ConsoleLock = new object();
        private static int m_LoadingX, m_LoadingY, m_Count;

        public static void DrawHeader()
        {
            lock (ConsoleLock)
            {
                Console.ForegroundColor = ConsoleColor.Green;

                Console.WriteLine();
                Console.WriteLine("                  _____             _____         __");
                Console.WriteLine(@"                 / ____|           |  __ \       / _|");
                Console.WriteLine("                | (___  _   _ _ __ | |  | | ___ | |_ _   _ ___ ");
                Console.WriteLine(@"                 \___ \| | | | '_ \| |  | |/ _ \|  _| | | / __|");
                Console.WriteLine(@"                 ____) | |_| | | | | |__| | (_) | | | |_| \__ \");
                Console.WriteLine(@"                |_____/ \__,_|_| |_|_____/ \___/|_|  \__,_|___/");
                Console.WriteLine("                                         __ By Ghost [Alias Gh0ster, Shaak]");
                Console.WriteLine();

                Console.WriteLine("________________________________________________________________________________");

                Console.WriteLine();
            }
        }

        public static void StartLoading(string message)
        {
            lock (ConsoleLock)
                Logger.Write(Logger.LoggerType.Load, string.Concat(message, "..."), false);
        }

        public static void StopLoading()
        {
            lock (ConsoleLock)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(" OK!", m_Count, m_Count);
            }
        }
    }
}
