using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using SYF_Server.Messages;

namespace SYF_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Misc.Settings.Load();
            Misc.Settings.Save();

            FaceImageMessage msg = new FaceImageMessage();
            msg.Username = "Test";
            msg.FaceImage = new Bitmap(200, 200);

            string Json = JsonHelper.Serialize<FaceImageMessage>(msg);
            FaceImageMessage newmsg = JsonHelper.Deserialize<FaceImageMessage>(Json);

            Database.GetInstance();

            Server srv = new Server(12345);
            srv.Start();

            Console.Read();
        }
    }
}
