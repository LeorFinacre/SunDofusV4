﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SunDofus.Network.Clients;

namespace SunDofus.World.Game.Characters
{
    class CharacterEnemies
    {
        private Character character;

        public CharacterEnemies(Character _character)
        {
            character = _character;
        }

        public void SendEnemies()
        {
            var packet = "iL|";

            foreach (var enemy in character.NClient.Enemies)
            {
                if (Servers.GameServer.Clients.Any(x => (x as GameClient).Infos.Pseudo == enemy && (x as GameClient).Characters.Any(c => c.IsConnected == true)))
                {
                    packet = string.Concat(packet, enemy);

                    var charact = (Servers.GameServer.Clients.First(x => (x as GameClient).Infos.Pseudo == enemy) as GameClient).Player;
                    bool seeLevel = (charact.NClient.Friends.Contains(character.NClient.Infos.Pseudo) ? true : false);
                    
                    packet = string.Format("{0};?;{1};{2};{3};{4};{5};{6}|", packet, charact.Name, (seeLevel ? charact.Level.ToString() : "?"), (seeLevel ? charact.Faction.ID.ToString() : "-1"),
                        charact.Class.ToString(), charact.Sex.ToString(), charact.Skin.ToString());
                }
                else
                    packet = string.Concat(packet, enemy, "|");
            }

            character.NClient.Send(packet.Substring(0, packet.Length - 1));
        }

        public void AddEnemy(string datas)
        {
            if (Servers.GameServer.Clients.Any(x => (x as GameClient).Characters.Any(f => f.Name == datas)))
            {
                var charact = Servers.GameServer.Clients.First(x => (x as GameClient).Characters.Any(f => f.Name == datas)) as GameClient;

                if (!character.NClient.Enemies.Contains(charact.Infos.Pseudo))
                {
                    character.NClient.Enemies.Add(charact.Infos.Pseudo);
                    character.NClient.Send(string.Format("iAK{0};2;{1};36;10;0;100.FL.", charact.Infos.Pseudo, charact.Player.Name));

                    //Insert Enemy DB
                    //Network.ServersHandler.AuthLinks.Send(new Network.Auth.Packets.CreatedEnemyPacket().GetPacket(character.NClient.Infos.ID, charact.Infos.Pseudo));
                }
                character.NClient.Send("iAEA");
            }
            else
                character.NClient.Send("FDEf");
        }

        public void RemoveEnemy(string datas)
        {
            var name = datas.Substring(1);

            if (datas.Substring(0, 1) == "*")
            {
                if (character.NClient.Enemies.Contains(name))
                {
                    character.NClient.Enemies.Remove(name);
                    character.NClient.Send("iDK");

                    //Delete EnemyDB
                    //Network.ServersHandler.AuthLinks.Send(new Network.Auth.Packets.DeletedEnemyPacket().GetPacket(character.NClient.Infos.ID, name));
                }
                else
                    character.NClient.Send("FDEf");
            }
            else if (datas.Substring(0, 1) == "%")
            {
                if (Servers.GameServer.Clients.Any(x => (x as GameClient).Characters.Any(f => f.Name == name)))
                {
                    var client = Servers.GameServer.Clients.First(x => (x as GameClient).Characters.Any(f => f.Name == name)) as GameClient;

                    if (character.NClient.Enemies.Contains(client.Infos.Pseudo))
                    {
                        character.NClient.Enemies.Remove(client.Infos.Pseudo);
                        character.NClient.Send("iDK");

                        //Delete EnemyDB
                        //Network.ServersHandler.AuthLinks.Send(new Network.Auth.Packets.DeletedEnemyPacket().GetPacket(character.NClient.Infos.ID, client.Infos.Pseudo));
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
