using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunDofus.Game.Maps;
using SunDofus.Utilities;
using TinyCore;
using SunDofus.Game.Characters.Stats;
using SunDofus.Network.Clients;
using SunDofus.Game.Characters;

namespace SunDofus.DataRecords
{
    [OTable("characters")]
    class DB_Character : ORecord
    {
        [OProperty("ID", OProperty.TinyPropertyType.INT, true)]
        public int ID { get; set; }
        [OProperty("name", OProperty.TinyPropertyType.STRING)]
        public string Name { get; set; }
        [OProperty("accountID", OProperty.TinyPropertyType.INT)]
        public int AccountID { get; set; }

        [OProperty("color", OProperty.TinyPropertyType.INT)]
        public int Color { get; set; }
        [OProperty("color2", OProperty.TinyPropertyType.INT)]
        public int Color2 { get; set; }
        [OProperty("color3", OProperty.TinyPropertyType.INT)]
        public int Color3 { get; set; }

        [OProperty("class", OProperty.TinyPropertyType.INT)]
        public int Class { get; set; }
        [OProperty("sex", OProperty.TinyPropertyType.INT)]
        public int Sex { get; set; }
        [OProperty("skin", OProperty.TinyPropertyType.INT)]
        public int Skin { get; set; }
        [OProperty("size", OProperty.TinyPropertyType.INT)]
        public int Size { get; set; }
        [OProperty("lvl", OProperty.TinyPropertyType.INT)]
        public int Level { get; set; }

        [OProperty("mapid", OProperty.TinyPropertyType.INT)]
        public int MapID { get; set; }
        [OProperty("mapcell", OProperty.TinyPropertyType.INT)]
        public int MapCell { get; set; }
        [OProperty("mapdir", OProperty.TinyPropertyType.INT)]
        public int MapDir { get; set; }

        [OProperty("statsPoint", OProperty.TinyPropertyType.INT)]
        public int CharactPoint { get; set; }
        [OProperty("spellPoint", OProperty.TinyPropertyType.INT)]
        public int SpellPoint { get; set; }

        [OProperty("savemap", OProperty.TinyPropertyType.INT)]
        public int SaveMap { get; set; }
        [OProperty("savecell", OProperty.TinyPropertyType.INT)]
        public int SaveCell { get; set; }

        [OProperty("exp", OProperty.TinyPropertyType.LONG)]
        public long Exp { get; set; }
        [OProperty("kamas", OProperty.TinyPropertyType.LONG)]
        public long Kamas { get; set; }

        public int Energy { get; set; }

        private long m_QuotaTrade;
        private long m_QuotaRecruitment;

        [OProperty("emotes", OProperty.TinyPropertyType.INT)]
        protected int p_Emotes { get; set; }
        
        private GameClient m_Client;

        public GenericStats GenericStats { get; set; }
        public DB_Map Map { get; set; }
        public DB_Guild Guild { get; set; }
        public CharacterParty Party { get; set; }
        public DB_CharacterStats Stats { get; set; }
        public DB_CharacterFaction Faction { get; set; }
        public List<DB_CharacterItem> Items { get; set; }
        public List<DB_CharacterSpell> Spells { get; set; }
        public List<DB_CharacterChannel> Channels { get; set; }
        public List<DB_Zaap> Zaaps { get; set; }

        public DB_Character()
        {
            p_Emotes = 1;
            Energy = 10000;
            m_QuotaRecruitment = 0;
            m_QuotaTrade = 0;

            IsConnected = false;            

            Followers = new List<DB_Character>();            
            GenericStats = new GenericStats(this);
            Stats = new DB_CharacterStats();
            Map = null;
            Faction = new DB_CharacterFaction();
            Items = new List<DB_CharacterItem>();
            Spells = new List<DB_CharacterSpell>();
            Channels = new List<DB_CharacterChannel>();
            Zaaps = new List<DB_Zaap>();
        }

        public void SetClient(GameClient client)
        {
            m_Client = client;
        }

        public void Send(string message)
        {
            if(m_Client != null)
                m_Client.Send(message);
        }

        public void Send(string format, params object[] args)
        {
            Send(string.Format(format, args));
        }

        #region Exp

        public void AddExp(long exp)
        {
            Exp += exp;
            LevelUp();
        }

        private void LevelUp()
        {
            if (this.Level == Entities.Requests.LevelsRequests.MaxLevel())
                return;

            if (Exp >= Entities.Requests.LevelsRequests.ReturnLevel(Level + 1).Character)
            {
                while (Exp >= Entities.Requests.LevelsRequests.ReturnLevel(Level + 1).Character)
                {
                    if (this.Level == Entities.Requests.LevelsRequests.MaxLevel())
                        break;

                    Level++;
                    SpellPoint++;
                    CharactPoint += 5;
                }

                if(IsConnected)
                    NClient.Send(string.Concat("AN", Level));

                SpellsInventary.LearnSpells();
                SendChararacterStats();
            }
        }

        #endregion

        #region Pattern

