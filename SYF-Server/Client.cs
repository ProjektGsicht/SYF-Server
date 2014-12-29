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
        private EndPoint LocalEndpoint;

        private Thread AvaibilityThread;
        private Thread ReaderThread;

        StreamWriter Writer;
        StreamReader Reader;

        public Client(TcpClient ClientSocket, Logger ServerLogger)
        {
            _ClientSocket = ClientSocket;
            this.ServerLogger = ServerLogger;

            LocalEndpoint = ClientSocket.Client.RemoteEndPoint;

            ClientDisconnected += new ClientDisconnectedHandler(OnClientDisconnectedHandler);

            ServerLogger.Log(ClientSocket.Client.RemoteEndPoint.ToString() + " connected", ConsoleColor.Green);

            Writer = new StreamWriter(ClientSocket.GetStream());
            Reader = new StreamReader(ClientSocket.GetStream());

            ReaderThread = new Thread(TReader);
            ReaderThread.Start();

            AvaibilityThread = new Thread(AvaibilityChecker);
            AvaibilityThread.Start();
        }

        public void Write(string message)
        {
            Writer.Write(message);
            Writer.Flush();
        }

        public void WriteLine(string message)
        {
            Writer.WriteLine(message);
            Writer.Flush();
        }

        private void TReader()
        {
            while (ClientSocket.Connected)
            {
                try
                {
                    string message = Reader.ReadLine();

                    ServerLogger.Log(String.Format("{0} - {1}", LocalEndpoint.ToString(), message), ConsoleColor.Yellow);
                }
                catch (Exception ex)
                {
                    //ServerLogger.Log(String.Format("{0} - {1}", LocalEndpoint.ToString(), ex.Message), ConsoleColor.DarkRed);
                }
            }
        }

        private void AvaibilityChecker()
        {
            while (true)
            {
                try
                {
                    WriteLine("PING");
                    ServerLogger.Log(String.Format("{0} - PING", LocalEndpoint.ToString()), ConsoleColor.DarkMagenta);
                }
                catch (Exception ex)
                {
                    //ServerLogger.Log(ex.Message, ConsoleColor.DarkRed);
                    break;
                }

                Thread.Sleep(1000);
            }

            OnClientDisconnected(EventArgs.Empty);
        }

        private void OnClientDisconnected(EventArgs e)
        {
            if (ClientDisconnected != null)
                ClientDisconnected(this, e);
        }

        private void OnClientDisconnectedHandler(Object sender, EventArgs e)
        {
            ServerLogger.Log(LocalEndpoint.ToString() + " disconnected", ConsoleColor.Red);
            ClientSocket.Close();
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
