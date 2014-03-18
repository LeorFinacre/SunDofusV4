using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SilverSock;
using SunDofus.DataRecords;
using SunDofus.Network.Parsers;

namespace SunDofus.Network.Clients
{
    class GameClient : TCPClient
    {

        public bool Authentified { get; set; }

        public DB_Character Player { get; set; }
        public List<DB_AccountFriend> Friends { get; set; }
        public List<DB_AccountEnemy> Enemies { get; set; }

        public DB_Account Account { get; set; }

        private object m_Locker;
        private GameParser m_Parser;

        public GameClient(SilverSocket s)
            : base(s)
        {
            m_Locker = new object();
            Authentified = false;

            this.DisconnectedSocket += new DisconnectedSocketHandler(this.Disconnected);
            this.ReceivedDatas += new ReceiveDatasHandler(this.ReceivedPackets);

            Friends = new List<DB_AccountFriend>();
            Enemies = new List<DB_AccountEnemy>();

            m_Parser = new GameParser(this);

            Send("HG");
        }

        public void Send(string message)
        {
            lock (m_Locker)
                this.SendBytes(message);

            Utilities.Logger.Write(Utilities.Logger.LoggerType.Debug, "Sent to <{0}> : {1}", IP, message);
        }

        public void SendGifts()
        {
            foreach(var gift in DB_Gift.Find<DB_Gift>(string.Concat("target = ", Account.ID)))
            {
                var item = new SunDofus.World.Game.Characters.Items.CharacterItem(Entities.Requests.ItemsRequests.ItemsList.First(x => x.ID == gift.ItemID));

                item.GeneratItem();

                gift.Item = item;

                this.Send(string.Format("Ag1|{0}|{1}|{2}|{3}|{4}~{5}~{6}~~{7};", gift.ID, gift.Title, gift.Message, (gift.Image != "" ? gift.Image : "http://s2.e-monsite.com/2009/12/26/04/167wpr7.png"),
                   Utilities.Basic.DeciToHex(item.Model.ID), Utilities.Basic.DeciToHex(item.Model.ID), Utilities.Basic.DeciToHex(item.Quantity), item.EffectsInfos()));
            }
        }

        public void SendConsoleMessage(string message, int color = 1)
        {
            Send(string.Concat("BAT", color, message));
        }

        public void SendMessage(string message)
        {
            Send(string.Format("cs<font color=\"#00A611\">{0}</font>", message));
        }

        private void ReceivedPackets(string datas)
        {
            Utilities.Logger.Write(Utilities.Logger.LoggerType.Debug, "Receive datas from @<{0}>@ : {1}", IP, datas);

            lock (m_Locker)
                m_Parser.Parse(datas);
        }

        private void Disconnected()
        {
            Utilities.Logger.Write(Utilities.Logger.LoggerType.Debug, "New closed client @<{0}>@ connection !", IP);

            if (Authentified == true)
            {
                Account.IsConnected = 0;
                Account.Update();

                if (Player != null)
                {
                    Player.Map.DelPlayer(Player);
                    Player.IsConnected = false;
                    Player.SetClient(null);

                    if (Player.OnExchange)
                        SunDofus.Game.Exchanges.ExchangesManager.LeaveExchange(Player);

                    if (Player.OnWaitingGuild)
                    {
                        if (Player.ReceiverInviteGuild != -1 || Player.SenderInviteGuild != -1)
                        {
                            if (Servers.Characters.Any(x => x.ID == (Player.ReceiverInviteGuild != -1 ? Player.ReceiverInviteGuild : Player.SenderInviteGuild)))
                            {

                                var character = Servers.Characters.First(x => x.ID == (Player.ReceiverInviteGuild != -1 ? Player.ReceiverInviteGuild : Player.SenderInviteGuild));
                                if (character.IsConnected)
                                {
                                    character.SenderInviteGuild = -1;
                                    character.ReceiverInviteGuild = -1;
                                    character.OnWaitingGuild = false;
                                    character.Send("gJEc");
                                }

                                Player.ReceiverInviteGuild = -1;
                                Player.SenderInviteGuild = -1;
                                Player.OnWaitingGuild = false;
                            }
                        }
                    }

                    if (Player.OnWaitingParty)
                    {
                        if (Player.ReceiverInviteParty != -1 || Player.SenderInviteParty != -1)
                        {
                            if (Servers.Characters.Any(x => x.ID == (Player.ReceiverInviteParty != -1 ? Player.ReceiverInviteParty : Player.SenderInviteParty)))
                            {

                                var character = Servers.Characters.First(x => x.ID == (Player.ReceiverInviteParty != -1 ? Player.ReceiverInviteParty : Player.SenderInviteParty));
                                if (character.IsConnected)
                                {
                                    character.SenderInviteParty = -1;
                                    character.ReceiverInviteParty = -1;
                                    character.OnWaitingParty = false;
                                    character.Send("PR");
                                }

                                Player.ReceiverInviteParty = -1;
                                Player.SenderInviteParty = -1;
                                Player.OnWaitingParty = false;
                            }
                        }
                    }

                    if (Player.Party != null)
                        Player.Party.LeaveParty(Player.Name);

                    if (Player.IsFollowing)
                    {
                        if (Servers.Characters.Any(x => x.Followers.Contains(Player) && x.ID == Player.FollowingID))
                            Servers.Characters.First(x => x.ID == Player.FollowingID).Followers.Remove(Player);
                    }

                    if (Player.IsFollow)
                    {
                        Player.Followers.Clear();
                        Player.IsFollow = false;
                    }

                    if (Player.IsChallengeAsked)
                    {
                        if (Servers.Characters.Any(x => x.ChallengeAsked == Player.ID))
                        {
                            var character = Servers.Characters.First(x => x.ChallengeAsked == Player.ID);

                            Player.ChallengeAsker = -1;
                            Player.IsChallengeAsked = false;

                            character.ChallengeAsked = -1;
                            character.IsChallengeAsker = false;

                            character.Send(string.Format("GA;902;{0};{1}", character.ID, Player.ID));
                        }
                    }

                    if (Player.IsChallengeAsker)
                    {
                        if (Servers.Characters.Any(x => x.ChallengeAsker == Player.ID))
                        {
                            var character = Servers.Characters.First(x => x.ChallengeAsker == Player.ID);

                            Player.ChallengeAsked = -1;
                            Player.IsChallengeAsker = false;

                            character.ChallengeAsker = -1;
                            character.IsChallengeAsked = false;

                            character.Send(string.Format("GA;902;{0};{1}", character.ID, Player.ID));
                        }
                    }
                }
            }

            lock (Servers.GameServer.Clients)
                Servers.GameServer.Clients.Remove(this);
        }

