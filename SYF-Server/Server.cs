using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

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
        private List<Client> Clients;
        private Logger ServerLogger;

        public Server(int Port)
        {
            this.Port = Port;

            Clients = new List<Client>();
            ServerSocket = new TcpListener(IPAddress.Parse("127.0.0.1"), this.Port);
            ServerLogger = new Logger("./log_" + DateTime.Now.ToLongTimeString().Replace(":", "-") + "." + DateTime.Now.Millisecond.ToString() + "_" + this.Port);
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

                ServerSocket.BeginAcceptTcpClient(AddNewClient, ServerSocket);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Stop()
        {
            _IsRunning = false;

            if (Clients != null)
            {
                foreach(Client SingleClient in Clients)
                {
                    SingleClient.Drop();
                }
            }
        }

        private void AddNewClient(IAsyncResult ar)
        {
            ServerLogger.Log("New Client trying to connect ...");

            TcpClient Client = ((TcpListener)ar.AsyncState).EndAcceptTcpClient(ar);

            Client NewClient = new Client(Client, ServerLogger);
            Clients.Add(NewClient);
        }
    }
}
