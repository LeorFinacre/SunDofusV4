using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SilverSock;

namespace SunDofus.Auth.Network.Sync
{
    class SyncServer : Master.TCPServer
    {
        public List<SyncClient> Clients { get; set; }

        public SyncServer()
            : base(Utilities.Config.GetString("SYNC_IP"), Utilities.Config.GetInt32("SYNC_PORT"))
        {
            Clients = new List<SyncClient>();

            this.SocketClientAccepted += new AcceptSocketHandler(this.OnAcceptedClient);
            this.ListeningServer += new ListeningServerHandler(this.OnListeningServer);
            this.ListeningServerFailed += new ListeningServerFailedHandler(this.OnListeningFailedServer);
        }

        private void OnAcceptedClient(SilverSocket socket)
        {
            if (socket == null) 
                return;

            
            Utilities.Logger.Write(Utilities.Logger.LoggerType.Debug, "New imputed sync connection <{0}> !", socket.IP);

            lock (Clients)
                Clients.Add(new SyncClient(socket));
        }

        private void OnListeningServer(string remote)
        {
            Utilities.Logger.Write(Utilities.Logger.LoggerType.Debug, "SyncServer starded on <{0}> !", remote);
        }

        private void OnListeningFailedServer(Exception exception)
        {
            Utilities.Logger.Write(Utilities.Logger.LoggerType.Errors, "SyncServer can't start : {0} !", exception.ToString());
        }
    }
}
