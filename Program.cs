using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SunDofus.Utilities;
using SunDofus.Auth.Entities;
using SunDofus.Auth.Entities.Requests;
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

                //TODO Start GameLogs on Console

                Logger.Write(Logger.LoggerType.Infos, "SunDofus @initialized@ and @started@!");
            }
            catch (Exception exception)
            {
                Logger.Write(Logger.LoggerType.Errors, exception.ToString());
            }

            while (true)
                Console.ReadLine();

            //if (Config.GetBool("REALM"))
            //{
            //    var realmthread = new Thread(() =>
            //        {
            //            try
            //            {
            //                Auth.Network.ServersHandler.InitialiseServers();
            //                Auth.Entities.DatabaseProvider.InitializeConnection();

            //                Logger.Console.Write(string.Format("Realm started in '{0}'s !", Basic.GetUpTime()[2]));
            //            }
            //            catch (Exception error)
            //            {
            //                Console.WriteLine(error);
            //            }
            //        });

            //    realmthread.Start();
            //}

            //if (Config.GetBool("WORLD"))
            //{
            //    var gamethread = new Thread(() =>
            //        {
            //            try
            //            {
            //                Console.Title = string.Format("{0} | Server '{1}'", Console.Title, Config.GetInt32("ServerID"));

            //                World.Entities.DatabaseProvider.Initialize();
            //                Databases.PreparedStatements.PrepareStatements();

            //                World.Entities.Requests.LevelsRequests.LoadLevels();

            //                World.Entities.Requests.ItemsRequests.LoadItems();
            //                World.Entities.Requests.ItemsRequests.LoadItemsSets();
            //                World.Entities.Requests.ItemsRequests.LoadUsablesItems();

            //                World.Entities.Requests.SpellsRequests.LoadSpells();
            //                World.Entities.Requests.SpellsRequests.LoadSpellsToLearn();

            //                World.Entities.Requests.MonstersRequests.LoadMonsters();
            //                World.Entities.Requests.MonstersRequests.LoadMonstersLevels();

            //                World.Entities.Requests.MapsRequests.LoadMaps();

            //                if(!Utilities.Config.GetBool("DEBUG"))
            //                    World.Entities.Requests.TriggersRequests.LoadTriggers();

            //                World.Entities.Requests.ZaapsRequests.LoadZaaps();
            //                World.Entities.Requests.ZaapisRequests.LoadZaapis();

            //                World.Entities.Requests.NoPlayerCharacterRequests.LoadNPCsAnswers();
            //                World.Entities.Requests.NoPlayerCharacterRequests.LoadNPCsQuestions();
            //                World.Entities.Requests.NoPlayerCharacterRequests.LoadNPCs();

            //                World.Entities.Requests.BanksRequests.LoadBanks();
            //                World.Entities.Requests.CharactersRequests.LoadCharacters();
            //                World.Entities.Requests.GuildsRequest.LoadGuilds();
            //                World.Entities.Requests.CollectorsRequests.LoadCollectors();

            //                World.Network.ServersHandler.InitialiseServers();

            //                World.Entities.Requests.AuthsRequests.LoadAuths();

            //                World.Game.World.Save.InitSaveThread();

            //                Logger.Console.Write(string.Format("World started in '{0}'s !", Basic.GetUpTime()[2]));
            //            }
            //            catch (Exception error)
            //            {
            //                Console.WriteLine(error);
            //            }
            //        });

            //    gamethread.Start();
            //}

            //while (true)
            //{
            //    Console.ReadKey();
            //    Logger.Console.Write(string.Format("Uptime : Hours : {0} - Minutes : {1} - Seconds : {2}", 
            //        Basic.GetUpTime()[0], Basic.GetUpTime()[1], Basic.GetUpTime()[2]));
            //}
        }
    }
}
