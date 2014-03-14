﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using SunDofus.World.Game.Characters;
using SunDofus.Databases;

namespace SunDofus.World.Entities.Requests
{
    class CharactersRequests
    {
        // We need to use a ConcurrentBag here as the Save thread will be accessing this list while we might still be writing to it.
        public static ConcurrentBag<Character> CharactersList = new ConcurrentBag<Character>();

        public static void LoadCharacters()
        {
            var connection = DatabaseProvider.CreateConnection();
            var sqlText = "SELECT * FROM characters";
            var sqlCommand = new MySqlCommand(sqlText, connection);

            var sqlResult = sqlCommand.ExecuteReader();

            while (sqlResult.Read())
            {
                var character = new Game.Characters.Character()
                {
                    ID = sqlResult.GetInt16("id"),
                    Name = sqlResult.GetString("name"),
                    Level = sqlResult.GetInt16("level"),
                    Class = sqlResult.GetInt16("class"),
                    Sex = sqlResult.GetInt16("sex"),
                    Size = 100,
                    Color = sqlResult.GetInt32("color"),
                    Color2 = sqlResult.GetInt32("color2"),
                    Color3 = sqlResult.GetInt32("color3"),

                    MapCell = int.Parse(sqlResult.GetString("mappos").Split(',')[1]),
                    MapID = int.Parse(sqlResult.GetString("mappos").Split(',')[0]),
                    Dir = int.Parse(sqlResult.GetString("mappos").Split(',')[2]),

                    Exp = sqlResult.GetInt64("experience"),

                    SaveState = EntityState.Unchanged

                };

                character.Skin = int.Parse(string.Concat(character.Class, character.Sex));
                character.ParseStats(sqlResult.GetString("stats"));

                if (sqlResult.GetString("items") != "")
                    character.ItemsInventary.ParseItems(sqlResult.GetString("items"));

                if (sqlResult.GetString("spells") != "")
                    character.SpellsInventary.ParseSpells(sqlResult.GetString("spells"));

                var factionInfos = sqlResult.GetString("faction").Split(';');
                character.Faction.ID = int.Parse(factionInfos[0]);
                character.Faction.Honor = int.Parse(factionInfos[1]);
                character.Faction.Deshonor = int.Parse(factionInfos[2]);

                foreach (var zaap in sqlResult.GetString("zaaps").Split(';'))
                {
                    if (zaap == "")
                        continue;

                    character.Zaaps.Add(int.Parse(zaap));
                }

                var savepos = sqlResult.GetString("savepos").Split(';');
                character.SaveMap = int.Parse(savepos[0]);
                character.SaveCell = int.Parse(savepos[1]);

                if (character.Faction.Honor > Entities.Requests.LevelsRequests.LevelsList.OrderByDescending(x => x.Alignment).ToArray()[0].Alignment)
                    character.Faction.Level = 10;
                else
                    character.Faction.Level = Entities.Requests.LevelsRequests.LevelsList.Where(x => x.Alignment <= character.Faction.Honor).OrderByDescending(x => x.Alignment).ToArray()[0].ID;

                CharactersList.Add(character);
            }

            sqlResult.Close();
        }

        public static void SaveCharacter(Game.Characters.Character character)
        {
            if (character.SaveState == EntityState.Unchanged)
                return;

            MySqlCommand command = null;
            if (character.SaveState == EntityState.New)
                command = PreparedStatements.GetQuery(Queries.InsertNewCharacter);
            else if (character.SaveState == EntityState.Modified)
                command = PreparedStatements.GetQuery(Queries.UpdateCharacter);
            else
                command = PreparedStatements.GetQuery(Queries.DeleteCharacter);

            command.Parameters["@id"].Value = character.ID;
            command.Parameters["@name"].Value = character.Name;
            command.Parameters["@level"].Value = character.Level;
            command.Parameters["@class"].Value = character.Class;
            command.Parameters["@sex"].Value = character.Sex;
            command.Parameters["@color"].Value = character.Color;
            command.Parameters["@color2"].Value = character.Color2;
            command.Parameters["@color3"].Value = character.Color3;
            command.Parameters["@mapinfos"].Value = character.MapID + "," + character.MapCell + "," + character.Dir;
            command.Parameters["@stats"].Value = character.SqlStats();
            command.Parameters["@items"].Value = character.GetItemsToSave();
            command.Parameters["@spells"].Value = character.SpellsInventary.SaveSpells();
            command.Parameters["@exp"].Value = character.Exp;
            command.Parameters["@faction"].Value = string.Concat(character.Faction.ID, ";",
                character.Faction.Honor, ";", character.Faction.Deshonor);
            command.Parameters["@zaaps"].Value = string.Join(";", character.Zaaps);
            command.Parameters["@savepos"].Value = string.Concat(character.SaveMap, ";", character.SaveCell);

            command.ExecuteNonQuery();

            character.SaveState = EntityState.Unchanged;
        }

        public static bool ExistsName(string name)
        {
            return CharactersList.Any(x => x.Name == name);
        }
    }
}