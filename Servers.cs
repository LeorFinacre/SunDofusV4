﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunDofus.DataRecords;
using SunDofus.Network;
using SunDofus.Network.Clients;
using SunDofus.Utilities;

namespace SunDofus
{
    class Servers
    {
        public enum GameState
        {
            IsDisconnected = 0,
            IsConnected = 1,
            OnMaintenance = 2
        }

        public static TCPServer RealmServer;
        public static TCPServer GameServer;

        public static int GAMESERVER_ID;
        public static GameState GAMESERVER_STATE;

        public static void InitializeServers()
        {
            RealmServer = new TCPServer(Config.GetString("LOGIN_IP"), Config.GetInt32("LOGIN_PORT"));
            RealmServer.OnAcceptSocketEvent += (s) =>
            {
                Logger.Write(Logger.LoggerType.Debug, "New inputted realm connection [@'{0}'@]!", s.IP);

                lock (RealmServer.Clients)
                    RealmServer.Clients.Add(new RealmClient(s));
            };
            RealmServer.OnListeningEvent += () =>
            {
                Logger.Write(Logger.LoggerType.Infos, "Realmserver started on the port [@'{0}'@]!", Config.GetInt32("LOGIN_PORT"));
            };
            RealmServer.OnListeningFailedEvent += (e) =>
            {
                throw new Exception(string.Format("Realmserver can't start : {0}", e.Message));
            };
            RealmServer.Start();


            GameServer = new TCPServer(Config.GetString("GAME_IP"), Config.GetInt32("GAME_PORT"));
            GameServer.OnAcceptSocketEvent += (s) =>
            {
                Logger.Write(Logger.LoggerType.Debug, "New inputted game connection [@'{0}'@]!", s.IP);

                lock (RealmServer.Clients)
                    RealmServer.Clients.Add(new GameClient(s));
            };
            GameServer.OnListeningEvent += () =>
            {
                Logger.Write(Logger.LoggerType.Infos, "Gameserver started on the port [@'{0}'@]!", Config.GetInt32("GAME_PORT"));
            };
            GameServer.OnListeningFailedEvent += (e) =>
            {
                throw new Exception(string.Format("Gameserver can't start : {0}", e.Message));
            };
            GameServer.Start();

            GAMESERVER_ID = Config.GetInt32("GAME_ID");
            GAMESERVER_STATE = GameState.IsConnected;

            GameKeys = new List<GameKey>();
            Maps = new List<DB_Map>();
        }

        public static List<DB_BannedIP> BannedIPs;
        public static List<DB_Character> Characters;
        public static List<DB_Level> Levels;
        public static List<DB_Map> Maps;

        #region GameKeys

        public static List<GameKey> GameKeys;

        public class GameKey
        {
            public DB_Account Account { get; private set; }
            public string Key { get; private set; }

            public GameKey(DB_Account acc, string key)
            {
                Account = acc;
                Key = key;
            }
        }

        #endregion

        #region LoadWorld

        public static void LoadBannedIPs()
        {
            MyConsole.StartLoading("Loading bannedIPs");
            BannedIPs = DB_BannedIP.Find<DB_BannedIP>();
            MyConsole.StopLoading();
        }

        public static void LoadCharacters()
        {
            MyConsole.StartLoading("Loading characters");
            Characters = DB_Character.Find<DB_Character>();
            MyConsole.StopLoading();
        }

        public static void LoadLevels()
        {
            MyConsole.StartLoading("Loading levels");
            Levels = DB_Level.Find<DB_Level>();
            MyConsole.StopLoading();
        }

        public static void LoadMaps()
        {
            if (!Config.GetBool("DEBUG"))
            {
                MyConsole.StartLoading("Loading maps");
                Maps = DB_Map.Find<DB_Map>();
                Maps.ForEach(x => x.ParsePos());
                MyConsole.StopLoading();
            }
            else
                Logger.Write(Logger.LoggerType.Debug, "No loading maps, debug mode!");
        }

        #endregion
    }
}