        public string PatternOnList()
        {
            StringBuilder builder = new StringBuilder();
            {
                builder.Append(ID).Append(";");
                builder.Append(Name).Append(";");
                builder.Append(Level).Append(";");
                builder.Append(Skin).Append(";");
                builder.Append(Utilities.Basic.DeciToHex(Color)).Append(";");
                builder.Append(Utilities.Basic.DeciToHex(Color2)).Append(";");
                builder.Append(Utilities.Basic.DeciToHex(Color3)).Append(";");
                builder.Append(GetItemsPos()).Append(";");
                builder.Append("0;").Append(Utilities.Config.GetInt32("SERVERID")).Append(";;;");
            }

            return builder.ToString();
        }

        public string PatternOnParty()
        {
            StringBuilder builder = new StringBuilder();
            {
                builder.Append(ID).Append(";");
                builder.Append(Name).Append(";");
                builder.Append(Skin).Append(";");
                builder.Append(Color).Append(";");
                builder.Append(Color2).Append(";");
                builder.Append(Color3).Append(";");
                builder.Append(GetItemsPos()).Append(";");
                builder.Append(Life).Append(",").Append(Stats.GetStat(StatEnum.MaxLife).Total).Append(";");
                builder.Append(Level).Append(";");
                builder.Append(Stats.GetStat(StatEnum.Initiative).Total).Append(";");
                builder.Append(Stats.GetStat(StatEnum.Prospection).Total).Append(";0");
            }

            return builder.ToString();
        }

        public string PatternOnSelect()
        {
            StringBuilder builder = new StringBuilder();
            {
                builder.Append("|").Append(ID).Append("|");
                builder.Append(Name).Append("|");
                builder.Append(Level).Append("|");
                builder.Append(Class).Append("|");
                builder.Append(Skin).Append("|");
                builder.Append(Utilities.Basic.DeciToHex(Color)).Append("|");
                builder.Append(Utilities.Basic.DeciToHex(Color2)).Append("|");
                builder.Append(Utilities.Basic.DeciToHex(Color3)).Append("||");
                builder.Append(GetItems()).Append("|");
            }

            return builder.ToString();
        }

        public string PatternGuild()
        {
            var member = Guild.Members.First(x => x.Character == this);

            StringBuilder builder = new StringBuilder();
            {
                builder.Append(ID).Append(";");
                builder.Append(Name).Append(";");
                builder.Append(Level).Append(";");
                builder.Append(Skin).Append(";");
                builder.Append(member.Rank).Append(";");
                builder.Append(member.ExpGaved).Append(";");
                builder.Append(member.ExpGived).Append(";");
                builder.Append(member.Rights).Append(";");
                builder.Append((IsConnected ? "1" : "0")).Append(";");
                builder.Append(Faction.ID).Append(";0");
            }

            return builder.ToString();
        }

        public string PatternDisplayChar()
        {
            StringBuilder builder = new StringBuilder();
            {
                builder.Append(MapCell).Append(";");
                builder.Append(Dir).Append(";0;");
                builder.Append(ID).Append(";");
                builder.Append(Name).Append(";");
                builder.Append(Class).Append(";");
                builder.Append(Skin).Append("^").Append(Size).Append(";");
                builder.Append(Sex).Append(";").Append(Faction.AlignementInfos).Append(";");
                builder.Append(Utilities.Basic.DeciToHex(Color)).Append(";");
                builder.Append(Utilities.Basic.DeciToHex(Color2)).Append(";");
                builder.Append(Utilities.Basic.DeciToHex(Color3)).Append(";");
                builder.Append(GetItemsPos()).Append(";"); // Items
                builder.Append("0;"); //Aura
                builder.Append(";;");

                if (Guild != null && Guild.Members.Count >= Utilities.Config.GetInt32("MINMEMBERS_TOWATCHOUT_GUILDS"))
                    builder.Append(Guild.Name).Append(";").Append(Guild.Emblem);
                else
                    builder.Append(";");

                builder.Append(";0;");
                builder.Append(";"); // Mount
            }

            return builder.ToString();
        }

