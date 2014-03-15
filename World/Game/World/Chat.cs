using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SunDofus.Network.Clients;
using SunDofus.World.Game.Characters;

namespace SunDofus.World.Game.World
{
    class Chat
    {
        public static void SendGeneralMessage(GameClient client, string message)
        {
            if (client.Player.GetMap() == null) 
                return;

            if (message.Substring(0, 1) == ".")
            {
                //client.Commander.ParseChatCommand(message.Substring(1));
                return;
            }

            client.Player.GetMap().Send(string.Format("cMK|{0}|{1}|{2}", client.Player.ID, client.Player.Name, message));
        }

        public static void SendIncarnamMessage(GameClient client, string message)
        {
            if (!client.Player.IsInIncarnam || client.Player.Level > 30)
            {
                client.Send("Im0139");
                return;
            }

            foreach (GameClient character in Servers.GameServer.Clients.Where
                (x => (x as GameClient).Authentified == true && (x as GameClient).Player.IsInIncarnam))
            {
                character.Send(string.Format("cMK^|{0}|{1}|{2}", client.Player.ID, client.Player.Name, message));
            }
        }

        public static void SendPrivateMessage(GameClient client, string receiver, string message)
        {
            if (SunDofus.World.Entities.Requests.CharactersRequests.CharactersList.Any(x => x.Name == receiver))
            {
                var character = SunDofus.World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.Name == receiver);

                if (character.IsConnected == true && !character.NClient.Enemies.Contains(client.Infos.Pseudo))
                {
                    character.NClient.Send(string.Format("cMKF|{0}|{1}|{2}", client.Player.ID, client.Player.Name, message));
                    client.Send(string.Format("cMKT|{0}|{1}|{2}", client.Player.ID, character.Name, message));
                }
                else
                    client.Send(string.Concat("cMEf", receiver));
            }
        }

        public static void SendTradeMessage(GameClient client, string message)
        {
            if (client.Player.CanSendinTrade() == true)
            {
                foreach (GameClient character in Servers.GameServer.Clients.Where(x => (x as GameClient).Authentified == true))
                    character.Send(string.Format("cMK:|{0}|{1}|{2}", client.Player.ID, client.Player.Name, message));

                client.Player.RefreshTrade();
            }
            else
                client.Send(string.Concat("Im0115;", client.Player.TimeTrade()));
        }

        public static void SendRecruitmentMessage(GameClient client, string message)
        {
            if (client.Player.CanSendinRecruitment() == true)
            {
                foreach (GameClient character in Servers.GameServer.Clients.Where(x => (x as GameClient).Authentified == true))
                    character.Send(string.Format("cMK?|{0}|{1}|{2}", client.Player.ID, client.Player.Name, message));

                client.Player.RefreshRecruitment();
            }
            else
                client.Send(string.Concat("Im0115;", client.Player.TimeRecruitment()));
        }

        public static void SendFactionMessage(GameClient client, string message)
        {
            if (client.Player.Faction.ID != 0 && client.Player.Faction.Level >= 3)
            {
                foreach (GameClient character in Servers.GameServer.Clients.Where(x => (x as GameClient).Authentified == true && (x as GameClient).Player.Faction.ID == client.Player.Faction.ID))
                    character.Send(string.Format("cMK!|{0}|{1}|{2}", client.Player.ID, client.Player.Name, message));
            }
            else
                client.Send("BN");
        }

        public static void SendPartyMessage(GameClient client, string message)
        {
            if (client.Player.State.Party != null)
            {
                foreach (var character in client.Player.State.Party.Members.Keys)
                    character.NClient.Send(string.Format("cMK$|{0}|{1}|{2}", client.Player.ID, client.Player.Name, message));
            }
            else
                client.Send("BN");
        }

        public static void SendGuildMessage(GameClient client, string message)
        {
            if (client.Player.Guild != null)
            {
                foreach (var character in client.Player.Guild.Members.Where(x => x.Character.IsConnected))
                    character.Character.NClient.Send(string.Format("cMK%|{0}|{1}|{2}", client.Player.ID, client.Player.Name, message));
            }
            else
                client.Send("BN");
        }

        public static void SendAdminMessage(GameClient client, string message)
        {
            if (client.Infos.GMLevel > 0)
            {
                foreach (GameClient character in Servers.GameServer.Clients.Where(x => (x as GameClient).Authentified == true && (x as GameClient).Infos.GMLevel > 0))
                    character.Send(string.Format("cMK@|{0}|{1}|{2}", client.Player.ID, client.Player.Name, message));
            }
            else
                client.Send("BN");
        }
    }
}
