﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunDofus.DataRecords
{
    class DB_CharacterFriend
    {
        private Character character;

        public bool WillNotifyWhenConnected { get; set; }

        public CharacterFriends(Character character)
        {
            this.character = character;
        }

        public void SendFriends()
        {
            var packet = "FL|";

            foreach (var friend in character.NClient.Friends)
            {
                if (Servers.GameServer.Clients.Any(x => (x as GameClient).Infos.Pseudo == friend && (x as GameClient).Characters.Any(c => c.IsConnected == true)))
                {
                    packet = string.Concat(packet, friend);

                    var charact = (Servers.GameServer.Clients.First(x => (x as GameClient).Infos.Pseudo == friend) as GameClient).Player;
                    bool seeLevel = (charact.NClient.Friends.Contains(character.NClient.Infos.Pseudo) ? true : false);

                    packet = string.Format("{0};?;{1};{2};{3};{4};{5};{6}|", packet, charact.Name, (seeLevel ? charact.Level.ToString() : "?"), (seeLevel ? charact.Faction.ID.ToString() : "-1"),
                        charact.Class.ToString(), charact.Sex.ToString(), charact.Skin.ToString());
                }
                else
                    packet = string.Concat(packet, friend, "|");
            }

            character.NClient.Send(packet.Substring(0, packet.Length - 1));
        }

        public void AddFriend(string datas)
        {
            if (Servers.GameServer.Clients.Any(x => (x as GameClient).Characters.Any(f => f.Name == datas)))
            {
                var charact = Servers.GameServer.Clients.First(x => (x as GameClient).Characters.Any(f => f.Name == datas)) as GameClient;

                if (!character.NClient.Friends.Contains(charact.Infos.Pseudo))
                {
                    character.NClient.Friends.Add(charact.Infos.Pseudo);
                    bool seeLevel = (charact.Friends.Contains(character.NClient.Infos.Pseudo) ? true : false);

                    var packet = string.Format("{0};?;{1};{2};{3};{4};{5};{6}|", charact.Infos.Pseudo, charact.Player.Name, (seeLevel ? charact.Player.Level.ToString() : "?"), (seeLevel ? charact.Player.Faction.ID.ToString() : "-1"),
                        charact.Player.Class.ToString(), charact.Player.Sex.ToString(), charact.Player.Skin.ToString());

                    character.NClient.Send(string.Concat("FAK", packet));

                    //Insert FriendDB
                    //Network.ServersHandler.AuthLinks.Send(new Network.Auth.Packets.CreatedFriendPacket().GetPacket(character.NClient.Infos.ID, charact.Infos.Pseudo));
                }
                else
                    character.NClient.Send("FAEa");
            }
            else
                character.NClient.Send("FAEf");
        }

        public void RemoveFriend(string datas)
        {
            var name = datas.Substring(1);

            if (datas.Substring(0, 1) == "*")
            {
                if (character.NClient.Friends.Contains(name))
                {
                    character.NClient.Friends.Remove(name);
                    character.NClient.Send("FDK");

                    //Delete friend db
                    //Network.ServersHandler.AuthLinks.Send(new Network.Auth.Packets.DeletedFriendPacket().GetPacket(character.NClient.Infos.ID, name));
                }
                else
                    character.NClient.Send("FDEf");
            }
            else if(datas.Substring(0,1) == "%")
            {
                if (Servers.GameServer.Clients.Any(x => (x as GameClient).Characters.Any(f => f.Name == name)))
                {
                    var client = Servers.GameServer.Clients.First(x => (x as GameClient).Characters.Any(f => f.Name == name)) as GameClient;

                    if (character.NClient.Friends.Contains(client.Infos.Pseudo))
                    {
                        character.NClient.Friends.Remove(client.Infos.Pseudo);
                        character.NClient.Send("FDK");

                        //Delete friend db
                        //Network.ServersHandler.AuthLinks.Send(new Network.Auth.Packets.DeletedFriendPacket().GetPacket(character.NClient.Infos.ID, client.Infos.Pseudo));
                    }
                    else
                        character.NClient.Send("FDEf");
                }
                else
                    character.NClient.Send("FDEf");
            }
        }
    }
}