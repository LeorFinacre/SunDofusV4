using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SilverSock;

namespace SunDofus.Network
{
    class TCPClient
    {
        private SilverSocket m_Socket;
        protected bool Connected { get; private set; }

        public string IP
        {
            get
            {
                return m_Socket.IP;
            }
        }

        protected delegate void DisconnectedSocketHandler();
        protected DisconnectedSocketHandler DisconnectedSocket;

        private void OnDisconnectedSocket()
        {
            var evnt = DisconnectedSocket;
            if (evnt != null)
                evnt();
        }

        protected delegate void ReceiveDatasHandler(string message);
        protected ReceiveDatasHandler ReceivedDatas;

        private void OnReceivedDatas(string message)
        {
            var evnt = ReceivedDatas;
            if (evnt != null)
                evnt(message);
        }

        public TCPClient(SilverSocket s)
        {
            m_Socket = s;

            m_Socket.OnConnected += () =>
            {
                Connected = true;
            };

            m_Socket.OnSocketClosedEvent += () =>
            {
                Connected = false;
                OnDisconnectedSocket();
            };

            m_Socket.OnDataArrivalEvent += (bytes) =>
            {
                foreach (var packet in Encoding.UTF8.GetString(bytes).Replace("\x0a", "").Split('\x00').Where(x => x != ""))
                    OnReceivedDatas(packet);
            };
        }

        public void ConnectTo(string ip, int port)
        {
            m_Socket.ConnectTo(ip, port);
        }

        public void Disconnect()
        {
            m_Socket.CloseSocket();
        }

        protected void SendBytes(string message)
        {
            try
            {
                m_Socket.Send(Encoding.UTF8.GetBytes(string.Concat(message, "\x00")));
            }
            catch { }
        }
    }
}
