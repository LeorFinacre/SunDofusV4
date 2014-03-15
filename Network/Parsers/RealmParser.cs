using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunDofus.DataRecords;
using SunDofus.Network.Clients;
using SunDofus.Utilities;

namespace SunDofus.Network.Parsers
{
    class RealmParser
    {
        private RealmClient m_Client;

        public RealmParser(RealmClient client)
        {
            m_Client = client;
        }

        public void ParsePacket(string packet)
        {
            switch ((int)m_Client.State)
            {
                case 0:

                    if (packet.Contains(Config.GetString("LOGIN_VERSION")))
                        m_Client.State = RealmClient.RealmClientState.CHECK_ACCOUNT;
                    else
                    {
                        m_Client.Send(string.Concat("AlEv", Config.GetString("LOGIN_VERSION")));
                        m_Client.Disconnect();
                    }

                    break;

                case 1:

                    if (!packet.Contains("#1"))
                        return;

                    var infos = packet.Split('#');
                    var username = infos[0];
                    var password = infos[1];

                    var requestedAccount = DB_Account.First<DB_Account>(string.Format("username = '{0}'", username));

                    if (requestedAccount != null && Utilities.Basic.Encrypt(requestedAccount.Password, m_Client.Key) == password)
                    {
                        if (requestedAccount.IsBanned == 1 || Servers.BannedIPs.Any(x => m_Client.IP.StartsWith(x.IP)))
                        {
                            m_Client.Send("AlEb");
                            m_Client.Disconnect();
                            return;
                        }

                        m_Client.Account = requestedAccount;

                        Logger.Write(Logger.LoggerType.Debug, "Client [@'{0}'@] authentified !", m_Client.Account.Pseudo);

                        m_Client.SendInformations();
                        m_Client.State = RealmClient.RealmClientState.CHECK_SERVERS;
                    }
                    else
                    {
                        m_Client.Send("AlEx");
                        m_Client.Disconnect();
                    }

                    break;

                case 2:

                    switch (packet.Substring(0, 2))
                    {
                        case "Af":
                        case "AF":

                            m_Client.Send("BN");

                            return;

                        //case "AX":

                        //    m_Client.Send("AxK{0}|{1},{2}", m_Client.Account.SubscriptionTime, Servers.GAMESERVER_ID, m_Client.Account.CharacterCnt);

                        //    break;

                        default:

                            var key = Basic.RandomString(16);

                            while (Servers.GameKeys.Any(x => x.Key == key))
                                key = Basic.RandomString(16);

                            lock (Servers.GameKeys)
                                Servers.GameKeys.Add(new Servers.GameKey(m_Client.Account, key));

                            m_Client.Send("AYK{0}:{1};{2}", Config.GetString("GAME_IP"), Config.GetInt32("GAME_PORT"), key);
                            m_Client.Disconnect();

                            break;
                    }

                    break;
            }
        }
    }
}
