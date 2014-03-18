using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunDofus.DataRecords;
using SunDofus.Network.Clients;

namespace SunDofus.Network.Parsers
{
    class GameParser
    {
        private GameClient m_Client { get; set; }

        private delegate void Packets(string datas);
        private Dictionary<string, Packets> m_RegisteredPackets;

        public GameParser(GameClient client)
        {
            m_Client = client;

            m_RegisteredPackets = new Dictionary<string, Packets>();
            RegisterPackets();
        }

        #region Packets

        private void RegisterPackets()
        {
            m_RegisteredPackets["AA"] = CreateCharacter;
            m_RegisteredPackets["AB"] = StatsBoosts;
            m_RegisteredPackets["AD"] = DeleteCharacter;
            m_RegisteredPackets["Ag"] = SendGifts;
            m_RegisteredPackets["AG"] = AcceptGift;
            m_RegisteredPackets["AL"] = SendCharacterList;
            m_RegisteredPackets["AP"] = SendRandomName;
            m_RegisteredPackets["AS"] = SelectCharacter;
            m_RegisteredPackets["AT"] = ParseTicket;
            m_RegisteredPackets["AV"] = SendCommunauty;
            m_RegisteredPackets["BA"] = ParseConsoleMessage;
            m_RegisteredPackets["Ba"] = TeleportByPos;
            m_RegisteredPackets["BD"] = SendDate;
            m_RegisteredPackets["BM"] = ParseChatMessage;
            m_RegisteredPackets["BS"] = UseSmiley;
            m_RegisteredPackets["cC"] = ChangeChannel;
            m_RegisteredPackets["DC"] = DialogCreate;
            m_RegisteredPackets["DR"] = DialogReply;
            m_RegisteredPackets["DV"] = DialogExit;
            m_RegisteredPackets["eD"] = ChangeDirection;
            m_RegisteredPackets["eU"] = UseEmote;
            m_RegisteredPackets["EA"] = ExchangeAccept;
            m_RegisteredPackets["EB"] = ExchangeBuy;
            m_RegisteredPackets["EK"] = ExchangeValidate;
            m_RegisteredPackets["EM"] = ExchangeMove;
            m_RegisteredPackets["ER"] = ExchangeRequest;
            m_RegisteredPackets["ES"] = ExchangeSell;
            m_RegisteredPackets["EV"] = CancelExchange;
            m_RegisteredPackets["FA"] = FriendAdd;
            m_RegisteredPackets["FD"] = FriendDelete;
            m_RegisteredPackets["FL"] = FriendsList;
            m_RegisteredPackets["FO"] = FriendsFollow;
            m_RegisteredPackets["fD"] = FightDetails;
            m_RegisteredPackets["fL"] = FightList;
            m_RegisteredPackets["fN"] = ToggleFightLock;
            m_RegisteredPackets["fH"] = ToggleFightHelp;
            m_RegisteredPackets["fP"] = ToggleFightParty;
            m_RegisteredPackets["fS"] = ToggleFightSpectator;
            m_RegisteredPackets["GA"] = GameAction;
            m_RegisteredPackets["GC"] = CreateGame;
            m_RegisteredPackets["GI"] = GameInformations;
            m_RegisteredPackets["GK"] = EndAction;
            m_RegisteredPackets["GP"] = ChangeAlignmentEnable;
            m_RegisteredPackets["GR"] = FightReady;
            m_RegisteredPackets["GT"] = FightTurnReady;
            m_RegisteredPackets["Gt"] = FightTurnPass;
            m_RegisteredPackets["GQ"] = FightLeave;
            m_RegisteredPackets["Gp"] = FightPlacement;
            m_RegisteredPackets["gB"] = UpgradeStatsGuild;
            m_RegisteredPackets["gb"] = UpgradeSpellsGuild;
            m_RegisteredPackets["gC"] = CreateGuild;
            m_RegisteredPackets["gH"] = LetCollectorGuild;
            m_RegisteredPackets["gI"] = GetGuildInfos;
            m_RegisteredPackets["gJ"] = GetGuildJoinRequest;
            m_RegisteredPackets["gK"] = ExitGuild;
            m_RegisteredPackets["gP"] = ModifyRightGuild;
            m_RegisteredPackets["gV"] = CloseGuildPanel;
            m_RegisteredPackets["iA"] = EnemyAdd;
            m_RegisteredPackets["iD"] = EnemyDelete;
            m_RegisteredPackets["iL"] = EnemiesList;
            m_RegisteredPackets["Od"] = DeleteItem;
            m_RegisteredPackets["OM"] = MoveItem;
            m_RegisteredPackets["OU"] = UseItem;
            m_RegisteredPackets["PA"] = PartyAccept;
            m_RegisteredPackets["PG"] = PartyGroupFollow;
            m_RegisteredPackets["PF"] = PartyFollow;
            m_RegisteredPackets["PI"] = PartyInvite;
            m_RegisteredPackets["PR"] = PartyRefuse;
            m_RegisteredPackets["PV"] = PartyLeave;
            m_RegisteredPackets["SB"] = SpellBoost;
            m_RegisteredPackets["SM"] = SpellMove;
            m_RegisteredPackets["WU"] = UseZaaps;
            m_RegisteredPackets["Wu"] = UseZaapis;
            m_RegisteredPackets["WV"] = ExitZaap;
            m_RegisteredPackets["Wv"] = ExitZaapis;
        }

        public void Parse(string datas)
        {
            if (datas == "ping")
                m_Client.Send("pong");
            else if (datas == "qping")
                m_Client.Send("qpong");

            if (datas.Length < 2) 
                return;

            string header = datas.Substring(0, 2);

            if (!m_RegisteredPackets.ContainsKey(header))
            {
                m_Client.Send("BN");
                return;
            }

            m_RegisteredPackets[header](datas.Substring(2));
        }

        #endregion

        #region Ticket

        private void ParseTicket(string datas)
        {
            if (Servers.GameKeys.Any(x => x.Key == datas))
            {
                var key = Servers.GameKeys.First(x => x.Key == datas);

                if (Servers.GameServer.Clients.Any(x => (x as GameClient).Authentified == true && (x as GameClient).Account.Pseudo == key.Account.Pseudo))
                    Servers.GameServer.Clients.First(x => (x as GameClient).Authentified == true && (x as GameClient).Account.Pseudo == key.Account.Pseudo).Disconnect();

                m_Client.Account = key.Account;
                m_Client.Account.ParseCharacters();

                m_Client.Authentified = true;

                m_Client.Account.IsConnected = 1;
                m_Client.Account.Update();

                lock (Servers.GameKeys)
                    Servers.GameKeys.Remove(key);

                m_Client.Send("ATK0");
            }
            else
                m_Client.Send("ATE");
        }

        #endregion
        
        #region Character

        private void SendRandomName(string datas)
        {
            m_Client.Send(string.Concat("APK", Utilities.Basic.RandomName()));
        }

        private void SendCommunauty(string datas)
        {
            m_Client.Send(string.Concat("AV", Utilities.Config.GetInt32("SERVERCOM")));
        }

        private void SendCharacterList(string datas)
        {
            var characters = Servers.Characters.Where(x => x.AccountID == m_Client.Account.ID).ToList();
            string packet = string.Format("ALK{0}|{1}", m_Client.Account.SubscriptionTime, characters.Count);

            if (characters.Count != 0)
            {
                foreach (var character in characters)
                    packet += string.Concat("|", character.PatternOnList());
            }

            m_Client.Send(packet);
        }

        private void CreateCharacter(string datas)
        {
            try
            {
                var characterDatas = datas.Split('|');

                if (characterDatas[0] != "" || !Servers.Characters.Any(x => x.Name == characterDatas[0]))
                {
                    var character = new DB_Character()
                    {
                        ID = Servers.Characters.Count > 0 ? Servers.Characters.OrderBy(x => x.ID).ToArray()[0].ID + 1 : 1,
                        Name = characterDatas[0],
                        Level = Utilities.Config.GetInt32("STARTLEVEL"),
                        Class = int.Parse(characterDatas[1]),
                        Sex = int.Parse(characterDatas[2]),
                        Size = 100,
                        Color = int.Parse(characterDatas[3]),
                        Color2 = int.Parse(characterDatas[4]),
                        Color3 = int.Parse(characterDatas[5])
                    };

                    character.Skin = int.Parse(character.Class.ToString() + character.Sex.ToString());

                    switch (character.Class)
                    {
                        case 1:
                            character.MapID = Utilities.Config.GetInt32("STARTMAP_FECA");
                            character.MapCell = Utilities.Config.GetInt32("STARTCELL_FECA");
                            character.MapDir = Utilities.Config.GetInt32("STARTDIR_FECA");
                            break;
                        case 2:
                            character.MapID = Utilities.Config.GetInt32("STARTMAP_OSA");
                            character.MapCell = Utilities.Config.GetInt32("STARTCELL_OSA");
                            character.MapDir = Utilities.Config.GetInt32("STARTDIR_OSA");
                            break;
                        case 3:
                            character.MapID = Utilities.Config.GetInt32("STARTMAP_ENU");
                            character.MapCell = Utilities.Config.GetInt32("STARTCELL_ENU");
                            character.MapDir = Utilities.Config.GetInt32("STARTDIR_ENU");
                            break;
                        case 4:
                            character.MapID = Utilities.Config.GetInt32("STARTMAP_SRAM");
                            character.MapCell = Utilities.Config.GetInt32("STARTCELL_SRAM");
                            character.MapDir = Utilities.Config.GetInt32("STARTDIR_SRAM");
                            break;
                        case 5:
                            character.MapID = Utilities.Config.GetInt32("STARTMAP_XEL");
                            character.MapCell = Utilities.Config.GetInt32("STARTCELL_XEL");
                            character.MapDir = Utilities.Config.GetInt32("STARTDIR_XEL");
                            break;
                        case 6:
                            character.MapID = Utilities.Config.GetInt32("STARTMAP_ECA");
                            character.MapCell = Utilities.Config.GetInt32("STARTCELL_ECA");
                            character.MapDir = Utilities.Config.GetInt32("STARTDIR_ECA");
                            break;
                        case 7:
                            character.MapID = Utilities.Config.GetInt32("STARTMAP_ENI");
                            character.MapCell = Utilities.Config.GetInt32("STARTCELL_ENI");
                            character.MapDir = Utilities.Config.GetInt32("STARTDIR_ENI");
                            break;
                        case 8:
                            character.MapID = Utilities.Config.GetInt32("STARTMAP_IOP");
                            character.MapCell = Utilities.Config.GetInt32("STARTCELL_IOP");
                            character.MapDir = Utilities.Config.GetInt32("STARTDIR_IOP");
                            break;
                        case 9:
                            character.MapID = Utilities.Config.GetInt32("STARTMAP_CRA");
                            character.MapCell = Utilities.Config.GetInt32("STARTCELL_CRA");
                            character.MapDir = Utilities.Config.GetInt32("STARTDIR_CRA");
                            break;
                        case 10:
                            character.MapID = Utilities.Config.GetInt32("STARTMAP_SADI");
                            character.MapCell = Utilities.Config.GetInt32("STARTCELL_SADI");
                            character.MapDir = Utilities.Config.GetInt32("STARTDIR_SADI");
                            break;
                        case 11:
                            character.MapID = Utilities.Config.GetInt32("STARTMAP_SACRI");
                            character.MapCell = Utilities.Config.GetInt32("STARTCELL_SACRI");
                            character.MapDir = Utilities.Config.GetInt32("STARTDIR_SACRI");
                            break;
                        case 12:
                            character.MapID = Utilities.Config.GetInt32("STARTMAP_PANDA");
                            character.MapCell = Utilities.Config.GetInt32("STARTCELL_PANDA");
                            character.MapDir = Utilities.Config.GetInt32("STARTDIR_PANDA");
                            break;
                    }

                    character.CharactPoint = (character.Level - 1) * 5;
                    character.SpellPoint = (character.Level - 1);
                    character.Exp = Servers.Levels.First(x => x.ID == character.Level).Character;
                    character.Kamas = (long)Utilities.Config.GetInt32("STARTKAMAS");

                    if (character.Class < 1 | character.Class > 12 | character.Sex < 0 | character.Sex > 1)
                    {
                        m_Client.Send("AAE");
                        return;
                    }

                    character.LearnSpells();

                    lock (Servers.Characters)
                        Servers.Characters.Add(character);

                    character.Insert();

                    m_Client.Send("AAK");
                    m_Client.Send("TB");
                    SendCharacterList("");
                }
                else
                {
                    m_Client.Send("AAE");
                }
            }
            catch { }
        }

