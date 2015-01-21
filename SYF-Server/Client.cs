using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Drawing;
using SYF_Server.Messages;
using SYF_Server.Validation;

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
                    if (message == null)
                    {
                        OnClientDisconnected(EventArgs.Empty);
                    }

                    string logmessage = string.Empty;

                    if (message.Equals("PONG"))
                    {
                        logmessage = "PING Answer";
                    }
                    else
                    {
                        MessageType Type = Message.GetTypeFromJson(message);

                        switch (Type)
                        {
                            case MessageType.NewInfo:
                                {
                                    NewInfoMessage InternalMessage = JsonHelper.Deserialize<NewInfoMessage>(message);
                                    logmessage = String.Format("New info for user {0}.", InternalMessage.WindowsUser);

                                    if (InternalMessage.InternalDataFaceImage.Length == 0)
                                    {
                                        ValidationResponseMessage ResponseMessage = new ValidationResponseMessage();

                                        if (Database.GetInstance().GetUserByName(InternalMessage.WindowsUser) == null)
                                        {
                                            ResponseMessage.Success = false;
                                        }
                                        else
                                        {
                                            ResponseMessage.Success = true;
                                        }
                                    }
                                    else
                                    {
                                        AddNewInfo(InternalMessage);
                                    }

                                    break;
                                }

                            case MessageType.FaceImage:
                                {
                                    ValidationResponseMessage ResponseMessage = new ValidationResponseMessage();
                                    FaceImageMessage InternalMessage = JsonHelper.Deserialize<FaceImageMessage>(message);
                                    logmessage = String.Format("New face validation incoming for user {0}.", InternalMessage.Username);
                                    

                                    ///////////////
                                    NewInfoMessage NewMessage = new NewInfoMessage();
                                    NewMessage.Username = InternalMessage.Username;
                                    NewMessage.Password = "test";
                                    NewMessage.WindowsUser = InternalMessage.Username;
                                    NewMessage.Name = "Testi";
                                    NewMessage.InternalDataFaceImage = new Byte[InternalMessage.InternalData.Length];
                                    Buffer.BlockCopy(InternalMessage.InternalData, 0,
                                        NewMessage.InternalDataFaceImage, 0,
                                        InternalMessage.InternalData.Length);

                                    AddNewInfo(NewMessage);
                                    ///////////////

                                    ResponseMessage.Success = ValidateFace(InternalMessage);

                                    WriteLine(JsonHelper.Serialize<ValidationResponseMessage>(ResponseMessage));

                                    break;
                                }

                            case MessageType.Unknown:
                                logmessage = "Unknown Message received.";
                                break;

                            default:
                                break;
                        }
                    }

                    ServerLogger.Log(String.Format("{0} - {1}", LocalEndpoint.ToString(), logmessage), ConsoleColor.Yellow);
                }
                catch (Exception ex)
                {
                    ServerLogger.Log(String.Format("{0} - {1}", LocalEndpoint.ToString(), ex.Message), ConsoleColor.DarkRed, true);
                }
            }
        }

        private void AddNewInfo(NewInfoMessage message)
        {
            Database.GetInstance().AddInfo(message);
        }

        private bool ValidateFace(FaceImageMessage message)
        {
            FaceImageValidator Validator = new FaceImageValidator(message.Username, message.FaceImage);
            bool FaceRecognized = Validator.Validate();
            return FaceRecognized;
        }

        private void AvaibilityChecker()
        {
            while (true)
            {
                try
                {
                    //WriteLine("PING");
                    //ServerLogger.Log(String.Format("{0} - PING", LocalEndpoint.ToString()), ConsoleColor.DarkMagenta);
                }
                catch (Exception ex)
                {
                    ServerLogger.Log(ex.Message, ConsoleColor.DarkRed, true);
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
