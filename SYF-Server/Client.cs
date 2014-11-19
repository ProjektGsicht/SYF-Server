using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SYF_Server
{
    class Client
    {
        private TcpClient _ClientSocket;
        public TcpClient ClientSocket
        {
            get { return _ClientSocket; }
        }

        private Logger ServerLogger;

        public delegate void ClientDisconnectedHandler(Object sender, EventArgs e);
        public event ClientDisconnectedHandler ClientDisconnected;

        public Client(TcpClient ClientSocket, Logger ServerLogger)
        {
            _ClientSocket = ClientSocket;
            this.ServerLogger = ServerLogger;

            ClientSocket.ReceiveTimeout = 10;
            ClientSocket.SendTimeout = 10;

            ClientDisconnected += new ClientDisconnectedHandler(OnClientDisconnectedHandler);

            ServerLogger.Log(ClientSocket.Client.RemoteEndPoint.ToString() + " connected", ConsoleColor.Green);

            Thread AvaibilityThread = new Thread(AvaibilityChecker);
            AvaibilityThread.Start();
        }

        private void AvaibilityChecker()
        {
            while (ClientSocket.Connected)
            {
                Thread.Sleep(10);
            }

            OnClientDisconnected(null);
        }

        private void OnClientDisconnected(EventArgs e)
        {
            if (ClientDisconnected != null)
                ClientDisconnected(this, e);
        }

        private void OnClientDisconnectedHandler(Object sender, EventArgs e)
        {
            ServerLogger.Log(ClientSocket.Client.RemoteEndPoint.ToString() + " disconnected", ConsoleColor.Red);
        }

        public void Drop()
        {
            if (ClientSocket.Connected)
            {
                ClientSocket.Close();
            }
        }
    }
}