        private void DeleteCharacter(string datas)
        {
            var id = 0;

            if (!int.TryParse(datas.Split('|')[0], out id))
                return;

            if (!Servers.Characters.Any(x => x.ID == id))
                return;

            var character = Servers.Characters.First(x => x.ID == id);

            if (datas.Split('|')[1] != m_Client.Account.Answer && character.Level >= 20)
            {
                m_Client.Send("ADE");
                return;
            }

            character.Delete();

            lock (Servers.Characters)
                Servers.Characters.Remove(character);

            SendCharacterList(string.Empty);
        }

        private void SelectCharacter(string datas)
        {
            var id = 0;

            if (!int.TryParse(datas, out id))
                return;

            if (!Servers.Characters.Any(x => x.ID == id))
                return;

            var character = Servers.Characters.First(x => x.ID == id);
            var characters = Servers.Characters.Where(x => x.AccountID == m_Client.Account.ID).ToList();

            if (characters.Contains(character))
            {
                m_Client.Player = character;
                m_Client.Player.IsConnected = true;
                m_Client.Player.SetClient(m_Client);

                //foreach(var friends in Servers.GameServer.Clients.Where(x => (x as GameClient).Friends.Any(x => x.CharacterFriends

                //foreach (GameClient client in Servers.GameServer.Clients.Where(x => (x as GameClient).Characters.Any(c => c.IsConnected == true)
                //    && (x as GameClient).Friends.Contains(m_Client.Account.Pseudo) && (x as GameClient).Player.Friends.WillNotifyWhenConnected))
                //{
                //    client.Send(string.Concat("Im0143;", m_Client.Player.Name));
                //}

                m_Client.Send(string.Concat("ASK", m_Client.Player.PatternOnSelect()));
            }
            else
                m_Client.Send("ASE");
        }

        #endregion

        #region Gift

        private void SendGifts(string datas)
        {
            m_Client.SendGifts();
        }

        private void AcceptGift(string datas)
        {
            var infos = datas.Split('|');

            var idGift = 0;
            var idChar = 0;

            if (!int.TryParse(infos[0], out idGift) || !int.TryParse(infos[1], out idChar))
                return;

            //if (Client.Characters.Any(x => x.ID == idChar))
            //{
            //    lock (Client.Infos.Gifts)
            //    {
            //        if (Client.Infos.Gifts.Any(x => x.ID == idGift))
            //        {
            //            var myGift = Client.Infos.Gifts.First(e => e.ID == idGift);
            //            Client.Characters.First(x => x.ID == idChar).ItemsInventary.AddItem(myGift.Item, true);

            //            Client.Send("AG0");
            //            Network.ServersHandler.AuthLinks.Send(new Network.Auth.Packets.DeletedGiftPacket().GetPacket(myGift.ID, Client.Infos.ID));

            //            lock (Client.Infos.Gifts)
            //                Client.Infos.Gifts.Remove(myGift);

            //        }
            //        else
            //            Client.Send("AGE");
            //    }
            //}
            //else
            //    Client.Send("AGE");
        }

        #endregion

        #region Date

        private void SendDate(string datas)
        {
            m_Client.Send(string.Concat("BD", Utilities.Basic.GetDofusDate()));
        }

        #endregion

        #region Channels

        private void ChangeChannel(string channel)
        {
            char head;

            if (!char.TryParse(channel.Substring(1), out head))
                return;

            var state = (channel.Substring(0, 1) == "+" ? true : false);
            m_Client.Player.ChangeChannelState(head, state);
        }

        #endregion

        #region Faction

        private void ChangeAlignmentEnable(string enable)
        {
            if (enable == "+")
            {
                if (m_Client.Player.Faction.FactionID != 0)
                    m_Client.Player.Faction.IsEnabled = true;
            }
            else if (enable == "*")
            {
                var hloose = m_Client.Player.Faction.Honor / 100;
                m_Client.Send(string.Concat("GIP", hloose.ToString()));

                return;
            }
            else if (enable == "-")
            {
                var hloose = m_Client.Player.Faction.Honor / 100;

                if (m_Client.Player.Faction.FactionID != 0)
                {
                    m_Client.Player.Faction.IsEnabled = false;
                    m_Client.Player.Faction.Honor -= hloose;
                }
            }

            m_Client.Player.Send(m_Client.Player.ToString());
        }

        #endregion

        #region Friends - Enemies

        private void FriendsList(string datas)
        {
            m_Client.SendFriends();
        }

        private void FriendAdd(string datas)
        {
            m_Client.AddFriend(datas);
        }

        private void FriendDelete(string datas)
        {
            m_Client.RemoveFriend(datas);
        }

        private void FriendsFollow(string datas)
        {
            if (datas.Substring(0, 1) == "+")
                m_Client.WillNotifyWhenConnected = true;
            else
                m_Client.WillNotifyWhenConnected = false;

            m_Client.Send(string.Concat("FO", (m_Client.WillNotifyWhenConnected ? "+" : "-")));
        }

        private void EnemiesList(string datas)
        {
            m_Client.SendEnemies();
        }

        private void EnemyAdd(string datas)
        {
            m_Client.AddEnemy(datas);
        }

        private void EnemyDelete(string datas)
        {
            m_Client.RemoveEnemy(datas);
        }

        #endregion

        #region Zaaps - Zaapis

        private void ExitZaap(string datas)
        {
            m_Client.Send("WV");
        }

        private void ExitZaapis(string datas)
        {
            m_Client.Send("Wv");
        }

        private void UseZaapis(string datas)
        {
            var id = 0;

            if (!int.TryParse(datas, out id))
                return;

            m_Client.Player.Map.OnMoveZaapis(m_Client.Player, id);
        }

        private void UseZaaps(string datas)
        {
            var id = 0;

            if (!int.TryParse(datas, out id))
                return;

            m_Client.Player.Map.OnMoveZaap(m_Client.Player, id);
        }

        #endregion

        #region Chat

        private void ParseChatMessage(string datas)
        {
            var infos = datas.Split('|');

            var channel = infos[0];
            var message = infos[1];

            switch (channel)
            {
                case "*":
                    m_Client.Player.SendGeneralMessage(message);
                    return;

                case "^":
                    m_Client.Player.SendIncarnamMessage(message);
                    return;

                case "$":
                    m_Client.Player.SendPartyMessage(message);
                    return;

                case "%":
                    m_Client.Player.SendGuildMessage(message);
                    return;

                case "#":
                    //TeamMessage
                    return;

                case "?":
                    m_Client.Player.SendRecruitmentMessage(message);
                    return;

                case "!":
                    m_Client.Player.SendFactionMessage(message);
                    return;

                case ":":
                    m_Client.Player.SendTradeMessage(message);
                    return;

                case "@":
                    m_Client.Player.SendAdminMessage(message);
                    return;

                case "¤":
                    //No idea
                    return;

                default:
                    if (channel.Length > 1)
                        m_Client.Player.SendPrivateMessage(channel, message);
                    return;
            }
        }

        #endregion

        #region Guilds

        private void CloseGuildPanel(string datas)
        {
            m_Client.Send(string.Concat("gV", m_Client.Player.Name));
        }

        private void CreateGuild(string datas)
        {
            //TODO VERIF IF HAST THE CLIENT A GILDAOGEME

            if (m_Client.Player.Guild != null)
            {
                m_Client.Player.Send("Ea");
                return;
            }

            var infos = datas.Split('|');

            if (infos.Length < 5)
            {
                m_Client.Send("BN");
                return;
            }

            if (infos[0].Contains("-"))
                infos[0] = "1";
            if (infos[1].Contains("-"))
                infos[1] = "0";
            if (infos[2].Contains("-"))
                infos[2] = "1";
            if (infos[3].Contains("-"))
                infos[3] = "0";

            var bgID = 0;
            var bgColor = 0;
            var embID = 0;
            var embColor = 0;

            if (!int.TryParse(infos[0], out bgID) || !int.TryParse(infos[1], out bgColor) ||
                !int.TryParse(infos[2], out embID) || !int.TryParse(infos[3], out embColor))
            {
                m_Client.Send("BN");
                return;
            }

            //if (infos[4].Length > 15 || Entities.Requests.GuildsRequest.GuildsList.Any(x => x.Name == infos[4]))
            //{
            //    Client.Player.NClient.Send("Ean");
            //    return;
            //}

            //var ID = (Entities.Requests.GuildsRequest.GuildsList.Count < 1 ? 1 : Entities.Requests.GuildsRequest.GuildsList.OrderByDescending(x => x.ID).ToArray()[0].ID + 1);

            //var guild = new World.Game.Guilds.Guild()
            //{
            //    ID = ID,
            //    Name = infos[4],
            //    BgID = bgID,
            //    BgColor = bgColor,
            //    EmbID = embID,
            //    EmbColor = embColor,
            //    Exp = 0,
            //    Level = 1,
            //    CollectorMax = 1,
            //    CollectorProspection = 0,
            //    CollectorWisdom = 0,
            //    CollectorPods = 0,
            //    SaveState = EntityState.New
            //};

            //guild.AddMember(new Game.Guilds.GuildMember(Client.Player));

            //guild.Spells.Add(462, 1);
            //guild.Spells.Add(461, 1);
            //guild.Spells.Add(460, 1);
            //guild.Spells.Add(459, 1);
            //guild.Spells.Add(458, 1);
            //guild.Spells.Add(457, 1);
            //guild.Spells.Add(456, 1);
            //guild.Spells.Add(455, 1);
            //guild.Spells.Add(454, 1);
            //guild.Spells.Add(453, 1);
            //guild.Spells.Add(452, 1);
            //guild.Spells.Add(451, 1);

            //Client.Send(string.Format("gS{0}|{1}|{2}", guild.Name, guild.Emblem.Replace(",", "|"), Utilities.Basic.ToBase36(guild.Members[0].Rights)));
            //Client.Send("gV");

            //Entities.Requests.GuildsRequest.GuildsList.Add(guild);

            ////REMOVE GILDAOGEME
        }

