using SunDofus.Game.Maps.Fights;
using SunDofus.Game.Maps.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCore;

namespace SunDofus.DataRecords
{
    [OTable("maps")]
    public class DB_Map : ORecord
    {
        [OProperty("ID", OProperty.TinyPropertyType.INT, true)]
        public int ID { get; set; }
        [OProperty("date", OProperty.TinyPropertyType.STRING)]
        public string Date { get; set; }
        [OProperty("width", OProperty.TinyPropertyType.INT)]
        public int Width { get; set; }
        [OProperty("height", OProperty.TinyPropertyType.INT)]
        public int Height { get; set; }
        [OProperty("capabilities", OProperty.TinyPropertyType.INT)]
        public int Capabilities { get; set; }

        public int PosX { get; set; }
        public int PosY { get; set; }
        public int SubArea { get; set; }

        [OProperty("mapData", OProperty.TinyPropertyType.STRING)]
        public string MapData { get; set; }
        [OProperty("key", OProperty.TinyPropertyType.STRING)]
        public string Key { get; set; }
        [OProperty("mappos", OProperty.TinyPropertyType.STRING)]
        public string Mappos { get; set; }

        [OProperty("numgroup", OProperty.TinyPropertyType.INT)]
        public int MaxMonstersGroup { get; set; }
        [OProperty("groupsize", OProperty.TinyPropertyType.INT)]
        public int MaxGroupSize { get; set; }

        public Dictionary<int, List<int>> Monsters { get; private set; }

        public DB_Map()
        {
            RushablesCells = UncompressDatas();

            Characters = new List<DB_Character>();
            Triggers = new List<DB_Trigger>();
            Npcs = new List<DB_NPC>();
            Monsters = new Dictionary<int, List<int>>();
            MonstersGroups = new List<MonstersGroup>();
            Fights = new List<Fight>();

            if (Monsters.Count != 0 && RushablesCells.Count != 0)
                RefreshAllMonsters();
        }

        public void ParsePos()
        {
            try
            {
                var datas = Mappos.Split(',');

                if (datas.Length < 3)
                    return;

                PosX = int.Parse(datas[0]);
                PosY = int.Parse(datas[1]);
                SubArea = int.Parse(datas[2]);
            }
            catch { }
        }
        public List<DB_Character> Characters { get; set; }
        public List<DB_Trigger> Triggers { get; set; }
        public List<DB_NPC> Npcs { get; set; }
        public List<MonstersGroup> MonstersGroups { get; set; }
        public List<Fight> Fights { get; set; }
        public List<int> RushablesCells { get; set; }
        public DB_GuildCollector Collector { get; set; }

        private void RefreshAllMonsters()
        {
            for (int i = 1; i <= MaxMonstersGroup; i++)
                AddMonstersGroup();
        }

        public void AddMonstersGroup()
        {
            if (MonstersGroups.Count >= MaxMonstersGroup)
                return;

            lock(MonstersGroups)
                MonstersGroups.Add(new MonstersGroup(Monsters, this));
        }

        public string FormatFightCount()
        {
            return string.Concat("fC", Fights.Count);
        }

        public bool IsRushableCell(int cell)
        {
            return RushablesCells.Contains(cell);
        }

        public void Send(string message)
        {
            lock (Characters)
            {
                foreach (var character in Characters)
                    character.Send(message);
            }
        }

