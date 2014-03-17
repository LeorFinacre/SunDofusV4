using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunDofus.Game.Maps;
using SunDofus.Utilities;
using TinyCore;

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

        [OProperty("emotes", OProperty.TinyPropertyType.INT)]
        protected int p_Emotes { get; set; }

        public DB_CharacterStats Stats { get; set; }
        public DB_CharacterFaction Faction { get; set; }
        public List<DB_CharacterItem> Items { get; set; }
        public List<DB_CharacterSpell> Spells { get; set; }

        public DB_Character()
        {
            p_Emotes = 1;

            Followers = new List<DB_Character>();
        }

        #region Pattern

        public string PatternOnList()
        {
            StringBuilder builder = new StringBuilder();
            {
                builder.Append(ID).Append(";");
                builder.Append(Name).Append(";");
                builder.Append(Level).Append(";");
                builder.Append(Skin).Append(";");
                builder.Append(Basic.DeciToHex(Color)).Append(";");
                builder.Append(Basic.DeciToHex(Color2)).Append(";");
                builder.Append(Basic.DeciToHex(Color3)).Append(";");

                //builder.Append(GetItemsPos()).Append(";");
                builder.Append(",,,,"); //Items

                builder.Append("0;").Append(Servers.GAMESERVER_ID).Append(";;;");
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
                //builder.Append(GetItems()).Append("|");         Items ToString separate with ;
                builder.Append("|");
            }

            return builder.ToString();
        }

        #endregion

        #region Stats



        #endregion

        #region Faction



        #endregion

        #region Items

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
        
        public List<Channel> Channels { get; set; }
        public Character Client { get; set; }

        public CharacterChannels(Character client)
        {
            Client = client;
            Channels = new List<Channel>();

            AddChannel('*', true);
            AddChannel('#', true);
            AddChannel('$', true);
            AddChannel('p', true);
            AddChannel('%', true);
            AddChannel('i', true);
            AddChannel(':', true);
            AddChannel('?', true);
            AddChannel('!', true);
            AddChannel('^', true);
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
    }
}