        #region Friends

        public bool WillNotifyWhenConnected { get; set; }

        public void SendFriends()
        {
            var packet = "FL|";

            foreach (var friend in Friends)
            {
                if (Servers.GameServer.Clients.Any(x => (x as GameClient).Account.ID == friend.AccountID && (x as GameClient).Player != null))
                {
                    packet = string.Concat(packet, friend.TargetPseudo);

                    var charact = (Servers.GameServer.Clients.First(x => (x as GameClient).Account.ID == friend.AccountID && (x as GameClient).Player != null) as GameClient).Player;
                    var client = (Servers.GameServer.Clients.First(x => (x as GameClient).Account.ID == friend.AccountID && (x as GameClient).Player != null) as GameClient);

                    bool seeLevel = (client.Friends.Any(x => x.TargetID == Account.ID) ? true : false);

                    packet = string.Format("{0};?;{1};{2};{3};{4};{5};{6}|", packet, charact.Name, (seeLevel ? charact.Level.ToString() : "?"), (seeLevel ? charact.Faction.FactionID.ToString() : "-1"),
                        charact.Class.ToString(), charact.Sex.ToString(), charact.Skin.ToString());
                }
                else
                    packet = string.Concat(packet, friend.TargetPseudo, "|");
            }

            Send(packet.Substring(0, packet.Length - 1));
        }

        public void AddFriend(string datas)
        {
            if (Servers.Characters.Any(x => x.Name == datas))
            {
                var character = Servers.Characters.First(x => x.Name == datas);

                if (Servers.GameServer.Clients.Any(x => (x as GameClient).Account.ID == character.ID))
                {
                    var client = Servers.GameServer.Clients.First(x => (x as GameClient).Account.ID == character.ID) as GameClient;

                    if (!Friends.Any(x => x.TargetID == client.Account.ID))
                    {
                        var friend = new DB_AccountFriend()
                        {
                            ID = Servers.Friends.Count > 0 ? Servers.Friends.OrderBy(x => x.ID).ToArray()[0].ID + 1 : 1,
                            AccountID = client.Account.ID,
                            TargetID = Account.ID,
                            TargetPseudo = Account.Pseudo
                        };

                        friend.Insert();

                        lock (Friends)
                            Friends.Add(friend);

                        bool seeLevel = (client.Friends.Any(x => x.TargetID == Account.ID) ? true : false);

                        var packet = string.Format("{0};?;{1};{2};{3};{4};{5};{6}|", Account.Pseudo, Player.Name, (seeLevel ? Player.Level.ToString() : "?"), (seeLevel ? Player.Faction.FactionID.ToString() : "-1"),
                            Player.Class, Player.Sex, Player.Skin);

                        Send(string.Concat("FAK", packet));
                    }
                    else
                        Send("FAEa");
                }
                else
                    Send("FAEf");
            }
            else
                Send("FAEf");
        }