        public string PatternFightDisplayChar()
        {
            StringBuilder builder = new StringBuilder();
            {
                builder.Append(Fighter.Cell).Append(";");
                builder.Append("1").Append(";0;");
                builder.Append(ID).Append(";");
                builder.Append(Name).Append(";");
                builder.Append(Class).Append(";");
                builder.Append(Skin).Append("^").Append(Size).Append(";");
                builder.Append(Sex).Append(";");
                builder.Append(Level).Append(";");
                builder.Append(Faction.AlignementInfos).Append(";");
                builder.Append(Utilities.Basic.DeciToHex(Color)).Append(";");
                builder.Append(Utilities.Basic.DeciToHex(Color2)).Append(";");
                builder.Append(Utilities.Basic.DeciToHex(Color3)).Append(";");
                builder.Append(GetItemsPos()).Append(";");
                builder.Append(Life).Append(";");
                builder.Append(6).Append(";").Append(3).Append(";");
                builder.Append("0;0;0;0;0;");
                builder.Append("0;0;");
                builder.Append(Fighter.Team.ID).Append(";;");
            }

            return builder.ToString();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("As");
            {
                builder.Append(Exp).Append(",");
                builder.Append(Entities.Requests.LevelsRequests.ReturnLevel(Level).Character).Append(",");
                builder.Append(Entities.Requests.LevelsRequests.ReturnLevel(Level + 1).Character).Append("|");
                builder.Append(Kamas).Append("|");
                builder.Append(CharactPoint).Append("|");
                builder.Append(SpellPoint).Append("|");
                builder.Append(Faction.ToString()).Append("|");
                builder.Append(Life).Append(",");
                builder.Append(Stats.GetStat(StatEnum.MaxLife).Total).Append("|");
                builder.Append(Energy).Append(",10000|");
                builder.Append(Stats.GetStat(StatEnum.Initiative).Total).Append("|");
                builder.Append(Stats.GetStat(StatEnum.Prospection).Total).Append("|");

                builder.Append(Stats.GetStat(StatEnum.MaxAP).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.MaxMP).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.Force).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.Vitalite).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.Sagesse).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.Chance).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.Agilite).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.Intelligence).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.RP).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.InvocationMax).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.Damage).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.DamagePhysic).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.DamageMagic).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.DamagePercent).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.Heal).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.DamagePiege).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.DamagePiegePercent).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.ReflectDamage).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.DamageCritic).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.EchecCritic).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.EsquivePA).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.EsquivePM).ToString()).Append("|");

                builder.Append(Stats.GetStat(StatEnum.ReduceDamageNeutre).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.ReduceDamagePercentNeutre).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.ReduceDamagePvPNeutre).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.ReduceDamagePercentPvPNeutre).ToString()).Append("|");

                builder.Append(Stats.GetStat(StatEnum.ReduceDamageTerre).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.ReduceDamagePercentTerre).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.ReduceDamagePvPTerre).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.ReduceDamagePercentPvPTerre).ToString()).Append("|");

                builder.Append(Stats.GetStat(StatEnum.ReduceDamageEau).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.ReduceDamagePercentEau).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.ReduceDamagePvPEau).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.ReduceDamagePercentPvPEau).ToString()).Append("|");

                builder.Append(Stats.GetStat(StatEnum.ReduceDamageAir).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.ReduceDamagePercentAir).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.ReduceDamagePvPAir).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.ReduceDamagePercentPvPAir).ToString()).Append("|");

                builder.Append(Stats.GetStat(StatEnum.ReduceDamageFeu).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.ReduceDamagePercentFeu).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.ReduceDamagePvPFeu).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.ReduceDamagePercentPvPFeu).ToString()).Append("|");

                builder.Append("1");
            }

            return builder.ToString();
        }

        #endregion

        #region Stats

        public void SendPods()
        {
            NClient.Send(string.Format("Ow{0}|{1}", Pods, Stats.GetStat(StatEnum.MaxPods).Total));
        }

        public void AddLife(int life)
        {
            if (Life == Stats.GetStat(StatEnum.MaxLife).Total)
                NClient.SendMessage("Im119");

            else if ((Life + life) > Stats.GetStat(StatEnum.MaxLife).Total)
            {
                NClient.SendMessage(string.Concat("Im01;", (Stats.GetStat(StatEnum.MaxLife).Total - Life)));
                Life = Stats.GetStat(StatEnum.MaxLife).Total;
            }
            else
            {
                NClient.SendMessage(string.Concat("Im01;", life));
                Life += life;
            }
        }

        public void ResetVita(string datas)
        {
            if (datas == "full")
            {
                Life = Stats.GetStat(StatEnum.MaxLife).Total;
                SendChararacterStats();
            }
            else
            {
                Life = (Stats.GetStat(StatEnum.MaxLife).Total / (int.Parse(datas) / 100));
                SendChararacterStats();
            }
        }

        #endregion

        #region Faction



        #endregion

        #region Items

        public string GetItemsPos()
        {
            var packet = "";

            if (ItemsInventary.ItemsList.Any(x => x.Position == 1))
                packet += Utilities.Basic.DeciToHex(ItemsInventary.ItemsList.First(x => x.Position == 1).Model.ID);

            packet += ",";

            if (ItemsInventary.ItemsList.Any(x => x.Position == 6))
                packet += Utilities.Basic.DeciToHex(ItemsInventary.ItemsList.First(x => x.Position == 6).Model.ID);

            packet += ",";

            if (ItemsInventary.ItemsList.Any(x => x.Position == 7))
                packet += Utilities.Basic.DeciToHex(ItemsInventary.ItemsList.First(x => x.Position == 7).Model.ID);

            packet += ",";

            if (ItemsInventary.ItemsList.Any(x => x.Position == 8))
                packet += Utilities.Basic.DeciToHex(ItemsInventary.ItemsList.First(x => x.Position == 8).Model.ID);

            packet += ",";

            if (ItemsInventary.ItemsList.Any(x => x.Position == 15))
                packet += Utilities.Basic.DeciToHex(ItemsInventary.ItemsList.First(x => x.Position == 15).Model.ID);

            return packet;
        }

        public string GetItems()
        {
            return string.Join(";", ItemsInventary.ItemsList);
        }

        public string GetItemsToSave()
        {
            return (string.Join(";", from x in ItemsInventary.ItemsList select x.SaveString()));
        }

        public Character Client { get; set; }
        public List<CharacterItem> ItemsList { get; set; }
        public Dictionary<int, CharacterSet> SetsList { get; set; }

        public InventaryItems(Character character)
        {
            Client = character;

            ItemsList = new List<CharacterItem>();
            SetsList = new Dictionary<int,CharacterSet>();
        }

        public void AddItem(int id, bool offline, int jet = 4)
        {
            if (offline == true)
            {
                if (!Entities.Requests.ItemsRequests.ItemsList.Any(x => x.ID == id))
                    return;

                var baseItem = Entities.Requests.ItemsRequests.ItemsList.First(x => x.ID == id);
                var item = new CharacterItem(baseItem);

                item.GeneratItem(jet);

                lock (ItemsList)
                {
                    if (ItemsList.Any(x => x.EffectsInfos() == item.EffectsInfos() && x.Model.ID == item.Model.ID && x.Position == item.Position))
                    {
                        var item2 = ItemsList.First(x => x.EffectsInfos() == item.EffectsInfos() && x.Model.ID == item.Model.ID && x.Position == item.Position);

                        item2.Quantity += item.Quantity;
                        Client.Pods += (item.Model.Pods * item.Quantity);

                        return;
                    }

                    item.ID = ItemsHandler.GetNewID();

                    ItemsList.Add(item);

                    Client.Pods += (item.Model.Pods * item.Quantity);
                }
            }
            else if (offline == false)
            {
                if (!Entities.Requests.ItemsRequests.ItemsList.Any(x => x.ID == id))
                    return;

                var baseItem = Entities.Requests.ItemsRequests.ItemsList.First(x => x.ID == id);
                var item = new CharacterItem(baseItem);

                item.GeneratItem(jet);

                lock (ItemsList)
                {
                    if (ItemsList.Any(x => x.EffectsInfos() == item.EffectsInfos() && x.Model.ID == item.Model.ID && x.Position == item.Position))
                    {
                        var item2 = ItemsList.First(x => x.EffectsInfos() == item.EffectsInfos() && x.Model.ID == item.Model.ID && x.Position == item.Position);

                        item2.Quantity += item.Quantity;
                        Client.Pods += (item.Model.Pods * item.Quantity);

                        RefreshBonus();
                        Client.NClient.Send(string.Format("OQ{0}|{1}", item2.ID, item2.Quantity));

                        return;
                    }

                    item.ID = ItemsHandler.GetNewID();

                    ItemsList.Add(item);
                }

                Client.Pods += (item.Model.Pods * item.Quantity);
                RefreshBonus();

                Client.NClient.Send(string.Concat("OAKO", item.ToString()));
            }
        }

        public void AddItem(CharacterItem item, bool offline)
        {
            lock (ItemsList)
            {
                if (offline == true)
                {
                    if (ItemsList.Any(x => x.EffectsInfos() == item.EffectsInfos() && x.Model.ID == item.Model.ID && x.Position == item.Position))
                    {
                        var item2 = ItemsList.First(x => x.EffectsInfos() == item.EffectsInfos() && x.Model.ID == item.Model.ID && x.Position == item.Position);

                        item2.Quantity += item.Quantity;
                        Client.Pods += (item.Model.Pods * item.Quantity);

                        return;
                    }

                    item.ID = ItemsHandler.GetNewID();

                    ItemsList.Add(item);
                    Client.Pods += (item.Model.Pods * item.Quantity);
                }
                else if (offline == false)
                {
                    if (ItemsList.Any(x => x.EffectsInfos() == item.EffectsInfos() && x.Model.ID == item.Model.ID && x.Position == item.Position))
                    {
                        var item2 = ItemsList.First(x => x.EffectsInfos() == item.EffectsInfos() && x.Model.ID == item.Model.ID && x.Position == item.Position);

                        item2.Quantity += item.Quantity;
                        Client.Pods += (item.Model.Pods * item.Quantity);

                        RefreshBonus();
                        Client.NClient.Send(string.Format("OQ{0}|{1}", item2.ID, item2.Quantity));

                        return;
                    }

                    item.ID = ItemsHandler.GetNewID();
                    ItemsList.Add(item);

                    Client.Pods += (item.Model.Pods * item.Quantity);
                    RefreshBonus();

                    Client.NClient.Send(string.Concat("OAKO", item.ToString()));
                }
            }
        }

        public void DeleteItem(int id, int quantity)
        {
            lock (ItemsList)
            {
                if (ItemsList.Any(x => x.ID == id))
                {
                    var item = ItemsList.First(x => x.ID == id);

                    if (item.Quantity <= quantity)
                    {
                        Client.Pods -= (item.Quantity * item.Model.Pods);

                        ItemsList.Remove(item);
                        Client.NClient.Send(string.Concat("OR", item.ID));

                        RefreshBonus();
                    }
                    else
                    {
                        Client.Pods -= (quantity * item.Model.Pods);

                        item.Quantity -= quantity;
                        Client.NClient.Send(string.Format("OQ{0}|{1}", item.ID, item.Quantity));

                        RefreshBonus();
                    }
                }
                else
                    Client.NClient.Send("BN");
            }
        }

        public void MoveItem(int id, int pos, int quantity)
        {
            if (!ItemsList.Any(x => x.ID == id))
                return;

            var item = ItemsList.First(x => x.ID == id);

            if (ItemsHandler.PositionAvaliable(item.Model.Type, item.Model.isUsable, pos) == false
                || pos == 1 && item.Model.isTwoHands == true && isOccuptedPos(15) || pos == 15 && isOccuptedPos(1))
            {
                Client.NClient.Send("BN");
                return;
            }

            if (!ItemsHandler.ConditionsAvaliable(item.Model, Client))
            {
                Client.NClient.Send("Im119|44");
                return;
            }

            if (IsEquippablePos(pos) & HasTemplateEquipped(item.Model.ID))
            {
                Client.NClient.Send("OAEA");
                return;
            }

            if (item.Model.Type == 23 && pos != -1)
            {
                if (!ItemsList.Any(x => x.Model.ID == item.Model.ID && x.Position != -1 && x.Model.Type == 23))
                {
                    Client.NClient.Send("OAEA");
                    return;
                }
            }

            if (item.Model.Level > Client.Level)
            {
                Client.NClient.Send("OAEL");
                return;
            }

            var lastpos = item.Position;
            item.Position = pos;

            if (item.Position == -1)
            {
                if (ItemsList.Any(x => x.EffectsInfos() == item.EffectsInfos() && x.Model.ID == item.Model.ID && x.Position == item.Position &&
                    x.ID != item.ID))
                {
                    var item2 = ItemsList.First(x => x.EffectsInfos() == item.EffectsInfos() && x.Model.ID == item.Model.ID && x.Position == item.Position &&
                    x.ID != item.ID);

                    item2.Quantity += item.Quantity;
                    Client.Pods += (item.Model.Pods * item.Quantity);
                    RefreshBonus();

                    Client.NClient.Send(string.Format("OQ{0}|{1}", item2.ID, item2.Quantity));
                    DeleteItem(item.ID, item.Quantity);

                    if (Client.State.Party != null && IsEquippablePos(lastpos))
                        Client.State.Party.UpdateMembers();

                    return;
                }
            }
            else
            {
                if (item.Quantity > 1)
                {
                    if (item.Model.Type == 12 || item.Model.Type == 13 || item.Model.Type == 14 || item.Model.Type == 28 ||
                        item.Model.Type == 33 || item.Model.Type == 37 || item.Model.Type == 42 || item.Model.Type == 49 ||
                        item.Model.Type == 69 || item.Model.Type == 87)
                    {
                        if (quantity <= 0)
                            return;

                        var Copy = item.Copy();
                        Copy.Quantity -= quantity;

                        if (item.Quantity == quantity)
                            Copy.Position = pos;
                        else
                            Copy.Position = -1;

                        item.Quantity = quantity;
                        AddItem(Copy, false);
                    }
                    else
                    {
                        var Copy = item.Copy();

                        Copy.Quantity -= 1;
                        Copy.Position = -1;

                        item.Quantity = 1;
                        AddItem(Copy, false);
                    }

                    Client.NClient.Send(string.Format("OQ{0}|{1}", item.ID, item.Quantity));
                }
            }

            if (Client.State.Party != null && IsEquippablePos(pos))
                Client.State.Party.UpdateMembers();

            Client.NClient.Send(string.Format("OM{0}|{1}", item.ID, (item.Position != -1 ? item.Position.ToString() : "")));
            Client.GetMap().Send(string.Format("Oa{0}|{1}", Client.ID, Client.GetItemsPos()));

            RefreshBonus();
        }

        public bool isOccuptedPos(int pos)
        {
            return ItemsList.Any(x => x.Position == pos);
        }

        public void ParseItems(string datas)
        {
            var splited = datas.Split(';');

            foreach (var infos in splited)
            {
                var allInfos = infos.Split('~');
                var item = new CharacterItem(Entities.Requests.ItemsRequests.ItemsList.First(x => x.ID == Convert.ToInt32(allInfos[0], 16)));
                item.EffectsList.Clear();

                item.ID = ItemsHandler.GetNewID();
                item.Quantity = Convert.ToInt32(allInfos[1], 16);

                if (allInfos[2] != "")
                    item.Position = Convert.ToInt32(allInfos[2], 16);
                else
                    item.Position = -1;

                if (allInfos[3] != "")
                {
                    var effectsList = allInfos[3].Split(',');

                    foreach (var effect in effectsList)
                    {
                        var NewEffect = new Effects.EffectItem();
                        string[] EffectInfos = effect.Split('#');

                        NewEffect.ID = Convert.ToInt32(EffectInfos[0], 16);

                        if (EffectInfos[1] != "")
                            NewEffect.Value = Convert.ToInt32(EffectInfos[1], 16);

                        if (EffectInfos[2] != "")
                            NewEffect.Value2 = Convert.ToInt32(EffectInfos[2], 16);

                        if (EffectInfos[3] != "")
                            NewEffect.Value3 = Convert.ToInt32(EffectInfos[3], 16);

                        NewEffect.Effect = EffectInfos[4];

                        lock(item.EffectsList)
                            item.EffectsList.Add(NewEffect);
                    }

                }

                Client.Pods += (item.Model.Pods * item.Quantity);

                lock(ItemsList)
                    ItemsList.Add(item);
            }
        }

        public bool IsEquippablePos(int pos)
        {
            return (pos > -1 & pos < 16);
        }

        public bool HasTemplateEquipped(int id)
        {
            for (int i = 0; i < 16; i++)
            {
                var item = GetItemByPos(i);

                if (item != null && item.Model.ID == id)
                    return true;
            }

            return false;
        }

        public CharacterItem GetItemByPos(int pos)
        {
            if (!ItemsList.Any(x => x.Position == pos))
                return null;

            return ItemsList.First(x => x.Position == pos);
        }

        public void RefreshBonus()
        {
            Client.Stats.ResetStatEquipped();
            SetsList.Clear();

            foreach (var item in ItemsList)
            {
                if (item.Position != -1 && item.Position < 23)
                {
                    foreach (var effect in item.EffectsList)
                        effect.ParseEffect(Client);
                }
                if (item.Model.Set != -1 && item.Position != -1)
                {
                    if (SetsList.ContainsKey(item.Model.Set))
                    {
                        if (!SetsList[item.Model.Set].ItemsList.Contains(item.Model.ID))
                            SetsList[item.Model.Set].ItemsList.Add(item.Model.ID);
                    }
                    else
                    {
                        SetsList.Add(item.Model.Set, new CharacterSet(item.Model.Set));
                        SetsList[item.Model.Set].ItemsList.Clear();
                        SetsList[item.Model.Set].ItemsList.Add(item.Model.ID);
                    }
                }
            }

            foreach (var set in SetsList.Values)
            {
                var numberItems = set.ItemsList.Count;
                var strItems = string.Join(";", set.ItemsList);
                var strEffects = "";

                foreach (var effect in set.BonusList[numberItems])
                {
                    strEffects += string.Concat(effect.SetString(), ",");
                    effect.ParseEffect(Client);
                }

                Client.NClient.Send(string.Format("OS+{0}|{1}|{2}", set.ID, strItems,
                    (strEffects == "" ? "" : strEffects.Substring(0, strEffects.Length - 1))));
            }

            Client.SendPods();
            Client.SendChararacterStats();
        }

        public void UseItem(string datas)
        {
            if (Client.State.OnMove == true)
            {
                Client.NClient.Send("BN");
                return;
            }

            var allDatas = datas.Split('|');

            var itemID = int.Parse(allDatas[0]);
            var charID = Client.ID;
            var cellID = Client.MapCell;

            if (allDatas.Length > 2)
            {
                charID = int.Parse(allDatas[1]);
                cellID = int.Parse(allDatas[2]);
            }

            if (!ItemsList.Any(x => x.ID == itemID))
            {
                Client.NClient.Send("OUE");
                return;
            }

            var item = ItemsList.First(x => x.ID == itemID);

            if (item.Model.isUsable == false)
            {
                Client.NClient.Send("BN");
                return;
            }

            var usable = Entities.Requests.ItemsRequests.UsablesList.First(x => x.Base == item.Model.ID);

            var character = SunDofus.World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == charID);

            if (!ItemsHandler.ConditionsAvaliable(item.Model, Client))
            {
                Client.NClient.Send("Im119|44");
                return;
            }

            usable.ParseEffect(character);

            if (usable.MustDelete == true)
                DeleteItem(item.ID, 1);
        }

        #endregion

        #region Spells

        public void LearnSpells()
        {
            foreach (var spell in Entities.Requests.SpellsRequests.SpellsToLearnList.Where(x => x.Race == Client.Class && x.Level <= Client.Level))
            {
                if (Spells.Any(x => x.ID == spell.SpellID))
                    continue;

                AddSpells(spell.SpellID, 1, spell.Pos);
            }
        }

        public void AddSpells(int id, int level, int pos)
        {
            if (Spells.Any(x => x.ID == id)) 
                return;

            if (level < 1) level = 1;
            if (level > 6) level = 6;

            if (pos > 25) pos = 25;
            if (pos < 1) pos = 25;

            lock (Spells)
            {
                var newSpell = new DB_CharacterSpell();
                {
                    newSpell.ID = Servers.CharactersSpells.Count > 0 ? Servers.CharactersSpells.OrderBy(x => x.ID).ToArray()[0].ID + 1 : 1;
                    newSpell.CharacterID = ID;
                    newSpell.SpellID = id;
                    newSpell.SpellLevel = level;
                    newSpell.SpellPosition = pos;
                }

                Spells.Add(newSpell);
                newSpell.Insert();
            }
        }

        public void SendAllSpells()
        {
            var packet = "";

            foreach (var spell in Spells)
                packet += string.Format("{0}~{1}~{2};", spell.SpellID, spell.SpellLevel, Pathfinding.GetDirChar(spell.SpellPosition));

            Send(string.Concat("SL", packet));
        }

        public string SaveSpells()
        {
            return string.Join("|", Spells);
        }

        #endregion

        #region Emotes

        public void AddEmote(int emote)
        {
            p_Emotes |= emote;
        }

        public void AddEmoteNbr(int emote)
        {
            p_Emotes |= (1 << (emote - 1));
        }

        public bool HasEmote(int emote)
        {
            if (emote == 1)
                return true;

            return (p_Emotes & (emote)) > 0;
        }

        public bool HasEmoteNbr(int emote)
        {
            if (emote == 1)
                return true;

            return (p_Emotes & (1 << emote - 1)) > 0;
        }

        #endregion

        #region State

        public bool IsConnected { get; set; }
        public bool OnMove { get; set; }
        public bool OnExchange { get; set; }
        public bool OnExchangePanel { get; set; }
        public bool OnExchangeAccepted { get; set; }
        public bool OnExchangeWithBank { get; set; }

        public int MoveToCell { get; set; }
        public int ActualNPC { get; set; }
        public int CurrentPlayerTrade { get; set; }

        public long SitStartTime { get; set; }
        public bool IsSitted { get; set; }

        public bool InFight { get; set; }
        public bool IsSpectator { get; set; }

        public bool OnWaitingParty { get; set; }
        public int SenderInviteParty { get; set; }
        public int ReceiverInviteParty { get; set; }

        public bool OnWaitingGuild { get; set; }
        public int SenderInviteGuild { get; set; }
        public int ReceiverInviteGuild { get; set; }

        public bool IsFollow { get; set; }
        public bool IsFollowing { get; set; }
        public int FollowingID { get; set; }

        public bool OnDialoging { get; set; }
        public int OnDialogingWith { get; set; }

        public bool IsChallengeAsked { get; set; }
        public bool IsChallengeAsker { get; set; }
        public int ChallengeAsked { get; set; }
        public int ChallengeAsker { get; set; }

        //public CharacterParty Party { get; set; }
        public List<DB_Character> Followers { get; set; }

        public bool Busy
        {
            get
            {
                return (OnMove || OnExchange || OnWaitingParty || OnDialoging || IsChallengeAsked || IsChallengeAsker || OnExchangeWithBank || InFight || IsSpectator);
            }
        }

        #endregion

        #region Channels
        
        public long TimeTrade()
        {
            return (long)Math.Ceiling((double)((m_QuotaTrade - Environment.TickCount) / 1000));
        }

        public long TimeRecruitment()
        {
            return (long)Math.Ceiling((double)((m_QuotaRecruitment - Environment.TickCount) / 1000));
        }

        public long TimeSmiley()
        {
            return (long)Math.Ceiling((double)((quotaSmiley - Environment.TickCount) / 1000));
        }

        public bool CanSendinTrade()
        {
            return (TimeTrade() <= 0 ? true : false);
        }

        public bool CanSendinRecruitment()
        {
            return (TimeRecruitment() <= 0 ? true : false);
        }

        public bool CanSendinSmiley()
        {
            return TimeSmiley() <= 0;
        }

        public void RefreshTrade()
        {
            m_QuotaTrade = Environment.TickCount + Utilities.Config.GetInt64("ANTISPAMTRADE");
        }

        public void RefreshRecruitment()
        {
            m_QuotaRecruitment = Environment.TickCount + Utilities.Config.GetInt64("ANTISPAMRECRUITMENT");
        }

        public void RefreshSmiley()
        {
            quotaSmiley = Environment.TickCount + 1000;
        }

        public void AddChannel(char head, bool state)
        {
            lock (Channels)
                Channels.Add(new Channel(head, state));
        }

        public void SendChannels()
        {
            Client.NClient.Send(string.Concat("cC+", string.Join("", from c in Channels select c.Head.ToString())));
        }

        public void ChangeChannelState(char head, bool state)
        {
            if (Channels.Any(x => x.Head == head))
            {
                Channels.First(x => x.Head == head).On = state;
                Client.NClient.Send(string.Format("cC{0}{1}", (state ? "+" : "-"), head.ToString()));
            }
        }

        #endregion

        #region Chats
        
        public void SendGeneralMessage(string message)
        {
            if(Map == null)
                return;

            Player.Map.Send(string.Format("cMK|{0}|{1}|{2}", Player.ID, Player.Name, message));
        }

        public void SendIncarnamMessage(string message)
        {
            if (!Player.IsInIncarnam || Player.Level > 30)
            {
                Send("Im0139");
                return;
            }

            foreach (GameClient character in Servers.GameServer.Clients.Where
                (x => (x as GameClient).Authentified == true && (x as GameClient).Player.IsInIncarnam))
            {
                character.Send(string.Format("cMK^|{0}|{1}|{2}", Player.ID, Player.Name, message));
            }
        }

        public void SendPrivateMessage(string receiver, string message)
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

        public void SendTradeMessage(string message)
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

        public void SendRecruitmentMessage(string message)
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

        public void SendFactionMessage(string message)
        {
            if (client.Player.Faction.ID != 0 && client.Player.Faction.Level >= 3)
            {
                foreach (GameClient character in Servers.GameServer.Clients.Where(x => (x as GameClient).Authentified == true && (x as GameClient).Player.Faction.ID == client.Player.Faction.ID))
                    character.Send(string.Format("cMK!|{0}|{1}|{2}", client.Player.ID, client.Player.Name, message));
            }
            else
                client.Send("BN");
        }

        public void SendPartyMessage(string message)
        {
            if (client.Player.State.Party != null)
            {
                foreach (var character in client.Player.State.Party.Members.Keys)
                    character.NClient.Send(string.Format("cMK$|{0}|{1}|{2}", client.Player.ID, client.Player.Name, message));
            }
            else
                client.Send("BN");
        }

        public void SendGuildMessage(string message)
        {
            if (client.Player.Guild != null)
            {
                foreach (var character in client.Player.Guild.Members.Where(x => x.Character.IsConnected))
                    character.Character.NClient.Send(string.Format("cMK%|{0}|{1}|{2}", client.Player.ID, client.Player.Name, message));
            }
            else
                client.Send("BN");
        }

        public void SendAdminMessage(string message)
        {
            if (client.Infos.GMLevel > 0)
            {
                foreach (GameClient character in Servers.GameServer.Clients.Where(x => (x as GameClient).Authentified == true && (x as GameClient).Infos.GMLevel > 0))
                    character.Send(string.Format("cMK@|{0}|{1}|{2}", client.Player.ID, client.Player.Name, message));
            }
            else
                client.Send("BN");
        }

        #endregion

        #region Jobs

        public List<Jobs.Job> Jobs { get; set; }
        public JobOptions Options { get; set; }

        private Character Client;

        public CharacterJobs(Character client)
        {
            Client = client;
            Options = new JobOptions(0, 0, 0);
            Jobs = new List<Jobs.Job>();
        }

        public void Parse(string data)
        {
            foreach (var j in data.Split('|'))
            {
                var info = data.Split(';');
                var job = new Jobs.Job(int.Parse(info[0]), int.Parse(info[1]), int.Parse(info[2]));

                job.ParseSkills(info[3]);
                Jobs.Add(job);
            }
        }

        public string Save()
        {
            var data = string.Empty;

            foreach (var job in Jobs)
                data += string.Format("{0}|{1};{2};{3};{4}", data, job.ID, job.Level, job.Experience, job.SaveSkills());

            return data.Length > 0 ? data.Substring(1) : "";
        }

        public void AddNewJob(int id)
        {
            if (Jobs.Any(x => x.ID == id))
                return;

            Jobs.Add(new Jobs.Job(id, 1, 0));

            SendJobs();
            SendJobsXP();
            SendJobOptions();
        }

        public void SendJobOptions()
        {
            Client.NClient.Send(string.Concat("JO", Options.ToString()));
        }

        public void SendJobsXP()
        {
            var data = string.Empty;

            foreach (var job in Jobs)
                data = string.Format("{0}|{1};{2};{3};{4};{5}",data, job.ID, job.Level, job.GetMinExperience(), job.Experience, job.GetMaxExperience());
            
            Client.NClient.Send(string.Concat("JX", data));
        }

        public void SendJobs()
        {
            var data = string.Empty;

            foreach (var job in Jobs)
                data = string.Format("{0}|{1};{2}",data, job.ID, job.GetSkills());

            Client.NClient.Send(string.Concat("JS", data));
        }

        public bool HasJob(int id)
        {
            return Jobs.Any(x => x.ID == id);
        }

        #endregion

        #region Map

        public void LoadMap()
        {
            if (Entities.Requests.MapsRequests.MapsList.Any(x => x.Model.ID == this.MapID))
            {
                var map = Entities.Requests.MapsRequests.MapsList.First(x => x.Model.ID == this.MapID);

                if (Utilities.Config.GetBool("DEBUG") & map.Triggers.Count == 0)
                    SunDofus.World.Entities.Requests.TriggersRequests.LoadTriggers(MapID);

                NClient.Send(string.Format("GDM|{0}|{1}|{2}", map.Model.ID, map.Model.Date, map.Model.Key));

                if (this.State.IsFollow)
                {
                    foreach (var character in this.State.Followers)
                        character.NClient.Send(string.Format("IC{0}|{1}", GetMap().Model.PosX, GetMap().Model.PosY));
                }
            }
        }

        public bool IsInIncarnam
        {
            get
            {
                var map = GetMap();

                return map.Model.SubArea == 440 || map.Model.SubArea == 442 || map.Model.SubArea == 443 ||
                    map.Model.SubArea == 444 || map.Model.SubArea == 445 || map.Model.SubArea == 446 ||
                    map.Model.SubArea == 449 || map.Model.SubArea == 450;
            }
        }

        public void TeleportNewMap(int _mapID, int _cell)
        {
            NClient.Send(string.Format("GA;2;{0};", ID));

            GetMap().DelPlayer(this);
            var map = Entities.Requests.MapsRequests.MapsList.First(x => x.Model.ID == _mapID);

            MapID = map.Model.ID;
            MapCell = _cell;

            LoadMap();
        }

        public void Sit()
        {
            State.IsSitted = !State.IsSitted;

            if (State.IsSitted)
            {
                State.SitStartTime = Environment.TickCount;

                NClient.Send("ILS1000");
            }
            else
            {
                int regenerated = (int)(Environment.TickCount - State.SitStartTime) / 1000;
                int missing = Stats.GetStat(StatEnum.MaxLife).Total - Life;

                if (regenerated > missing)
                    regenerated = missing;

                NClient.Player.Life += regenerated;

                NClient.Send("ILF" + regenerated);
                NClient.Send("ILS360000");
            }
        }

        public Maps.Map GetMap()
        {
            return Entities.Requests.MapsRequests.MapsList.First(x => x.Model.ID == this.MapID);
        }

        #endregion

        #region Params

        public string GetParam(string paramName)
        {
            switch (paramName)
            {
                case "kamas":
                    return Kamas.ToString();

                default:
                    return "";
            }
        }

        #endregion
    }
}
