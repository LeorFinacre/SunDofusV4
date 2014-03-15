using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Transactions;
using SunDofus.World.Entities;
using MySql.Data.MySqlClient;

namespace SunDofus.World.Game.World
{
    class Save
    {
        private static Timer timer;

        public static void InitSaveThread()
        {
            timer = new Timer((e) =>
            {
                SaveWorld();
                timer.Change(Utilities.Config.GetInt32("AUTOSAVETIME") * 1 * 1000, Timeout.Infinite);
            }, null, Utilities.Config.GetInt32("AUTOSAVETIME") * 1 * 1000, Timeout.Infinite);
        }

        public static void SaveWorld()
        {
            Servers.GAMESERVER_STATE = Servers.GameState.OnMaintenance;
            //SunDofus.World.Entities.Requests.CharactersRequests.CharactersList.Where(x => x.IsConnected).ToList().ForEach(x => x.NClient.Send("Im1164"));

            try
            {
                using (TransactionScope transaction = new TransactionScope())
                {
                    DatabaseProvider.CreateConnection().EnlistTransaction(Transaction.Current);
                    SaveChararacters();
                    SaveGuilds();
                    SaveCollectors();
                    SaveBanks();
                    transaction.Complete();
                }
            }
            catch (Exception ex)
            {
                //Save Into SQL-Files

            }

            //SunDofus.World.Entities.Requests.CharactersRequests.CharactersList.Where(x => x.IsConnected).ToList().ForEach(x => x.NClient.Send("Im1165"));
            Servers.GAMESERVER_STATE = Servers.GameState.IsConnected;
        }

        private static void SaveChararacters()
        {
            foreach (var character in SunDofus.World.Entities.Requests.CharactersRequests.CharactersList)
                Entities.Requests.CharactersRequests.SaveCharacter(character);
        }

        private static void SaveGuilds()
        {
            foreach (var guild in Entities.Requests.GuildsRequest.GuildsList)
                Entities.Requests.GuildsRequest.SaveGuild(guild);
        }

        private static void SaveCollectors()
        {
            foreach (var collector in Entities.Requests.CollectorsRequests.CollectorsList)
                Entities.Requests.CollectorsRequests.SaveCollector(collector);
        }

        private static void SaveBanks()
        {
            foreach (var bank in Entities.Requests.BanksRequests.BanksList)
                Entities.Requests.BanksRequests.SaveBank(bank);
        }
    }
}