        private void ExitGuild(string datas)
        {
            if (!World.Entities.Requests.CharactersRequests.CharactersList.Any(x => x.Name == datas))
            {
                m_Client.Send("BN");
                return;
            }

            if (datas == m_Client.Player.Name)
            {
                if (m_Client.Player.Guild == null)
                {
                    m_Client.Send("BN");
                    return;
                }

                var guild = m_Client.Player.Guild;

                if (guild.Members.Count < 2)
                {
                    m_Client.Player.NClient.Send("Im1101");
                    return;
                }

                var member = guild.Members.First(x => x.Character == m_Client.Player);

                if (member.Rank == 1)
                {
                    m_Client.Player.NClient.Send("Im1101");
                    return;
                }

                m_Client.Send(string.Format("gKK{0}|{1}", m_Client.Player.Name, m_Client.Player.Name));

                member.Rank = 0;
                member.Rights = 0;
                member.ExpGived = 0;
                member.ExpGaved = 0;

                guild.Members.Remove(member);
                m_Client.Player.Guild = null;
                m_Client.Player.NClient.Send("Im0176");
            }
            else
            {
                var character = World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.Name == datas);

                if (character.Guild == null || m_Client.Player.Guild == null || (m_Client.Player.Guild != character.Guild))
                {
                    m_Client.Send("BN");
                    return;
                }

                var guild = m_Client.Player.Guild;

                if (!guild.Members.First(x => x.Character == m_Client.Player).CanBann)
                {
                    m_Client.Send("Im1101");
                    return;
                }

                var member = guild.Members.First(x => x.Character == character);

                if (member.Rank == 1)
                {
                    m_Client.Player.NClient.Send("Im1101");
                    return;
                }

                if (character.IsConnected)
                    character.NClient.Send(string.Format("gKK{0}|{1}", m_Client.Player.Name, m_Client.Player.Name));

                member.Rank = 0;
                member.Rights = 0;
                member.ExpGived = 0;
                member.ExpGaved = 0;

                m_Client.Player.NClient.Send(string.Concat("Im0177;", character.Name));

                guild.Members.Remove(member);
                character.Guild = null;
            }
        }

        private void ModifyRightGuild(string datas)
        {
            if (m_Client.Player.Guild == null)
            {
                m_Client.Send("BN");
                return;
            }

            var guild = m_Client.Player.Guild;

            if (!guild.Members.First(x => x.Character == m_Client.Player).CanManageRights)
            {
                m_Client.Send("Im1101");
                return;
            }

            var memember = guild.Members.First(x => x.Character == m_Client.Player);
            var infos = datas.Split('|');

            var ID = 0;
            var rank = 0;
            var rights = 0;
            var expgived = 0;

            if (!int.TryParse(infos[0], out ID) || !int.TryParse(infos[1], out rank) || !int.TryParse(infos[3], out rights) ||
                !int.TryParse(infos[2], out expgived) || !World.Entities.Requests.CharactersRequests.CharactersList.Any(x => x.ID == ID))
            {
                m_Client.Send("BN");
                return;
            }

            var character = World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == ID);

            if (character.Name == m_Client.Player.Name && m_Client.Player.Guild.Members.First
                (x => x.Character.Name == m_Client.Player.Name).Rights != rights)
            {
                m_Client.Send("BN");
                return;
            }

            var member = guild.Members.First(x => x.Character == character);

            if (member.Rank == 1 && (member.Rights != rights || member.Rank != rank))
            {
                m_Client.Player.NClient.Send("Im1101");
                return;
            }

            if (expgived > 90)
                expgived = 90;
            else if (expgived < 0)
                expgived = 0;

            if (rank == 1 && member.Rank != 1 && memember.Rank == 1)
            {
                memember.Rank = member.Rank;
                memember.Rights = member.Rights;


                m_Client.Send(string.Format("gS{0}|{1}|{2}", guild.Name, guild.Emblem.Replace(",", "|"), Utilities.Basic.ToBase36(memember.Rights)));

                member.Rank = 1;
                member.Rights = 29695;
                member.ExpGived = expgived;

                if (member.Character.IsConnected)
                    member.Character.NClient.Send(string.Format("gS{0}|{1}|{2}", guild.Name, guild.Emblem.Replace(",", "|"), Utilities.Basic.ToBase36(member.Rights)));

                return;
            }

            member.ExpGived = expgived;
            member.Rights = rights;
            member.Rank = rank;

