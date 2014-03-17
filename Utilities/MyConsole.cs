using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SunDofus.Utilities
{
    class MyConsole
    {
        public static object ConsoleLock = new object();

        private int m_NBPlayersOnline;
        private int m_NBAdminsOnline;
        private int m_NBCharactersWon;

        public int NBPlayersOnline
        {
            get
            {
                return m_NBPlayersOnline;
            }
            set
            {
                m_NBPlayersOnline = value;
                SetProperty(1, value.ToString(), true);
            }
        }

        public int NBAdminsOnline
        {
            get
            {
                return m_NBAdminsOnline;
            }
            set
            {
                m_NBAdminsOnline = value;
                SetProperty(2, value.ToString(), true);
            }
        }

        public int NBCharactersWon
        {
            get
            {
                return m_NBCharactersWon;
            }
            set
            {
                m_NBCharactersWon = value;
                SetProperty(1, value.ToString(), false);
            }
        }

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
                Console.WriteLine(" OK!");
            }
        }

        public static void InitializeGameStats()
        {
            Console.Clear();

            DrawHeader();
            SetName(GameStats.Uptime, "Uptime", false);
            SetName(GameStats.Characters_Earned, "Characters Earned", false);
            SetName(GameStats.Players_Connected, "Players Online", true);
            SetName(GameStats.Admin_Connected, "Admin Online", true);

            Console.CursorVisible = false;

            var uptimeTimer = new Timer(1000);

            uptimeTimer.Elapsed += (s, e) =>
                {
                    var up = Basic.GetUpTime();
                    SetProperty(1, string.Format("{0}h {1}m {2}s", up[0], up[1], up[2]), false);
                };

            uptimeTimer.Start();
        }

        private enum GameStats
        {
            Uptime,
            Characters_Earned,
            Players_Connected,
            Admin_Connected
        }

        private static void SetName(GameStats stat, string value, bool right)
        {
            lock (ConsoleLock)
            {
                var id = 0;

                switch (stat)
                {
                    case GameStats.Uptime:
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        id = 1;
                        break;

                    case GameStats.Characters_Earned:
                        Console.ForegroundColor = ConsoleColor.Green;
                        id = 2;
                        break;

                    case GameStats.Players_Connected:
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        id = 1;
                        break;

                    case GameStats.Admin_Connected:
                        Console.ForegroundColor = ConsoleColor.White;
                        id = 2;
                        break;
                }

                Console.CursorTop = 10 + (id * 4);
                Console.CursorLeft = right ? 46 : 8;

                Console.Write(value);

                SetProperty(id, "0", right);
            }
        }

        private static void SetProperty(int id, string value, bool right)
        {
            if (Config.GetBool("DEBUG"))
                return;

            lock (ConsoleLock)
            {
                Console.ForegroundColor = ConsoleColor.Gray;

                Console.CursorTop = 10 + (id * 4);
                Console.CursorLeft = right ? 66 : 28;

                Console.Write(value);
            }
        }
    }
}