        public void AddPlayer(DB_Character character)
        {
            Send(string.Concat("GM|+", character.PatternDisplayChar()));

            character.Send(string.Concat("fC", Fights.Count)); //Fight

            lock (Characters)
                Characters.Add(character);

            if (Characters.Count > 0)
                character.Send(string.Concat("GM", CharactersPattern()));

            if (Npcs.Count > 0)
                character.Send(string.Concat("GM", NPCsPattern()));

            if (MonstersGroups.Count > 0)
                character.Send(string.Concat("GM", MonstersGroupsPattern()));

            if (Collector != null && !Collector.IsInFight)
                character.Send(string.Concat("GM", Collector.PatternMap()));

            if (Fights.Count > 0)
            {
                character.Send(FormatFightCount());

                foreach (Fight fight in Fights)
                {
                    if (fight.State == FightState.STARTING)
                    {
                        character.Send(fight.FormatFlagShow());
                        character.Send(fight.FormatFlagFighter(fight.Team1.GetFighters()));
                        character.Send(fight.FormatFlagFighter(fight.Team2.GetFighters()));

                        if (fight.Team1.IsToggle(ToggleType.LOCK))
                            character.Send(string.Concat("Go+A", fight.Team1.ID));
                        if (fight.Team1.IsToggle(ToggleType.HELP))
                            character.Send(string.Concat("Go+H", fight.Team1.ID));
                        if (fight.Team1.IsToggle(ToggleType.PARTY))
                            character.Send(string.Concat("Go+P", fight.Team1.ID));
                        if (fight.Team1.IsToggle(ToggleType.SPECTATOR))
                            character.Send(string.Concat("Go+S", fight.Team1.ID));

                        if (fight.Team2.IsToggle(ToggleType.LOCK))
                            character.Send(string.Concat("Go+A", fight.Team2.ID));
                        if (fight.Team2.IsToggle(ToggleType.HELP))
                            character.Send(string.Concat("Go+H", fight.Team2.ID));
                        if (fight.Team2.IsToggle(ToggleType.PARTY))
                            character.Send(string.Concat("Go+P", fight.Team2.ID));
                        if (fight.Team2.IsToggle(ToggleType.SPECTATOR))
                            character.Send(string.Concat("Go+S", fight.Team2.ID));
                    }
                }
            }
        }

        public void AddFight(Fight fight)
        {
            lock (Fights)
                Fights.Add(fight);
        }

        public void RemoveFight(Fight fight)
        {
            lock (Fights)
                Fights.Remove(fight);
        }

        public void DelPlayer(DB_Character character)
        {
            Send(string.Concat("GM|-", character.ID));

            lock(Characters)
                Characters.Remove(character);
        }

        public int NextNpcID()
        {
            var i = -1;

            while (Npcs.Any(x => x.ID == i) || MonstersGroups.Any(x => x.ID == i))
                i -= 1;

            return i;
        }

        public int NextFightID()
        {
            var i = 1;

            while (Fights.Any(x => x.ID == i))
                i++;

            return i;
        }

        private string CharactersPattern()
        {
            return string.Concat("|+", string.Join("|+", from c in Characters select c.PatternDisplayChar()));
        }

        private string NPCsPattern()
        {
            return string.Concat("|+", string.Join("|+", from n in Npcs select n.PatternOnMap()));
        }

        private string MonstersGroupsPattern()
        {
            return string.Concat("|+", string.Join("|+", from m in MonstersGroups select m.PatternOnMap()));
        }

        #region MapData

        private List<int> UncompressDatas()
        {
            List<int> newList = new List<int>();

            string data = DecypherData(MapData, "");

            for (int i = 0; i < data.Length; i += 10)
            {
                string CurrentCell = data.Substring(i, 10);
                byte[] CellInfo = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                for (int i2 = CurrentCell.Length - 1; i2 >= 0; i2--)
                    CellInfo[i2] = (byte)hash.IndexOf(CurrentCell[i2]);

                var type = (CellInfo[2] & 56) >> 3;

                if (type != 0)
                    newList.Add(i / 10);
            }

            return newList;
        }

        public static string DecypherData(string Data, string DecryptKey)
        {
            try
            {
                string result = string.Empty;

                if (DecryptKey != "")
                {
                    DecryptKey = PrepareKey(DecryptKey);
                    int checkSum = CheckSum(DecryptKey) * 2;

                    for (int i = 0, k = 0; i < Data.Length; i += 2)
                        result += (char)(int.Parse(Data.Substring(i, 2), System.Globalization.NumberStyles.HexNumber) ^ (int)(DecryptKey[(k++ + checkSum) % DecryptKey.Length]));

                    return Uri.UnescapeDataString(result);
                }
                else return Data;
            }
            catch { return ""; }
        }

        private static int CheckSum(string Data)
        {
            int result = 0;

            for (int i = 0; i < Data.Length; i++)
                result += Data[i] % 16;

            return result % 16;
        }

        private static string PrepareKey(string Key)
        {
            string keyResult = "";

            for (var i = 0; i < Key.Length; i += 2)
                keyResult += Convert.ToChar(int.Parse(Key.Substring(i, 2), System.Globalization.NumberStyles.HexNumber));

            return Uri.UnescapeDataString(keyResult);
        }