            if (member.Character.IsConnected)
                member.Character.NClient.Send(string.Format("gS{0}|{1}|{2}", guild.Name, guild.Emblem.Replace(",", "|"), Utilities.Basic.ToBase36(member.Rights)));
        }

        private void UpgradeStatsGuild(string datas)
        {
            if (m_Client.Player.Guild == null)
            {
                m_Client.Send("BN");
                return;
            }

            var guild = m_Client.Player.Guild;

            if (!guild.Members.First(x => x.Character == m_Client.Player).CanManageBoost)
            {
                m_Client.Send("Im1101");
                return;
            }

            switch (datas[0])
            {
                case 'p':

                    if (guild.BoostPoints > 0)
                    {
                        guild.BoostPoints -= 1;
                        guild.CollectorProspection += 1;
                        return;
                    }
                    break;

                case 'x':

                    if (guild.BoostPoints > 0)
                    {
                        guild.BoostPoints -= 1;
                        guild.CollectorWisdom += 1;
                        return;
                    }
                    break;

                case 'o':

                    if (guild.BoostPoints > 0)
                    {
                        guild.BoostPoints -= 1;
                        guild.CollectorPods += 20;
                        return;
                    }
                    break;

                case 'k':

                    if (guild.BoostPoints > 19)
                    {
                        guild.BoostPoints -= 20;
                        guild.CollectorMax += 1;
                        return;
                    }
                    break;
            }

            m_Client.Send("BN");
        }

        private void UpgradeSpellsGuild(string datas)
        {
            if (m_Client.Player.Guild == null)
            {
                m_Client.Send("BN");
                return;
            }

            var guild = m_Client.Player.Guild;

            if (!guild.Members.First(x => x.Character == m_Client.Player).CanManageBoost)
            {
                m_Client.Send("Im1101");
                return;
            }

            var spellID = 0;

            if (!int.TryParse(datas, out spellID) || !guild.Spells.ContainsKey(spellID) || guild.BoostPoints < 5)
            {
                m_Client.Send("BN");
                return;
            }

            guild.Spells[spellID]++;
            guild.BoostPoints -= 5;

            GetGuildInfos("B");
        }

        private void LetCollectorGuild(string datas)
        {
            var map = m_Client.Player.GetMap();

            if (m_Client.Player.Guild == null || map == null)
            {
                m_Client.Send("BN");
                return;
            }

            var guild = m_Client.Player.Guild;

            if (!guild.Members.First(x => x.Character == m_Client.Player).CanHireTaxCollector)
            {
                m_Client.Send("Im1101");
                return;
            }

            if (guild.CollectorMax <= guild.Collectors.Count)
            {
                m_Client.Player.NClient.SendMessage("Vous avez trop de percepteurs !");
                return;
            }

            if (map.Collector != null)
            {
                m_Client.Player.NClient.SendMessage("Un percepteur est déjà présent sur la map !");
                return;
            }

            var ID = m_Client.Player.GetMap().NextNpcID();

            //var collector = new Game.Guilds.GuildCollector(map, Client.Player, ID)
            //{
            //    SaveState = EntityState.New
            //};

            //guild.Collectors.Add(collector);
            //Entities.Requests.CollectorsRequests.CollectorsList.Add(collector);

            //Client.Player.Guild.SendMessage(string.Format("Un percepteur vient d'être posé par <b>{0}</b> en [{1},{2}] !", Client.Player.Name, Client.Player.GetMap().Model.PosX, Client.Player.GetMap().Model.PosY));
            //GetGuildInfos("B");
        }

        private void GetGuildInfos(string datas)
        {
            if (m_Client.Player.Guild == null)
            {
                m_Client.Send("BN");
                return;
            }

            var packet = string.Empty;
            var guild = m_Client.Player.Guild;

            switch (datas[0])
            {
                case 'B':

                    packet = string.Format("gIB{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}", guild.CollectorMax, guild.Collectors.Count, (guild.Level * 100), guild.Level,
                          guild.CollectorPods, guild.CollectorProspection, guild.CollectorWisdom, guild.CollectorMax, guild.BoostPoints, (1000 + (10 * guild.Level)), guild.GetSpells());

                    m_Client.Send(packet);
                    return;

                case 'G':

                    //var lastLevel = Entities.Requests.LevelsRequests.LevelsList.OrderByDescending(x => x.Guild).Where(x => x.Guild <= guild.Exp).ToArray()[0].Guild;
                    //var nextLevel = Entities.Requests.LevelsRequests.LevelsList.OrderBy(x => x.Guild).Where(x => x.Guild > guild.Exp).ToArray()[0].Guild;

                    //packet = string.Format("gIG1|{0}|{1}|{2}|{3}", guild.Level, lastLevel, guild.Exp, nextLevel);

                    //Client.Send(packet);
                    return;

                case 'M':

                    m_Client.Send(string.Concat("gIM+", string.Join("|", from c in guild.Members select c.Character.PatternGuild())));
                    return;

                case 'T':

                    packet = string.Concat("gITM+", string.Join("|", from c in guild.Collectors select c.PatternGuild()));
                    m_Client.Send(packet.Substring(0, packet.Length - 1));
                    return;
            }
        }

        private void GetGuildJoinRequest(string datas)
        {
            switch (datas[0])
            {
                case 'R':

                    if (!World.Entities.Requests.CharactersRequests.CharactersList.Any(x => x.Name == datas.Substring(1)))
                    {
                        m_Client.Send("BN");
                        return;
                    }

                    var receiverCharacter = World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.Name == datas.Substring(1));

                    if (receiverCharacter.Guild != null || m_Client.Player.Guild == null || !receiverCharacter.IsConnected)
                    {
                        if (receiverCharacter.Guild != null)
                        {
                            m_Client.Player.NClient.Send("Im134");
                            return;
                        }

                        m_Client.Send("BN");
                        return;
                    }

                    if (!m_Client.Player.Guild.Members.First(x => x.Character == m_Client.Player).CanInvite)
                    {
                        m_Client.Send("Im1101");
                        return;
                    }

                    m_Client.Player.ReceiverInviteGuild = receiverCharacter.ID;
                    receiverCharacter.SenderInviteGuild = m_Client.Player.ID;

                    m_Client.Player.OnWaitingGuild = true;
                    receiverCharacter.OnWaitingGuild = true;

                    m_Client.Send(string.Concat("gJR", receiverCharacter.Name));
                    receiverCharacter.NClient.Send(string.Format("gJr{0}|{1}|{2}", m_Client.Player.ID, m_Client.Player.Name, m_Client.Player.Guild.Name));

                    break;

                case 'K':

                    var ID = 0;

                    if (!int.TryParse(datas.Substring(1), out ID) || !World.Entities.Requests.CharactersRequests.CharactersList.Any(x => x.ID == ID))
                    {
                        m_Client.Send("BN");
                        return;
                    }

                    var accepttoCharacter = World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == ID);

                    if (!accepttoCharacter.IsConnected || accepttoCharacter.ReceiverInviteGuild != m_Client.Player.ID)
                    {
                        m_Client.Send("BN");
                        return;
                    }

                    m_Client.Player.SenderInviteGuild = -1;
                    accepttoCharacter.ReceiverInviteGuild = -1;

                    m_Client.Player.OnWaitingGuild = false;
                    accepttoCharacter.OnWaitingGuild = false;

                    m_Client.Player.Guild = accepttoCharacter.Guild;
                    var member = new SunDofus.World.Game.Guilds.GuildMember(m_Client.Player);
                    var guild = m_Client.Player.Guild;

                    accepttoCharacter.Guild.Members.Add(member);

                    m_Client.Send(string.Format("gS{0}|{1}|{2}", guild.Name, guild.Emblem.Replace(",", "|"), Utilities.Basic.ToBase36(member.Rights)));
                    accepttoCharacter.NClient.Send(string.Concat("gJKa", m_Client.Player.Name));

                    break;

                case 'E':

                    if (!World.Entities.Requests.CharactersRequests.CharactersList.Any(x => x.Name == datas.Substring(1)))
                    {
                        m_Client.Send("BN");
                        return;
                    }

                    var refusetoCharacter = World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.Name == datas.Substring(1));

                    if (!refusetoCharacter.IsConnected || refusetoCharacter.ReceiverInviteGuild == m_Client.Player.ID)
                    {
                        m_Client.Send("BN");
                        return;
                    }

                    refusetoCharacter.NClient.Send("gJEc");

                    break;
            }
        }

        #endregion

        #region Game

        private void CreateGame(string datas)
        {
            if (m_Client.Player.SaveMap == 0)
            {
                m_Client.Player.SaveMap = m_Client.Player.MapID;
                m_Client.Player.SaveCell = m_Client.Player.MapCell;

                m_Client.Send("Im06");
            }

            m_Client.Send(string.Concat("GCK|1|", m_Client.Player.Name));
            m_Client.Send("AR6bk");

            m_Client.Player.Channels.SendChannels();
            m_Client.Send("SLo+");
            m_Client.Player.SpellsInventary.SendAllSpells();
            m_Client.Send(string.Concat("BT", Utilities.Basic.GetActualTime()));

            if (m_Client.Player.Life == 0)
                m_Client.Player.Life = m_Client.Player.Stats.GetStat(StatEnum.MaxLife).Total;

            m_Client.Player.ItemsInventary.RefreshBonus();
            m_Client.Player.SendPods();
            m_Client.Player.SendChararacterStats();

            m_Client.Player.LoadMap();

            m_Client.Send(string.Concat("FO", (m_Client.Player.Friends.WillNotifyWhenConnected ? "+" : "-")));

            if (m_Client.Player.Guild != null)
            {
                var guild = m_Client.Player.Guild;
                m_Client.Send(string.Format("gS{0}|{1}|{2}", guild.Name, guild.Emblem.Replace(",", "|"), Utilities.Basic.ToBase36(guild.Members.First(x => x.Character == m_Client.Player).Rights)));
            }
        }

        private void TeleportByPos(string packet)
        {
            if (packet[0] != 'M' || !packet.Contains(','))
                return;

            if (m_Client.Account.GMLevel < Utilities.Config.GetInt32("MINGMLEVEL_TOTELEPORTWITH_GEOPOSITION"))
                return;

            var pos = packet.Substring(1).Split(',');
            var posx = 0;
            var posy = 0;

            if (!int.TryParse(pos[0], out posx) || !int.TryParse(pos[1], out posy))
                return;

            var maps = Servers.Maps.Where(x => x.PosX == posx && x.PosY == posy).ToArray();

            if (maps.Length > 0)
            {
                m_Client.Player.TeleportNewMap(maps[0].ID, m_Client.Player.MapCell);
                m_Client.SendConsoleMessage("Character Teleported !", 0);
            }
        }

        private void GameInformations(string datas)
        {
            m_Client.Player.GetMap().AddPlayer(m_Client.Player);
            m_Client.Send("GDK");
        }

        private void GameAction(string datas)
        {
            var packet = 0;

            if (!int.TryParse(datas.Substring(0, 3), out packet))
                return;

            switch (packet)
            {
                case 1:
                    GameMove(datas.Substring(3));
                    return;

                case 500:
                    ParseGameAction(datas.Substring(3));
                    return;

                case 300:
                    FightLaunchSpell(datas.Substring(3));
                    return;

                case 303:
                    FightUseWeapon(datas.Substring(3));
                    return;

                case 900:
                    AskChallenge(datas.Substring(3));
                    return;

                case 901:
                    AcceptChallenge(datas.Substring(3));
                    return;

                case 902:
                    RefuseChallenge(datas.Substring(3));
                    return;

                case 903:
                    FightJoin(datas.Substring(3));
                    return;

                case 906:
                    LauchFactionChallenge(datas.Substring(3));
                    return;
            }
        }

        private void ParseGameAction(string packet)
        {
            var infos = packet.Split(';');

            var id = 0;

            if (!int.TryParse(infos[1], out id))
                return;

            switch (id)
            {
                case 44:
                    SunDofus.World.Game.Maps.Zaaps.ZaapsManager.SaveZaap(m_Client.Player);
                    return;

                case 114:
                    SunDofus.World.Game.Maps.Zaaps.ZaapsManager.SendZaaps(m_Client.Player);
                    return;

                case 157:
                    SunDofus.World.Game.Maps.Zaapis.ZaapisManager.SendZaapis(m_Client.Player);
                    return;

                default:
                    m_Client.Send("BN");
                    return;
            }
        }

        private void GameMove(string packet)
        {
            if (!m_Client.Player.InFight & m_Client.Player.Busy)
                return;

            var path = new SunDofus.World.Game.Maps.Pathfinding(packet, m_Client.Player.GetMap(), (m_Client.Player.InFight ? m_Client.Player.Fighter.Cell : m_Client.Player.MapCell), m_Client.Player.Dir, true);
            var newPath = path.RemakePath();

            if (m_Client.Player.InFight && !m_Client.Player.Fight.TryMove(m_Client.Player.Fighter, path))
                return;

            m_Client.Player.Dir = path.Direction;
            m_Client.Player.MoveToCell = path.Destination;
            m_Client.Player.OnMove = true;

            if (m_Client.Player.InFight)
                m_Client.Player.Fighter.Fight.Send(string.Format("GA0;1;{0};{1}", m_Client.Player.ID, newPath));
            else
                m_Client.Player.GetMap().Send(string.Format("GA0;1;{0};{1}", m_Client.Player.ID, newPath));
        }

        private void EndAction(string datas)
        {
            switch (datas[0])
            {
                case 'K':

                    if (m_Client.Player.OnMove == true)
                    {
                        if (m_Client.Player.InFight)
                            m_Client.Player.Fighter.Cell = m_Client.Player.MoveToCell;
                        else
                            m_Client.Player.MapCell = m_Client.Player.MoveToCell;

                        m_Client.Player.OnMove = false;
                        m_Client.Player.MoveToCell = -1;

                        m_Client.Send("BN");

                        if (m_Client.Player.GetMap().Triggers.Any(x => x.CellID == m_Client.Player.MapCell))
                        {
                            var trigger = m_Client.Player.GetMap().Triggers.First(x => x.CellID == m_Client.Player.MapCell);

                            if (SunDofus.World.Game.World.Conditions.TriggerCondition.HasConditions(m_Client.Player, trigger.Conditions))
                                SunDofus.World.Game.Effects.EffectAction.ParseEffect(m_Client.Player, trigger.ActionID, trigger.Args);
                            else
                                m_Client.SendMessage("Im11");
                        }
                    }

                    return;

                case 'E':

                    var cell = 0;

                    if (!int.TryParse(datas.Split('|')[1], out cell))
                        return;

                    m_Client.Player.OnMove = false;
                    m_Client.Player.MapCell = cell;

                    return;
            }
        }

        private void UseSmiley(string packet)
        {
            int smiley;

            if (!int.TryParse(packet, out smiley))
                return;

            if (smiley < 1 | smiley > 15)
                return;

            if (!m_Client.Player.CanSendinSmiley())
                return;

            m_Client.Player.RefreshSmiley();

            if (m_Client.Player.InFight)
                m_Client.Player.Fight.Send("cS" + m_Client.Player.ID + '|' + smiley);
            else
                m_Client.Player.GetMap().Send("cS" + m_Client.Player.ID + '|' + smiley);
        }

        private void ChangeDirection(string packet)
        {
            int direction;

            if (!int.TryParse(packet, out direction))
                return;

            if (direction < 0 | direction > 7)
                return;

            m_Client.Player.GetMap().Send("eD" + m_Client.Player.ID + "|" + direction);
        }

        private void UseEmote(string packet)
        {
            //JAVA -> Emulator PIOU

            //if (client == null || packet == null || client.getAccount() == null || client.getCharacter() == null)
            //{
            //    return;
            //}
            //Characters c = client.getCharacter();
            //int emote;
            //try
            //{
            //    emote = Integer.parseInt(packet.substring(2));
            //}
            //catch (NumberFormatException ex)
            //{
            //    client.sendPacket(EmotePacket.getInstance().useFail(c));
            //    return;
            //}
            //if (emote <= 0 || !c.hasEmoteNbr(emote))
            //{
            //    client.sendPacket(EmotePacket.getInstance().useFail(c));
            //    return;
            //}
            //c.getLocation().getMap().sendPacket(EmotePacket.getInstance().use(c, emote));
            //if ((emote == 1 || emote == 19) && !c.isSit())
            //{
            //    c.regenLife();
            //    c.setSit(true);
            //}
            //else if (!(emote == 1 || emote == 19) && c.isSit())
            //{
            //    c.regenLife();
            //    c.setSit(false);
            //}

            int emote;

            if (!int.TryParse(packet, out emote))
                return;

            if (emote == 1)
            {
                m_Client.Player.Sit();
                m_Client.Player.GetMap().Send("eUK" + m_Client.Player.ID + '|' + (m_Client.Player.IsSitted ? 1 : 0));
            }
        }

        #endregion

        #region Challenge

        private void AskChallenge(string datas)
        {
            var charid = 0;

            if (!int.TryParse(datas, out charid))
                return;

            if (m_Client.Player.Busy)
                return;

            if (World.Entities.Requests.CharactersRequests.CharactersList.Any(x => x.ID == charid))
            {
                var character = World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == charid);

                if (m_Client.Player.Busy || character.Busy || m_Client.Player.GetMap().Model.ID != character.GetMap().Model.ID)
                {
                    m_Client.SendMessage("Personnage actuellement occupé ou indisponible !");
                    return;
                }

                m_Client.Player.ChallengeAsked = character.ID;
                m_Client.Player.ChallengeAsker = m_Client.Player.ID;
                m_Client.Player.IsChallengeAsker = true;

                character.ChallengeAsker = m_Client.Player.ID;
                character.ChallengeAsked = character.ID;
                character.IsChallengeAsked = true;

                m_Client.Player.GetMap().Send(string.Format("GA;900;{0};{1}", m_Client.Player.ID, character.ID));
            }
        }

        private void AcceptChallenge(string datas)
        {
            var charid = 0;

            if (!int.TryParse(datas, out charid))
                return;

            if (World.Entities.Requests.CharactersRequests.CharactersList.Any(x => x.ID == charid) && m_Client.Player.ChallengeAsker == charid)
            {
                var character = World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == charid);

                m_Client.Player.ChallengeAsker = -1;
                m_Client.Player.ChallengeAsked = -1;
                m_Client.Player.IsChallengeAsked = false;

                character.ChallengeAsked = -1;
                character.ChallengeAsker = -1;
                character.IsChallengeAsker = false;

                m_Client.Send(string.Format("GA;901;{0};{1}", m_Client.Player.ID, character.ID));
                character.NClient.Send(string.Format("GA;901;{0};{1}", character.ID, m_Client.Player.ID));

                m_Client.Player.GetMap().AddFight(new ChallengeFight
                    (m_Client.Player, character, m_Client.Player.GetMap()));
            }
        }

        private void RefuseChallenge(string datas)
        {
            var charid = 0;

            if (!int.TryParse(datas, out charid))
                return;

            if (World.Entities.Requests.CharactersRequests.CharactersList.Any(x => x.ID == charid) && m_Client.Player.ChallengeAsker == charid)
            {
                SunDofus.World.Game.Characters.Character asker = World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == m_Client.Player.ChallengeAsker);
                SunDofus.World.Game.Characters.Character asked = World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == m_Client.Player.ChallengeAsked);

                asker.ChallengeAsked = -1;
                asker.ChallengeAsker = -1;
                asker.IsChallengeAsker = false;

                asked.ChallengeAsker = -1;
                asked.ChallengeAsked = -1;
                asked.IsChallengeAsked = false;

                asker.NClient.Send(string.Format("GA;902;{0};{1}", asker.ID, asked.ID));
                asked.NClient.Send(string.Format("GA;902;{0};{1}", asked.ID, asker.ID));
            }
        }

        private void LauchFactionChallenge(string datas)
        {
            var charid = 0;

            if (!int.TryParse(datas, out charid))
                return;

            if (World.Entities.Requests.CharactersRequests.CharactersList.Any(x => x.ID == charid && x.ID != m_Client.Player.ID))
            {
                var character = World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == charid);

                if (character.Faction.ID != m_Client.Player.Faction.ID && m_Client.Player.Faction.ID != 0)
                {
                    m_Client.Player.GetMap().Send(string.Format("GA;906;{0};{1}", m_Client.Player.ID, character.ID));
                    m_Client.Player.Faction.IsEnabled = true;
                    character.Faction.IsEnabled = true;

                    m_Client.Player.GetMap().AddFight(new ChallengeFight
                        (m_Client.Player, character, m_Client.Player.GetMap(), FightType.AGRESSION));
                }
                else
                    m_Client.Send("BN");
            }
        }

        #endregion

        #region Items

        private void DeleteItem(string datas)
        {
            var allDatas = datas.Split('|');
            var ID = 0;
            var quantity = 0;

            if (!int.TryParse(allDatas[0], out ID) || !int.TryParse(allDatas[1], out quantity) || quantity <= 0)
                return;

            m_Client.Player.ItemsInventary.DeleteItem(ID, quantity);
        }

        private void MoveItem(string datas)
        {
            var allDatas = datas.Split('|');

            var ID = 0;
            var pos = 0;
            var quantity = 1;

            if (allDatas.Length >= 3)
            {
                if (!int.TryParse(allDatas[2], out quantity))
                    return;
            }

            if (!int.TryParse(allDatas[0], out ID) || !int.TryParse(allDatas[1], out pos))
                return;

            m_Client.Player.ItemsInventary.MoveItem(ID, pos, quantity);
        }

        private void UseItem(string datas)
        {
            m_Client.Player.ItemsInventary.UseItem(datas);
        }

        #endregion

        #region StatsBoosts

        private void StatsBoosts(string datas)
        {
            var caract = 0;

            if (!int.TryParse(datas, out caract))
                return;

            var count = 0;

            switch (caract)
            {
                case 11:

                    if (m_Client.Player.CharactPoint < 1)
                        return;

                    if (m_Client.Player.Class == 11)
                    {
                        m_Client.Player.Stats.GetStat(StatEnum.Vitalite).Base += 2;
                        m_Client.Player.Life += 2;
                    }
                    else
                    {
                        m_Client.Player.Stats.GetStat(StatEnum.Vitalite).Base += 1;
                        m_Client.Player.Life += 1;
                    }

                    m_Client.Player.CharactPoint -= 1;
                    m_Client.Player.SendChararacterStats();

                    break;

                case 12:

                    if (m_Client.Player.CharactPoint < 3)
                        return;

                    m_Client.Player.Stats.GetStat(StatEnum.Sagesse).Base += 1;
                    m_Client.Player.CharactPoint -= 3;
                    m_Client.Player.SendChararacterStats();

                    break;

                case 10:

                    if (m_Client.Player.Class == 1 | m_Client.Player.Class == 7 | m_Client.Player.Class == 2 | m_Client.Player.Class == 5)
                    {
                        if (m_Client.Player.Stats.GetStat(StatEnum.Force).Base < 51) count = 2;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Force).Base > 50) count = 3;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Force).Base > 150) count = 4;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Force).Base > 250) count = 5;
                    }

                    else if (m_Client.Player.Class == 3 | m_Client.Player.Class == 9)
                    {
                        if (m_Client.Player.Stats.GetStat(StatEnum.Force).Base < 51) count = 1;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Force).Base > 50) count = 2;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Force).Base > 150) count = 3;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Force).Base > 250) count = 4;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Force).Base > 350) count = 5;
                    }

                    else if (m_Client.Player.Class == 4 | m_Client.Player.Class == 6 | m_Client.Player.Class == 8 | m_Client.Player.Class == 10)
                    {
                        if (m_Client.Player.Stats.GetStat(StatEnum.Force).Base < 101) count = 1;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Force).Base > 100) count = 2;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Force).Base > 200) count = 3;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Force).Base > 300) count = 4;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Force).Base > 400) count = 5;
                    }

                    else if (m_Client.Player.Class == 11)
                    {
                        count = 3;
                    }

                    else if (m_Client.Player.Class == 12)
                    {
                        if (m_Client.Player.Stats.GetStat(StatEnum.Force).Base < 51) count = 1;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Force).Base > 50) count = 2;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Force).Base > 200) count = 3;
                    }

                    if (m_Client.Player.CharactPoint >= count)
                    {
                        m_Client.Player.Stats.GetStat(StatEnum.Force).Base += 1;
                        m_Client.Player.CharactPoint -= count;
                        m_Client.Player.SendChararacterStats();
                    }
                    else
                        m_Client.Send("ABE");

                    break;

                case 15:

                    if (m_Client.Player.Class == 1 | m_Client.Player.Class == 2 | m_Client.Player.Class == 5 | m_Client.Player.Class == 7 | m_Client.Player.Class == 10)
                    {
                        if (m_Client.Player.Stats.GetStat(StatEnum.Intelligence).Base < 101) count = 1;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Intelligence).Base > 100) count = 2;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Intelligence).Base > 200) count = 3;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Intelligence).Base > 300) count = 4;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Intelligence).Base > 400) count = 5;
                    }

                    else if (m_Client.Player.Class == 3)
                    {
                        if (m_Client.Player.Stats.GetStat(StatEnum.Intelligence).Base < 21) count = 1;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Intelligence).Base > 20) count = 2;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Intelligence).Base > 60) count = 3;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Intelligence).Base > 100) count = 4;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Intelligence).Base > 140) count = 5;
                    }

                    else if (m_Client.Player.Class == 4)
                    {
                        if (m_Client.Player.Stats.GetStat(StatEnum.Intelligence).Base < 51) count = 1;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Intelligence).Base > 50) count = 2;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Intelligence).Base > 150) count = 3;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Intelligence).Base > 250) count = 4;
                    }

                    else if (m_Client.Player.Class == 6 | m_Client.Player.Class == 8)
                    {
                        if (m_Client.Player.Stats.GetStat(StatEnum.Intelligence).Base < 21) count = 1;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Intelligence).Base > 20) count = 2;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Intelligence).Base > 40) count = 3;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Intelligence).Base > 60) count = 4;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Intelligence).Base > 80) count = 5;
                    }

                    else if (m_Client.Player.Class == 9)
                    {
                        if (m_Client.Player.Stats.GetStat(StatEnum.Intelligence).Base < 51) count = 1;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Intelligence).Base > 50) count = 2;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Intelligence).Base > 150) count = 3;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Intelligence).Base > 250) count = 4;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Intelligence).Base > 350) count = 5;
                    }

                    else if (m_Client.Player.Class == 11)
                    {
                        count = 3;
                    }

                    else if (m_Client.Player.Class == 12)
                    {
                        if (m_Client.Player.Stats.GetStat(StatEnum.Intelligence).Base < 51) count = 1;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Intelligence).Base > 50) count = 2;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Intelligence).Base > 200) count = 3;
                    }

                    if (m_Client.Player.CharactPoint >= count)
                    {
                        m_Client.Player.Stats.GetStat(StatEnum.Intelligence).Base += 1;
                        m_Client.Player.CharactPoint -= count;
                        m_Client.Player.SendChararacterStats();
                    }
                    else
                        m_Client.Send("ABE");

                    break;

                case 13:

                    if (m_Client.Player.Class == 1 | m_Client.Player.Class == 4 | m_Client.Player.Class == 5
                        | m_Client.Player.Class == 6 | m_Client.Player.Class == 7 | m_Client.Player.Class == 8 | m_Client.Player.Class == 9)
                    {
                        if (m_Client.Player.Stats.GetStat(StatEnum.Chance).Base < 21) count = 1;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Chance).Base > 20) count = 2;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Chance).Base > 40) count = 3;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Chance).Base > 60) count = 4;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Chance).Base > 80) count = 5;
                    }

                    else if (m_Client.Player.Class == 2 | m_Client.Player.Class == 10)
                    {
                        if (m_Client.Player.Stats.GetStat(StatEnum.Chance).Base < 101) count = 1;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Chance).Base > 100) count = 2;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Chance).Base > 200) count = 3;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Chance).Base > 300) count = 4;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Chance).Base > 400) count = 5;
                    }

                    else if (m_Client.Player.Class == 3)
                    {
                        if (m_Client.Player.Stats.GetStat(StatEnum.Chance).Base < 101) count = 1;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Chance).Base > 100) count = 2;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Chance).Base > 150) count = 3;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Chance).Base > 230) count = 4;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Chance).Base > 330) count = 5;
                    }

                    else if (m_Client.Player.Class == 11)
                    {
                        count = 3;
                    }

                    else if (m_Client.Player.Class == 12)
                    {
                        if (m_Client.Player.Stats.GetStat(StatEnum.Chance).Base < 51) count = 1;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Chance).Base > 50) count = 2;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Chance).Base > 200) count = 3;
                    }

                    if (m_Client.Player.CharactPoint >= count)
                    {
                        m_Client.Player.Stats.GetStat(StatEnum.Chance).Base += 1;
                        m_Client.Player.CharactPoint -= count;
                        m_Client.Player.SendChararacterStats();
                    }
                    else
                        m_Client.Send("ABE");

                    break;

                case 14:

                    if (m_Client.Player.Class == 1 | m_Client.Player.Class == 2 | m_Client.Player.Class == 3 | m_Client.Player.Class == 5
                        | m_Client.Player.Class == 7 | m_Client.Player.Class == 8 | m_Client.Player.Class == 10)
                    {
                        if (m_Client.Player.Stats.GetStat(StatEnum.Agilite).Base < 21) count = 1;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Agilite).Base > 20) count = 2;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Agilite).Base > 40) count = 3;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Agilite).Base > 60) count = 4;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Agilite).Base > 80) count = 5;
                    }

                    else if (m_Client.Player.Class == 4)
                    {
                        if (m_Client.Player.Stats.GetStat(StatEnum.Agilite).Base < 101) count = 1;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Agilite).Base > 100) count = 2;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Agilite).Base > 200) count = 3;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Agilite).Base > 300) count = 4;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Agilite).Base > 400) count = 5;
                    }

                    else if (m_Client.Player.Class == 6 | m_Client.Player.Class == 9)
                    {
                        if (m_Client.Player.Stats.GetStat(StatEnum.Agilite).Base < 51) count = 1;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Agilite).Base > 50) count = 2;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Agilite).Base > 100) count = 3;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Agilite).Base > 150) count = 4;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Agilite).Base > 200) count = 5;
                    }

                    else if (m_Client.Player.Class == 11)
                    {
                        count = 3;
                    }

                    else if (m_Client.Player.Class == 12)
                    {
                        if (m_Client.Player.Stats.GetStat(StatEnum.Agilite).Base < 51) count = 1;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Agilite).Base > 50) count = 2;
                        if (m_Client.Player.Stats.GetStat(StatEnum.Agilite).Base > 200) count = 3;
                    }

                    if (m_Client.Player.CharactPoint >= count)
                    {
                        m_Client.Player.Stats.GetStat(StatEnum.Agilite).Base += 1;
                        m_Client.Player.CharactPoint -= count;
                        m_Client.Player.SendChararacterStats();
                    }
                    else
                        m_Client.Send("ABE");

                    break;
            }
        }

        #endregion

        #region Spells

        private void SpellBoost(string datas)
        {
            var spellID = 0;

            if (!int.TryParse(datas, out spellID))
                return;

            if (!m_Client.Player.SpellsInventary.Spells.Any(x => x.ID == spellID))
            {
                m_Client.Send("SUE");
                return;
            }

            var level = m_Client.Player.SpellsInventary.Spells.First(x => x.ID == spellID).Level;

            if (m_Client.Player.SpellPoint < level || level >= 6)
            {
                m_Client.Send("SUE");
                return;
            }

            m_Client.Player.SpellPoint -= level;

            var spell = m_Client.Player.SpellsInventary.Spells.First(x => x.ID == spellID);
            spell.ChangeLevel(spell.Level + 1);

            m_Client.Send(string.Format("SUK{0}~{1}", spellID, level + 1));
            m_Client.Player.SendChararacterStats();
        }

        private void SpellMove(string _datas)
        {
            m_Client.Send("BN");

            var datas = _datas.Split('|');
            var spellID = 0;
            var newPos = 0;

            if (!int.TryParse(datas[0], out spellID) || !int.TryParse(datas[1], out newPos))
                return;

            if (!m_Client.Player.SpellsInventary.Spells.Any(x => x.ID == spellID))
                return;

            if (m_Client.Player.SpellsInventary.Spells.Any(x => x.Position == newPos))
            {
                m_Client.Player.SpellsInventary.Spells.First(x => x.Position == newPos).Position = 25;
                m_Client.Player.SpellsInventary.Spells.First(x => x.ID == spellID).Position = newPos;
            }
            else
                m_Client.Player.SpellsInventary.Spells.First(x => x.ID == spellID).Position = newPos;
        }

        #endregion

        #region Exchange

        private void ExchangeRequest(string datas)
        {
            if (m_Client.Player == null || m_Client.Player.Busy)
            {
                m_Client.Send("BN");
                return;
            }

            var packet = datas.Split('|');
            var ID = 0;
            var receiverID = 0;

            if (!int.TryParse(packet[0], out ID) || !int.TryParse(packet[1], out receiverID))
                return;

            switch (ID)
            {
                case 0://NPC BUY/SELL
                    {
                        var NPC = m_Client.Player.GetMap().Npcs.First(x => x.ID == receiverID);

                        if (NPC.Model.SellingList.Count == 0)
                        {
                            m_Client.Send("BN");
                            return;
                        }

                        m_Client.Player.OnExchange = true;
                        m_Client.Player.ActualNPC = NPC.ID;

                        m_Client.Send(string.Concat("ECK0|", NPC.ID));

                        var newPacket = "EL";

                        //foreach (var i in NPC.Model.SellingList)
                        //{
                        //    var item = Entities.Requests.ItemsRequests.ItemsList.First(x => x.ID == i);
                        //    newPacket += string.Format("{0};{1}|", i, item.EffectInfos());
                        //}

                        m_Client.Send(newPacket.Substring(0, newPacket.Length - 1));

                        break;
                    }
                case 1://Player
                    {
                        if (World.Entities.Requests.CharactersRequests.CharactersList.Any(x => x.ID == receiverID))
                        {
                            var character = World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == receiverID);

                            if (!character.IsConnected == true && !character.Busy)
                            {
                                m_Client.Send("BN");
                                return;
                            }

                            character.NClient.Send(string.Format("ERK{0}|{1}|1", m_Client.Player.ID, character.ID));
                            m_Client.Send(string.Format("ERK{0}|{1}|1", m_Client.Player.ID, character.ID));

                            character.CurrentPlayerTrade = m_Client.Player.ID;
                            character.OnExchange = true;

                            m_Client.Player.CurrentPlayerTrade = character.ID;
                            m_Client.Player.OnExchange = true;
                        }

                        break;
                    }
                case 2: // Trade with NPC
                    {

                        break;
                    }
            }
        }

        private void CancelExchange(string t)
        {
            m_Client.Send("EV");

            if (m_Client.Player.OnExchange)
                SunDofus.World.Game.Exchanges.ExchangesManager.LeaveExchange(m_Client.Player);
            else if (m_Client.Player.OnExchangeWithBank)
                m_Client.Player.OnExchangeWithBank = false;
        }

        private void ExchangeBuy(string packet)
        {
            //if (!Client.Player.OnExchange)
            //{
            //    Client.Send("EBE");
            //    return;
            //}

            //var datas = packet.Split('|');
            //var itemID = 0;
            //var quantity = 1;

            //if (!int.TryParse(datas[0], out itemID) || !int.TryParse(datas[1], out quantity))
            //    return;

            //var item = Entities.Requests.ItemsRequests.ItemsList.First(x => x.ID == itemID);
            //var NPC = Client.Player.GetMap().Npcs.First(x => x.ID == Client.Player.ActualNPC);

            //if (quantity <= 0 || !NPC.Model.SellingList.Contains(itemID))
            //{
            //    Client.Send("EBE");
            //    return;
            //}

            //var price = item.Price * quantity;

            //if (Client.Player.Kamas >= price)
            //{
            //    var newItem = new SunDofus.World.Game.Characters.Items.CharacterItem(item);
            //    newItem.GeneratItem(4);
            //    newItem.Quantity = quantity;


            //    Client.Player.Kamas -= price;
            //    Client.Send("EBK");
            //    Client.Player.ItemsInventary.AddItem(newItem, false);
            //}
            //else
            //    Client.Send("EBE");
        }

        private void ExchangeSell(string datas)
        {
            if (!m_Client.Player.Busy)
            {
                m_Client.Send("OSE");
                return;
            }

            var packet = datas.Split('|');

            var itemID = 0;
            var quantity = 1;

            if (!int.TryParse(packet[0], out itemID) || !int.TryParse(packet[1], out quantity))
                return;

            if (!m_Client.Player.ItemsInventary.ItemsList.Any(x => x.ID == itemID) || quantity <= 0)
            {
                m_Client.Send("OSE");
                return;
            }

            var item = m_Client.Player.ItemsInventary.ItemsList.First(x => x.ID == itemID);

            if (item.Quantity < quantity)
                quantity = item.Quantity;

            var price = Math.Floor((double)item.Model.Price / 10) * quantity;

            if (price < 1)
                price = 1;

            m_Client.Player.Kamas += (int)price;
            m_Client.Player.ItemsInventary.DeleteItem(item.ID, quantity);
            m_Client.Send("ESK");
        }

        private void ExchangeMove(string datas)
        {
            switch (datas[0])
            {
                case 'G': //kamas

                    if (m_Client.Player.OnExchangeWithBank)
                    {
                        var length = (long)0;
                        var addkamas = true;

                        if (!long.TryParse(datas.Substring(1), out length))
                            return;

                        if (datas[1] == '-')
                        {
                            addkamas = false;
                            if (!long.TryParse(datas.Substring(2), out length))
                                return;
                        }

                        World.Game.Bank.BanksManager.FindExchange(m_Client.Player).MoveKamas(length, addkamas);
                        return;
                    }

                    var character = World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == m_Client.Player.CurrentPlayerTrade);

                    if (!m_Client.Player.OnExchangePanel || !character.OnExchangePanel || character.CurrentPlayerTrade != m_Client.Player.ID)
                    {
                        m_Client.Send("EME");
                        return;
                    }

                    var actualExchange = SunDofus.World.Game.Exchanges.ExchangesManager.Exchanges.First(x => (x.memberOne.Character.ID == m_Client.Player.ID &&
                        x.memberTwo.Character.ID == character.ID) || (x.memberTwo.Character.ID == m_Client.Player.ID && x.memberOne.Character.ID == character.ID));

                    long kamas = 0;

                    if (!long.TryParse(datas.Substring(1), out kamas))
                        return;

                    if (kamas > m_Client.Player.Kamas)
                        kamas = m_Client.Player.Kamas;
                    else if (kamas < 0)
                        kamas = 0;

                    actualExchange.MoveGold(m_Client.Player, kamas);

                    break;

                case 'O': //Items

                    var itemID = 0;
                    var quantity = 0;
                    var infos = new string[0];

                    if (m_Client.Player.OnExchangeWithBank)
                    {
                        var additem = true;
                        infos = datas.Substring(2).Split('|');

                        if (datas[1] == '-')
                            additem = false;

                        itemID = 0;
                        quantity = 0;
                        SunDofus.World.Game.Characters.Items.CharacterItem item = null;

                        if (!int.TryParse(infos[0], out itemID) || !int.TryParse(infos[1], out quantity))
                        {
                            m_Client.Send("EME");
                            return;
                        }

                        if (additem)
                        {
                            if (m_Client.Player.ItemsInventary.ItemsList.Any(x => x.ID == itemID))
                                item = m_Client.Player.ItemsInventary.ItemsList.First(x => x.ID == itemID);
                            else
                                return;
                        }
                        else
                        {
                            var bank = SunDofus.World.Game.Bank.BanksManager.FindExchange(m_Client.Player).Bank;

                            if (bank.Items.Any(x => x.ID == itemID))
                                item = bank.Items.First(x => x.ID == itemID);
                            else
                                return;
                        }

                        if (quantity <= 0)
                            quantity = 1;
                        else if (quantity > item.Quantity)
                            quantity = item.Quantity;

                        SunDofus.World.Game.Bank.BanksManager.FindExchange(m_Client.Player).MoveItem(item, quantity, additem);
                        return;
                    }

                    var character2 = World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == m_Client.Player.CurrentPlayerTrade);

                    if (!m_Client.Player.OnExchangePanel || !character2.OnExchangePanel || character2.CurrentPlayerTrade != m_Client.Player.ID)
                    {
                        m_Client.Send("EME");
                        return;
                    }

                    var actualExchange2 = SunDofus.World.Game.Exchanges.ExchangesManager.Exchanges.First(x => (x.memberOne.Character.ID == m_Client.Player.ID &&
                        x.memberTwo.Character.ID == character2.ID) || (x.memberTwo.Character.ID == m_Client.Player.ID && x.memberOne.Character.ID == character2.ID));

                    var add = (datas.Substring(1, 1) == "+" ? true : false);
                    infos = datas.Substring(2).Split('|');

                    itemID = 0;
                    quantity = 0;

                    if (!int.TryParse(infos[0], out itemID) || !int.TryParse(infos[1], out quantity))
                        return;

                    var charItem = m_Client.Player.ItemsInventary.ItemsList.First(x => x.ID == itemID);
                    if (charItem.Quantity < quantity)
                        quantity = charItem.Quantity;
                    if (quantity < 1)
                        return;

                    actualExchange2.MoveItem(m_Client.Player, charItem, quantity, add);

                    break;
            }
        }

        private void ExchangeAccept(string datas)
        {
            if (m_Client.Player.OnExchange && m_Client.Player.CurrentPlayerTrade != -1)
            {
                var character = SunDofus.World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == m_Client.Player.CurrentPlayerTrade);
                if (character.CurrentPlayerTrade == m_Client.Player.ID)
                {
                    SunDofus.World.Game.Exchanges.ExchangesManager.AddExchange(character, m_Client.Player);
                    return;
                }
            }
            m_Client.Send("BN");
        }

        private void ExchangeValidate(string datas)
        {
            if (!m_Client.Player.OnExchange)
            {
                m_Client.Send("BN");
                return;
            }

            m_Client.Player.OnExchangeAccepted = true;

            var character = World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == m_Client.Player.CurrentPlayerTrade);

            if (!m_Client.Player.OnExchangePanel || !character.OnExchangePanel || character.CurrentPlayerTrade != m_Client.Player.ID)
            {
                m_Client.Send("EME");
                return;
            }

            var actualExchange = SunDofus.World.Game.Exchanges.ExchangesManager.Exchanges.First(x => (x.memberOne.Character.ID == m_Client.Player.ID &&
                x.memberTwo.Character.ID == character.ID) || (x.memberTwo.Character.ID == m_Client.Player.ID && x.memberOne.Character.ID == character.ID));

            m_Client.Send(string.Concat("EK1", m_Client.Player.ID));
            character.NClient.Send(string.Concat("EK1", m_Client.Player.ID));

            if (character.OnExchangeAccepted)
                actualExchange.ValideExchange();
        }

        #endregion

        #region Party

        private void PartyInvite(string datas)
        {
            if (World.Entities.Requests.CharactersRequests.CharactersList.Any(x => x.Name == datas && x.IsConnected))
            {
                var character = World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.Name == datas);
                if (character.Party != null || character.Busy)
                {
                    m_Client.Send(string.Concat("PIEa", datas));
                    return;
                }

                if (m_Client.Player.Party != null)
                {
                    if (m_Client.Player.Party.Members.Count < 8)
                    {
                        character.SenderInviteParty = m_Client.Player.ID;
                        character.OnWaitingParty = true;
                        m_Client.Player.ReceiverInviteParty = character.ID;
                        m_Client.Player.OnWaitingParty = true;

                        m_Client.Send(string.Format("PIK{0}|{1}", m_Client.Player.Name, character.Name));
                        character.NClient.Send(string.Format("PIK{0}|{1}", m_Client.Player.Name, character.Name));
                    }
                    else
                    {
                        m_Client.Send(string.Concat("PIEf", datas));
                        return;
                    }
                }
                else
                {
                    character.SenderInviteParty = m_Client.Player.ID;
                    character.OnWaitingParty = true;
                    m_Client.Player.ReceiverInviteParty = character.ID;
                    m_Client.Player.OnWaitingParty = true;

                    m_Client.Send(string.Format("PIK{0}|{1}", m_Client.Player.Name, character.Name));
                    character.NClient.Send(string.Format("PIK{0}|{1}", m_Client.Player.Name, character.Name));
                }
            }
            else
                m_Client.Send(string.Concat("PIEn", datas));
        }

        private void PartyRefuse(string datas)
        {
            if (m_Client.Player.SenderInviteParty == -1)
            {
                m_Client.Send("BN");
                return;
            }

            var character = World.Entities.Requests.CharactersRequests.CharactersList.First
                (x => x.ID == m_Client.Player.SenderInviteParty);

            if (character.IsConnected == false || character.ReceiverInviteParty != m_Client.Player.ID)
            {
                m_Client.Send("BN");
                return;
            }

            character.ReceiverInviteParty = -1;
            character.OnWaitingParty = false;

            m_Client.Player.SenderInviteParty = -1;
            m_Client.Player.OnWaitingParty = false;

            character.NClient.Send("PR");
        }

        private void PartyAccept(string datas)
        {
            if (m_Client.Player.SenderInviteParty != -1 && m_Client.Player.OnWaitingParty)
            {
                var character = World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == m_Client.Player.SenderInviteParty);

                if (character.IsConnected == false || character.ReceiverInviteParty != m_Client.Player.ID)
                {
                    m_Client.Player.SenderInviteParty = -1;
                    m_Client.Player.OnWaitingParty = false;
                    m_Client.Send("BN");
                    return;
                }

                m_Client.Player.SenderInviteParty = -1;
                m_Client.Player.OnWaitingParty = false;

                character.ReceiverInviteParty = -1;
                character.OnWaitingParty = false;

                if (character.Party == null)
                {
                    character.Party = new CharacterParty(character);
                    character.Party.AddMember(m_Client.Player);
                }
                else
                {
                    if (character.Party.Members.Count > 7)
                    {
                        m_Client.Send("BN");
                        character.NClient.Send("PR");
                        return;
                    }
                    character.Party.AddMember(m_Client.Player);
                }

                character.NClient.Send("PR");
            }
            else
            {
                m_Client.Player.SenderInviteParty = -1;
                m_Client.Player.OnWaitingParty = false;
                m_Client.Send("BN");
            }
        }

        private void PartyLeave(string datas)
        {
            if (m_Client.Player.Party == null || !m_Client.Player.Party.Members.Keys.Contains(m_Client.Player))
            {
                m_Client.Send("BN");
                return;
            }

            if (datas == "")
                m_Client.Player.Party.LeaveParty(m_Client.Player.Name);
            else
            {
                var character = m_Client.Player.Party.Members.Keys.ToList().First(x => x.ID == int.Parse(datas));
                m_Client.Player.Party.LeaveParty(character.Name, m_Client.Player.ID.ToString());
            }
        }

        private void PartyFollow(string datas)
        {
            var add = (datas.Substring(0, 1) == "+" ? true : false);
            var charid = 0;

            if (!int.TryParse(datas.Substring(1, datas.Length - 1), out charid))
                return;

            var character = World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == charid);

            if (add)
            {
                if (!character.IsConnected || m_Client.Player.IsFollowing)
                {
                    m_Client.Send("BN");
                    return;
                }

                if (character.Party == null || !character.Party.Members.ContainsKey(m_Client.Player)
                    || character.Followers.Contains(m_Client.Player))
                {
                    m_Client.Send("BN");
                    return;
                }

                lock (character.Followers)
                    character.Followers.Add(m_Client.Player);

                character.IsFollow = true;
                character.NClient.Send(string.Concat("Im052;", m_Client.Player.Name));

                m_Client.Player.FollowingID = character.ID;
                m_Client.Player.IsFollowing = true;

                m_Client.Send(string.Format("IC{0}|{1}", character.GetMap().Model.PosX, character.GetMap().Model.PosY));
                m_Client.Send(string.Concat("PF+", character.ID));
            }
            else
            {
                if (character.Party == null || !character.Party.Members.ContainsKey(m_Client.Player)
                    || !character.Followers.Contains(m_Client.Player) || character.ID != m_Client.Player.FollowingID)
                {
                    m_Client.Send("BN");
                    return;
                }

                lock (character.Followers)
                    character.Followers.Remove(m_Client.Player);

                character.IsFollow = false;
                character.NClient.Send(string.Concat("Im053;", m_Client.Player.Name));

                m_Client.Player.FollowingID = -1;
                m_Client.Player.IsFollowing = false;

                m_Client.Send("IC|");
                m_Client.Send("PF-");
            }
        }

        private void PartyGroupFollow(string datas)
        {
            var add = (datas.Substring(0, 1) == "+" ? true : false);
            var charid = 0;

            if (!int.TryParse(datas.Substring(1, datas.Length - 1), out charid))
                return;

            var character = World.Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == charid);

            if (add)
            {
                if (!character.IsConnected || character.Party == null || !character.Party.Members.ContainsKey(m_Client.Player))
                {
                    m_Client.Send("BN");
                    return;
                }

                foreach (var charinparty in character.Party.Members.Keys.Where(x => x != character))
                {
                    if (charinparty.IsFollowing)
                        charinparty.NClient.Send("PF-");

                    lock (character.Followers)
                        character.Followers.Add(m_Client.Player);

                    character.NClient.Send(string.Concat("Im052;", m_Client.Player.Name));

                    charinparty.FollowingID = character.ID;
                    charinparty.IsFollowing = true;

                    charinparty.NClient.Send(string.Format("IC{0}|{1}", character.GetMap().Model.PosX, character.GetMap().Model.PosY));
                    charinparty.NClient.Send(string.Concat("PF+", character.ID));
                }

                character.IsFollow = true;
            }
            else
            {
                if (character.Party == null || !character.Party.Members.ContainsKey(m_Client.Player))
                {
                    m_Client.Send("BN");
                    return;
                }

                foreach (var charinparty in character.Party.Members.Keys.Where(x => x != character))
                {
                    lock (character.Followers)
                        character.Followers.Remove(m_Client.Player);

                    character.NClient.Send(string.Concat("Im053;", m_Client.Player.Name));

                    charinparty.FollowingID = -1;
                    charinparty.IsFollowing = false;

                    charinparty.NClient.Send("IC|");
                    charinparty.NClient.Send("PF-");
                }

                character.IsFollow = false;
            }
        }

        #endregion

        #region Dialogs

        private void DialogCreate(string datas)
        {
            var id = 0;

            if (!int.TryParse(datas, out id))
                return;

            if ((!m_Client.Player.GetMap().Npcs.Any(x => x.ID == id) && m_Client.Player.GetMap().Collector.ID != id) || m_Client.Player.Busy)
            {
                m_Client.Send("BN");
                return;
            }

            if (m_Client.Player.GetMap().Npcs.Any(x => x.ID == id)) //Is also a NPC
            {
                var npc = m_Client.Player.GetMap().Npcs.First(x => x.ID == id);

                if (npc.Model.Question == null)
                {
                    m_Client.Send("BN");
                    m_Client.SendMessage("Dialogue inexistant !");
                    return;
                }

                m_Client.Player.OnDialoging = true;
                m_Client.Player.OnDialogingWith = npc.ID;

                m_Client.Send(string.Concat("DCK", npc.ID));

                var packet = string.Concat("DQ", npc.Model.Question.QuestionID);

                if (npc.Model.Question.Params.Count > 0)
                {
                    packet = string.Concat(packet, ";");

                    foreach (var param in npc.Model.Question.Params)
                        packet = string.Concat(m_Client.Player.GetParam(param), ",");

                    packet = packet.Substring(0, packet.Length - 1);
                }

                packet = string.Concat(packet, "|");

                if (npc.Model.Question.Answers.Count(x => x.HasConditions(m_Client.Player)) != 0)
                {
                    foreach (var answer in npc.Model.Question.Answers)
                    {
                        if (answer.HasConditions(m_Client.Player))
                            packet += string.Concat(answer.AnswerID, ";");
                    }
                }

                m_Client.Send(packet.Substring(0, packet.Length - 1));
            }
            else //Is also a collector
            {
                var collector = m_Client.Player.GetMap().Collector;

                m_Client.Player.OnDialoging = true;
                m_Client.Player.OnDialogingWith = collector.ID;
                m_Client.Send(string.Concat("DCK", collector.ID));

                var packet = string.Format("DQ1;{0},{1},{2},{3},{4}",
                    collector.Guild.Name, collector.Guild.CollectorPods, collector.Guild.CollectorProspection, collector.Guild.CollectorWisdom, collector.Guild.Collectors.Count);

                m_Client.Send(packet);
            }
        }

        private void DialogReply(string datas)
        {
            var id = 0;

            if (!int.TryParse(datas.Split('|')[1], out id))
                return;

            if (!m_Client.Player.GetMap().Npcs.Any(x => x.ID == m_Client.Player.OnDialogingWith))
            {
                m_Client.Send("BN");
                return;
            }

            var npc = m_Client.Player.GetMap().Npcs.First(x => x.ID == m_Client.Player.OnDialogingWith);

            if (!npc.Model.Question.Answers.Any(x => x.AnswerID == id))
            {
                m_Client.Send("BN");
                return;
            }

            var answer = npc.Model.Question.Answers.First(x => x.AnswerID == id);

            if (!answer.HasConditions(m_Client.Player))
            {
                m_Client.Send("BN");
                return;
            }

            answer.ApplyEffects(m_Client.Player);
            DialogExit("");
        }

        private void DialogExit(string datas)
        {
            m_Client.Send("DV");

            m_Client.Player.OnDialogingWith = -1;
            m_Client.Player.OnDialoging = false;
        }

        #endregion

        #region Fights (out)

        private void FightDetails(string packet)
        {
            int ID = 0;

            if (!int.TryParse(packet, out ID))
                return;

            Fight fight = m_Client.Player.GetMap().Fights.Find(x => x.ID == ID);

            if (fight != null)
            {
                StringBuilder builder = new StringBuilder("fD").Append(fight.ID).Append('|');

                foreach (Fighter fighter in fight.Team1.GetAliveFighters())
                    builder.Append(fighter.Name).Append('~').Append(fighter.Level).Append(';');

                builder.Append('|');

                foreach (Fighter fighter in fight.Team2.GetAliveFighters())
                    builder.Append(fighter.Name).Append('~').Append(fighter.Level).Append(';');

                m_Client.Send(builder.ToString());
            }
        }

        private void FightList(string packet)
        {
            StringBuilder builder;
            List<String> fights = new List<String>();

            foreach (Fight fight in m_Client.Player.GetMap().Fights)
            {
                builder = new StringBuilder();

                builder.Append(fight.ID).Append(';').Append(0).Append(';');
                builder.Append("0,0,").Append(fight.Team1.GetAliveFighters().Length).Append(';');
                builder.Append("0,0,").Append(fight.Team2.GetAliveFighters().Length).Append(';');
                builder.Append('|');

                fights.Add(builder.ToString());
            }

            m_Client.Send("fL" + string.Join("|", fights));
        }

        private void FightJoin(string packet)
        {
            if (m_Client.Player.Busy)
                return;

            if (!packet.Contains(";"))
            {
                int fightID;

                if (!int.TryParse(packet, out fightID))
                    return;

                Fight fight = m_Client.Player.GetMap().Fights.First(x => x.ID == fightID);

                if (fight == null)
                    return;

                if (fight.CanJoinSpectator())
                    fight.PlayerJoinSpectator(m_Client.Player);
            }
            else
            {
                string[] data = packet.Split(';');

                int fightID;

                if (!int.TryParse(data[0], out fightID))
                    return;

                Fight fight = m_Client.Player.GetMap().Fights.First(x => x.ID == fightID);

                if (fight == null)
                    return;

                int leaderID;

                if (!int.TryParse(data[1], out leaderID))
                    return;

                FightTeam team = fight.GetTeam(leaderID);

                if (fight.CanJoin(m_Client.Player, team))
                    fight.PlayerJoin(m_Client.Player, team.ID);
                else
                    m_Client.Send("BN");
            }
        }

        #endregion

        #region Fights (toggle)

        private void ToggleFightLock(string packet)
        {
            if (!m_Client.Player.InFight)
                return;

            m_Client.Player.Fight.Toggle(m_Client.Player.Fighter, ToggleType.LOCK);
        }

        private void ToggleFightHelp(string packet)
        {
            if (!m_Client.Player.InFight)
                return;

            m_Client.Player.Fight.Toggle(m_Client.Player.Fighter, ToggleType.HELP);
        }

        private void ToggleFightParty(string packet)
        {
            if (!m_Client.Player.InFight)
                return;

            m_Client.Player.Fight.Toggle(m_Client.Player.Fighter, ToggleType.PARTY);
        }

        private void ToggleFightSpectator(string packet)
        {
            if (!m_Client.Player.InFight)
                return;

            m_Client.Player.Fight.Toggle(m_Client.Player.Fighter, ToggleType.SPECTATOR);
        }

        #endregion

        #region Fights (in)

        private void FightReady(string packet)
        {
            if (!m_Client.Player.InFight)
                return;

            m_Client.Player.Fight.PlayerFightReady(m_Client.Player.Fighter, packet[0] == '0' ? false : true);
        }

        private void FightTurnReady(string packet)
        {
            if (!m_Client.Player.InFight)
                return;

            m_Client.Player.Fight.PlayerTurnReady(m_Client.Player.Fighter);
        }

        private void FightTurnPass(string packet)
        {
            if (!m_Client.Player.InFight)
                return;

            m_Client.Player.Fight.PlayerTurnPass(m_Client.Player.Fighter);
        }

        private void FightLeave(string packet)
        {
            if (!m_Client.Player.InFight & !m_Client.Player.IsSpectator)
                return;

            Fighter fighter = m_Client.Player.Fighter;

            if (packet.Length > 0)
            {
                if (m_Client.Player.Fighter.Fight != FightState.STARTING)
                    return;

                if (m_Client.Player.Fighter != m_Client.Player.Fighter.Team.Leader)
                    return;

                int fighterID;

                if (!int.TryParse(packet, out fighterID))
                    return;

                fighter = m_Client.Player.Fight.GetFighter(fighterID);

                if (fighter.Team != m_Client.Player.Fighter.Team)
                    return;
            }

            if (fighter == null)
                m_Client.Player.Fight.SpectatorLeave(m_Client.Player);
            else
                m_Client.Player.Fight.PlayerLeave(fighter);
        }

        private void FightPlacement(string packet)
        {
            if (!m_Client.Player.InFight)
                return;

            int cell;

            if (!int.TryParse(packet, out cell))
                return;

            m_Client.Player.Fight.PlayerPlace(m_Client.Player.Fighter, cell);
        }

        private void FightLaunchSpell(string datas)
        {
            if (!m_Client.Player.InFight)
                return;

            if (!datas.Contains(';'))
                return;

            string[] data = datas.Split(';');
            int spellID;
            int cell;

            if (!int.TryParse(data[0], out spellID))
                return;

            if (!int.TryParse(data[1], out cell))
                return;

            CharacterSpell spell = m_Client.Player.SpellsInventary.Spells.Find(x => x.ID == spellID);

            m_Client.Player.Fight.LaunchSpell(m_Client.Player.Fighter, spell, cell);
        }

        private void FightUseWeapon(string datas)
        {
            if (!m_Client.Player.InFight)
                return;

            var cell = -1;

            if (!int.TryParse(datas, out cell))
                return;

            var weapon = m_Client.Player.ItemsInventary.GetItemByPos(1);

            if (weapon == null)
            {
                var effect = new EffectItem();
                effect.ID = 100;
                effect.Value = 1;
                effect.Value2 = 5;
                effect.Effect = string.Format("1d{0}+{1}", (effect.Value2 - effect.Value + 1), (effect.Value - 1));

                var model = new ItemModel();
                model.CostAP = 4;
                model.MinPO = 1;
                model.MaxPO = 1;
                model.BonusCC = 0;
                model.TauxCC = 0;
                model.TauxEC = 0;

                model.EffectsList.Add(effect);

                weapon = new CharacterItem(model);
            }

            m_Client.Player.Fight.UseWeapon(m_Client.Player.Fighter, weapon, cell);
        }

        #endregion
    }
}
