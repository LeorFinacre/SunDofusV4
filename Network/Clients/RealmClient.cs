using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SilverSock;
using SunDofus.DataRecords;
using SunDofus.Network.Parsers;
using SunDofus.Utilities;

namespace SunDofus.Network.Clients
{
    class RealmClient : TCPClient
    {
        public enum RealmClientState
        {
            CHECK_VERSION = 0,
            CHECK_ACCOUNT = 1,
            CHECK_SERVERS = 2
        }

        private object m_Locker;
        private RealmParser m_Parser;

        public string Key { get; private set; }
        public RealmClientState State { get; set; }
        public DB_Account Account { get; set; }

        public RealmClient(SilverSocket s)
            : base(s)
        {
            Key = Basic.RandomString(32);

            m_Locker = new object();
            m_Parser = new RealmParser(this);

            State = RealmClientState.CHECK_VERSION;

            this.DisconnectedSocket += () =>
            {
                Logger.Write(Logger.LoggerType.Debug, "New disconnection realmclient notified!");

                lock (Servers.RealmServer.Clients)
                    Servers.RealmServer.Clients.Remove(this);
            };

            this.ReceivedDatas += (string packet) =>
            {
                Logger.Write(Logger.LoggerType.Debug, "Receive from client [@'{0}'@] : {1}", this.IP, packet);

                lock (m_Locker)
                    m_Parser.ParsePacket(packet);
            };

            Send(string.Concat("HC", Key));
        }

        public void Send(string message)
        {
            Logger.Write(Logger.LoggerType.Debug, "Sent to client [@'{0}'@] : {1}", this.IP, message);

            lock (m_Locker)
                this.SendBytes(message);
        }

        public void Send(string format, params object[] args)
        {
            Send(string.Format(format, args));
        }

        public void SendInformations()
        {
            Send(string.Concat("Ad", Account.Pseudo));
            Send(string.Concat("Ac", Account.Communauty));

            RefreshHosts();

            Send(string.Concat("AlK", Account.GMLevel));
            Send(string.Concat("AQ", Account.Question));
        }

        public void RefreshHosts()
        {
            Send("AH{0};{1};{2};1", Servers.GAMESERVER_ID, (int)Servers.GAMESERVER_STATE, (Servers.GAMESERVER_ID * 75));
        }
    }
}
