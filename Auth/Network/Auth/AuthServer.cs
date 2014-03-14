using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SilverSock;

namespace SunDofus.Auth.Network.Auth
{
    class AuthServer : Master.TCPServer
    {
        public List<AuthClient> Clients { get; set; }

        public AuthServer()
            : base(Utilities.Config.GetString("AUTH_IP"), Utilities.Config.GetInt32("AUTH_PORT"))
        {
            Clients = new List<AuthClient>();

            this.SocketClientAccepted += new AcceptSocketHandler(this.OnAcceptedClient);
            this.ListeningServer += new ListeningServerHandler(this.OnListeningServer);
            this.ListeningServerFailed += new ListeningServerFailedHandler(this.OnListeningFailedServer);
        }

        private void OnAcceptedClient(SilverSocket socket)
        {
            if (socket == null) 
                return;

            Utilities.Logger.Write(Utilities.Logger.LoggerType.Debug, "New inputed client connection <{0}> !", socket.IP);

            lock (Clients)
                Clients.Add(new AuthClient(socket));
        }

        private void OnListeningServer(string remote)
        {
            Utilities.Logger.Write(Utilities.Logger.LoggerType.Infos, "AuthServer starded on <{0}> !", remote);
        }

        private void OnListeningFailedServer(Exception exception)
        {
            Utilities.Logger.Write(Utilities.Logger.LoggerType.Errors, "AuthServer can't start : {0}", exception.ToString());
        }
    }
}
