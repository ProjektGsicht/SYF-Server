using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SYF_Server
{
    class Server
    {
        private int _Port;
        public int Port
        {
            get { return _Port; }
            set { _Port = value; }
        }

        private bool _IsRunning;
        public bool IsRunning
        {
            get { return _IsRunning; }
        }

        private TcpListener ServerSocket;
        private Thread ListenerThread;
        private List<Client> Clients;
        private Logger ServerLogger;

        public Server(int Port)
        {
            this.Port = Port;

            Clients = new List<Client>();
            ServerSocket = new TcpListener(new IPAddress(0x0), this.Port);
            ServerLogger = new Logger("log_" + Port);
        }

        public bool Start()
        {
            if (_IsRunning)
            {
                Stop();
            }

            try
            {
                ServerLogger.Log("Starting server ...");

                ServerSocket.Start();

                _IsRunning = true;

                ServerLogger.Log("Starting listener queue ...");
                ListenerThread = new Thread(ListenerLoop);
                ListenerThread.Start();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Stop()
        {
            if (_IsRunning)
            {
                _IsRunning = false;

                if (ListenerThread != null)
                {
                    ListenerThread.Abort();
                }

                if (Clients != null)
                {
                    foreach(Client SingleClient in Clients)
                    {
                        SingleClient.Drop();
                    }
                }
            }
        }

        private void ListenerLoop()
        {
            while (IsRunning)
            {
                if (ServerSocket.Pending())
                {
                    ServerLogger.Log("New Client trying to connect ...");

                    TcpClient NewClientSocket = ServerSocket.AcceptTcpClient();
                    Client NewClient = new Client(NewClientSocket, ServerLogger);

                    Clients.Add(NewClient);
                }

                Thread.Sleep(50);
            }
        }
    }
}
