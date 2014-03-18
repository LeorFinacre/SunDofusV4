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
            Monsters = new Dictionary<int, List<int>>();
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
        public List<Fights.Fight> Fights { get; set; }
        public List<int> RushablesCells { get; set; }
        public Guilds.GuildCollector Collector { get; set; }

        public Entities.Models.Maps.MapModel Model { get; set; }

        public Map(Entities.Models.Maps.MapModel map)
        {
            Model = map;

            RushablesCells = UncompressDatas();

            Characters = new List<DB_Character>();
            Triggers = new List<Entities.Models.Maps.TriggerModel>();
            Npcs = new List<Characters.NPC.NPCMap>();
            MonstersGroups = new List<Monsters.MonstersGroup>();
            Fights = new List<Fights.Fight>();

            if (Model.Monsters.Count != 0 && RushablesCells.Count != 0)
                RefreshAllMonsters();
        }

        private void RefreshAllMonsters()
        {
            for (int i = 1; i <= Model.MaxMonstersGroup; i++)
                AddMonstersGroup();
        }

        public void AddMonstersGroup()
        {
            if (MonstersGroups.Count >= Model.MaxMonstersGroup)
                return;

            lock(MonstersGroups)
                MonstersGroups.Add(new Monsters.MonstersGroup(Model.Monsters, this));
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

            character.NClient.Send(string.Concat("fC", Fights.Count)); //Fight

            lock (Characters)
                Characters.Add(character);

            if (Characters.Count > 0)
                character.NClient.Send(string.Concat("GM", CharactersPattern()));

            if (Npcs.Count > 0)
                character.NClient.Send(string.Concat("GM", NPCsPattern()));

            if (MonstersGroups.Count > 0)
                character.NClient.Send(string.Concat("GM", MonstersGroupsPattern()));

            if (Collector != null && !Collector.IsInFight)
                character.NClient.Send(string.Concat("GM", Collector.PatternMap()));

            if (Fights.Count > 0)
            {
                character.NClient.Send(FormatFightCount());

                foreach (Fight fight in Fights)
                {
                    if (fight.State == FightState.STARTING)
                    {
                        character.NClient.Send(fight.FormatFlagShow());
                        character.NClient.Send(fight.FormatFlagFighter(fight.Team1.GetFighters()));
                        character.NClient.Send(fight.FormatFlagFighter(fight.Team2.GetFighters()));

                        if (fight.Team1.IsToggle(ToggleType.LOCK))
                            character.NClient.Send(string.Concat("Go+A", fight.Team1.ID));
                        if (fight.Team1.IsToggle(ToggleType.HELP))
                            character.NClient.Send(string.Concat("Go+H", fight.Team1.ID));
                        if (fight.Team1.IsToggle(ToggleType.PARTY))
                            character.NClient.Send(string.Concat("Go+P", fight.Team1.ID));
                        if (fight.Team1.IsToggle(ToggleType.SPECTATOR))
                            character.NClient.Send(string.Concat("Go+S", fight.Team1.ID));

                        if (fight.Team2.IsToggle(ToggleType.LOCK))
                            character.NClient.Send(string.Concat("Go+A", fight.Team2.ID));
                        if (fight.Team2.IsToggle(ToggleType.HELP))
                            character.NClient.Send(string.Concat("Go+H", fight.Team2.ID));
                        if (fight.Team2.IsToggle(ToggleType.PARTY))
                            character.NClient.Send(string.Concat("Go+P", fight.Team2.ID));
                        if (fight.Team2.IsToggle(ToggleType.SPECTATOR))
                            character.NClient.Send(string.Concat("Go+S", fight.Team2.ID));
                    }
                }
            }
        }

        public void AddFight(Fights.Fight fight)
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

            string data = DecypherData(Model.MapData, "");

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

            for (int i = 0; i < Key.Length; i += 2)
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
            if (Entities.Requests.ZaapsRequests.ZaapsList.Any(x => x.MapID == character.MapID))
            {
                var zaap = Entities.Requests.ZaapsRequests.ZaapsList.First(x => x.MapID == character.MapID);

                if (!character.Zaaps.Contains(zaap.MapID))
                {
                    character.Zaaps.Add(zaap.MapID);
                    character.NClient.Send("Im024");
                }

                var savepos = (Entities.Requests.ZaapsRequests.ZaapsList.Any(x => x.MapID == character.SaveMap) ?
                    Entities.Requests.ZaapsRequests.ZaapsList.First(x => x.MapID == character.SaveMap).MapID.ToString() : "");
                var packet = string.Format("WC{0}|", savepos);

                Entities.Requests.ZaapsRequests.ZaapsList.Where(x => character.Zaaps.Contains(x.MapID)).ToList().
                    ForEach(x => packet = string.Format("{0}{1};{2}|", packet, x.MapID, CalcPrice(character.GetMap(), x.Map)));

                character.NClient.Send(packet.Substring(0, packet.Length - 1));
            }
            else
                character.NClient.Send("BN");
        }

        public void SaveZaap(DB_Character character)
        {
            if (Entities.Requests.ZaapsRequests.ZaapsList.Any(x => x.MapID == character.MapID))
            {
                var zaap = Entities.Requests.ZaapsRequests.ZaapsList.First(x => x.MapID == character.MapID);

                character.SaveMap = zaap.MapID;
                character.SaveCell = zaap.CellID;

                character.NClient.Send("Im06");
            }
            else
                character.NClient.Send("BN");
        }

        public void OnMoveZaap(DB_Character character, int nextZaap)
        {
            if (Entities.Requests.ZaapsRequests.ZaapsList.Any(x => x.MapID == nextZaap))
            {
                var zaap = Entities.Requests.ZaapsRequests.ZaapsList.First(x => x.MapID == nextZaap);

                var price = CalcPrice(character.GetMap(), zaap.Map);

                character.Kamas -= price;
                character.NClient.Send(string.Concat("Im046;", price));
                character.TeleportNewMap(zaap.MapID, zaap.CellID);

                character.NClient.Send("WV");

                character.SendChararacterStats();
            }
            else
                character.NClient.Send("BN");
        }

        private int CalcPrice(Map startMap, Map nextMap)
        {
            return (int)(10 * (Math.Abs(nextMap.Model.PosX - startMap.Model.PosX) + Math.Abs(nextMap.Model.PosY - startMap.Model.PosY)));
        }

        #endregion

        #region Zaapis

        public void SendZaapis(DB_Character character)
        {
            if (Entities.Requests.ZaapisRequests.ZaapisList.Any(x => x.MapID == character.MapID))
            {
                var zaapis = Entities.Requests.ZaapisRequests.ZaapisList.First(x => x.MapID == character.MapID);
                var packet = string.Format("Wc{0}|", character.MapID);

                if ((zaapis.Faction == 1 && character.Faction.ID == 2) || (zaapis.Faction == 2 && character.Faction.ID == 1))
                {
                    character.NClient.Send("Im1196");
                    return;
                }

                Entities.Requests.ZaapisRequests.ZaapisList.Where(x => x.Faction == zaapis.Faction).ToList().
                    ForEach(x => packet = string.Concat(packet, x.MapID, (character.Faction.ID == x.Faction ? ";10|" : ";20|")));

                character.NClient.Send(packet);
            }
            else
                character.NClient.Send("BN");
        }

        public void OnMoveZaapis(DB_Character character, int nextZaapis)
        {
            if (Entities.Requests.ZaapisRequests.ZaapisList.Any(x => x.MapID == nextZaapis))
            {
                var zaapis = Entities.Requests.ZaapisRequests.ZaapisList.First(x => x.MapID == nextZaapis);

                if ((zaapis.Faction == 1 && character.Faction.ID == 2) || (zaapis.Faction == 2 && character.Faction.ID == 1))
                {
                    character.NClient.Send("Im1196");
                    return;
                }

                var price = (character.Faction.ID == zaapis.Faction ? 10 : 20);

                character.Kamas -= price;
                character.NClient.Send(string.Concat("Im046;", price));
                character.TeleportNewMap(zaapis.MapID, zaapis.CellID);

                character.NClient.Send("Wv");

                character.SendChararacterStats();
            }
            else
                character.NClient.Send("BN");
        }

        #endregion
    }
}
