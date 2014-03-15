using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SilverSock;

namespace SunDofus.Network
{
    class TCPServer : SilverServer
    {
        public List<TCPClient> Clients;

        public TCPServer(string listeningAdress, int listeningPort)
            : base(listeningAdress, listeningPort)
        {
            Clients = new List<TCPClient>();
        }

        public void Start()
        {
            this.WaitConnection();
        }
    }
}