        public void RemoveFriend(string datas)
        {
            var name = datas.Substring(1);

            if (datas.Substring(0, 1) == "*")
            {
                if (Friends.Any(x => x.TargetPseudo == name))
                {
                    var friend = Friends.First(x => x.TargetPseudo == name);

                    lock (Friends)
                        Friends.Remove(friend);

                    friend.Delete();

                    Send("FDK");
                }
                else
                    Send("FDEf");
            }
            else if (datas.Substring(0, 1) == "%")
            {
                if(Servers.Characters.Any(x => x.Name == name))
                {
                    var character = Servers.Characters.First(x => x.Name == name);

                    if (Friends.Any(x => x.AccountID == character.AccountID))
                    {
                        var friend = Friends.First(x => x.AccountID == character.AccountID);

                        lock (Friends)
                            Friends.Remove(friend);

                        friend.Delete();

                        Send("FDK");
                    }
                    else
                        Send("FDEf");
                }
                else
                    Send("FDEf");
            }
        }

        #endregion

        #region Enemies

        public void SendEnemies()
        {
            var packet = "iL|";

            foreach (var enemy in Enemies)
            {
                if (Servers.GameServer.Clients.Any(x => (x as GameClient).Account.ID == enemy.AccountID && (x as GameClient).Player != null))
                {
                    packet = string.Concat(packet, enemy.TargetPseudo);

                    var charact = (Servers.GameServer.Clients.First(x => (x as GameClient).Account.ID == enemy.AccountID && (x as GameClient).Player != null) as GameClient).Player;
                    var client = (Servers.GameServer.Clients.First(x => (x as GameClient).Account.ID == enemy.AccountID && (x as GameClient).Player != null) as GameClient);

                    bool seeLevel = (client.Friends.Any(x => x.TargetID == Account.ID) ? true : false);

                    packet = string.Format("{0};?;{1};{2};{3};{4};{5};{6}|", packet, charact.Name, (seeLevel ? charact.Level.ToString() : "?"), (seeLevel ? charact.Faction.FactionID.ToString() : "-1"),
                        charact.Class.ToString(), charact.Sex.ToString(), charact.Skin.ToString());
                }
                else
                    packet = string.Concat(packet, enemy.TargetPseudo, "|");
            }

            Send(packet.Substring(0, packet.Length - 1));
        }

        public void AddEnemy(string datas)
        {
            if (Servers.Characters.Any(x => x.Name == datas))
            {
                var character = Servers.Characters.First(x => x.Name == datas);

                if (Servers.GameServer.Clients.Any(x => (x as GameClient).Account.ID == character.ID))
                {
                    var client = Servers.GameServer.Clients.First(x => (x as GameClient).Account.ID == character.ID) as GameClient;

                    if (!Enemies.Any(x => x.TargetID == client.Account.ID))
                    {
                        var enemy = new DB_AccountEnemy()
                        {
                            ID = Servers.Enemies.Count > 0 ? Servers.Enemies.OrderBy(x => x.ID).ToArray()[0].ID + 1 : 1,
                            AccountID = client.Account.ID,
                            TargetID = Account.ID,
                            TargetPseudo = Account.Pseudo
                        };

                        enemy.Insert();

                        lock (Enemies)
                            Enemies.Add(enemy);

                        bool seeLevel = (client.Friends.Any(x => x.TargetID == Account.ID) ? true : false);

                        var packet = string.Format("{0};2;{1};36;10;0;100.FL.", Account.Pseudo, Player.Name, (seeLevel ? Player.Level.ToString() : "?"), (seeLevel ? Player.Faction.FactionID.ToString() : "-1"),
                            Player.Class, Player.Sex, Player.Skin);

                        Send(string.Concat("iAK", packet));
                    }
                    else
                        Send("iAEA");
                }
                else
                    Send("FAEf");
            }
            else
                Send("FAEf");
        }

        public void RemoveEnemy(string datas)
        {
            var name = datas.Substring(1);

            if (datas.Substring(0, 1) == "*")
            {
                if (Enemies.Any(x => x.TargetPseudo == name))
                {
                    var enemy = Enemies.First(x => x.TargetPseudo == name);

                    lock (Enemies)
                        Enemies.Remove(enemy);

                    enemy.Delete();

                    Send("iDK");
                }
                else
                    Send("FDEf");
            }
            else if (datas.Substring(0, 1) == "%")
            {
                if (Servers.Characters.Any(x => x.Name == name))
                {
                    var character = Servers.Characters.First(x => x.Name == name);

                    if (Enemies.Any(x => x.AccountID == character.AccountID))
                    {
                        var enemy = Enemies.First(x => x.AccountID == character.AccountID);

                        lock (Enemies)
                            Enemies.Remove(enemy);

                        enemy.Delete();

                        Send("iDK");
                    }
                    else
                        Send("FDEf");
                }
                else
                    Send("FDEf");
            }
        }

        #endregion
    }
}
