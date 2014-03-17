using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SunDofus.Utilities;
using System.Reflection;
using System.Threading;
using TinyCore;

namespace SunDofus
{
    class Program
    {
        static void Main(string[] args)
        {
            Basic.Uptime = Environment.TickCount;
            Console.Title = string.Concat("SunDofus v", Config.Version(Assembly.GetExecutingAssembly().FullName.Split(',')[1].Replace("Version=", "").Trim()));

            MyConsole.DrawHeader();
            Logger.Initialize();

            try
            {
                Config.LoadConfiguration();

                OProvider.Initialize("localhost", "root", "", "sundofus");
                Logger.Write(Logger.LoggerType.Infos, "@Connected@ to the @database@!");

                Servers.InitializeServers();

                //START Load DB

                Servers.LoadBannedIPs();
                Servers.LoadLevels();
                Servers.LoadMaps();
                Servers.LoadCharacters();

                //END Load DB

                Servers.InitializeAutoSave();

                Logger.Write(Logger.LoggerType.Infos, "SunDofus @initialized@ and @started@!");

                if (!Config.GetBool("DEBUG"))
                    MyConsole.InitializeGameStats();
            }
            catch (Exception exception)
            {
                Logger.Write(Logger.LoggerType.Errors, exception.ToString());
            }

            while (true)
                Console.ReadLine();
        }
    }
}