        private string hash = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";

        private int hashCodes(char a)
        {
            return hash.IndexOf(a);
        }

        private bool isValidCell(string datas)
        {
            var lengh = datas.Length - 1;
            var table = new int[5000];

            while (lengh >= 0)
            {
                table[lengh] = hashCodes(datas[lengh]);
                lengh -= 1;
            }

            return ((table[2] & 56) >> 3) != 0;
        }

        #endregion

        #region Zaaps

        public void SendZaaps(DB_Character character)
        {
            if (Servers.Zaaps.Any(x => x.MapID == character.MapID))
            {
                var zaap = Servers.Zaaps.First(x => x.MapID == character.MapID);

                if (!character.Zaaps.Contains(zaap))
                {
                    character.Zaaps.Add(zaap);
                    character.Send("Im024");
                }

                var savepos = (Servers.Zaaps.Any(x => x.MapID == character.SaveMap) ? Servers.Zaaps.First(x => x.MapID == character.SaveMap).MapID.ToString() : "");
                var packet = string.Format("WC{0}|", savepos);

                Servers.Zaaps.Where(x => character.Zaaps.Contains(x)).ToList().ForEach(x => packet = string.Format("{0}{1};{2}|", packet, x.MapID, CalcZaapPrice(character.Map, x.Map)));

                character.Send(packet.Substring(0, packet.Length - 1));
            }
            else
                character.Send("BN");
        }

        public void SaveZaap(DB_Character character)
        {
            if (Servers.Zaaps.Any(x => x.MapID == character.MapID))
            {
                var zaap = Servers.Zaaps.First(x => x.MapID == character.MapID);

                character.SaveMap = zaap.MapID;
                character.SaveCell = zaap.CellID;

                character.Send("Im06");
            }
            else
                character.Send("BN");
        }

        public void OnMoveZaap(DB_Character character, int nextZaap)
        {
            if (Servers.Zaaps.Any(x => x.MapID == nextZaap))
            {
                var zaap = Servers.Zaaps.First(x => x.MapID == nextZaap);

                var price = CalcZaapPrice(character.GetMap(), zaap.Map);

                character.Kamas -= price;
                character.Send(string.Concat("Im046;", price));
                character.TeleportNewMap(zaap.MapID, zaap.CellID);

                character.Send("WV");

                character.Send(character.ToString());
            }
            else
                character.Send("BN");
        }

        private int CalcZaapPrice(DB_Map startMap, DB_Map nextMap)
        {
            return (int)(10 * (Math.Abs(nextMap.PosX - startMap.PosX) + Math.Abs(nextMap.PosY - startMap.PosY)));
        }

        #endregion

        #region Zaapis

        public void SendZaapis(DB_Character character)
        {
            if (Servers.Zaapis.Any(x => x.MapID == character.MapID))
            {
                var zaapis = Servers.Zaapis.First(x => x.MapID == character.MapID);
                var packet = string.Format("Wc{0}|", character.MapID);

                if ((zaapis.Faction == 1 && character.Faction.FactionID == 2) || (zaapis.Faction == 2 && character.Faction.FactionID == 1))
                {
                    character.Send("Im1196");
                    return;
                }

                Servers.Zaapis.Where(x => x.Faction == zaapis.Faction).ToList().ForEach(x => packet = string.Concat(packet, x.MapID, (character.Faction.FactionID == x.Faction ? ";10|" : ";20|")));

                character.Send(packet);
            }
            else
                character.Send("BN");
        }

        public void OnMoveZaapis(DB_Character character, int nextZaapis)
        {
            if (Servers.Zaapis.Any(x => x.MapID == nextZaapis))
            {
                var zaapis = Servers.Zaapis.First(x => x.MapID == nextZaapis);

                if ((zaapis.Faction == 1 && character.Faction.FactionID == 2) || (zaapis.Faction == 2 && character.Faction.FactionID == 1))
                {
                    character.Send("Im1196");
                    return;
                }

                var price = (character.Faction.FactionID == zaapis.Faction ? 10 : 20);

                character.Kamas -= price;
                character.Send(string.Concat("Im046;", price));
                character.TeleportNewMap(zaapis.MapID, zaapis.CellID);

                character.Send("Wv");

                character.Send(character.ToString());
            }
            else
                character.Send("BN");
        }

        #endregion
    }
}
