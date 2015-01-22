using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using SYF_Server.Messages;
using MySql.Data.MySqlClient;
using SYF_Server.Validation;

namespace SYF_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Misc.Settings.Load();
            Misc.Settings.Save();
            
            /*Misc.Settings.GetInstance().MySqlHost = "hakase";
            Misc.Settings.GetInstance().MySqlUser = "syf";
            Misc.Settings.GetInstance().MySqlPassword = "syf";
            Misc.Settings.GetInstance().MySqlDatabase = "syf";*/

            Database myDb = Database.GetInstance();

            /*
            FaceImageMessage msg = new FaceImageMessage();
            msg.Username = "Test";
            msg.FaceImage = new Bitmap(200, 200);

            string Json = JsonHelper.Serialize<FaceImageMessage>(msg);

            FaceImageMessage newmsg = JsonHelper.Deserialize<FaceImageMessage>(Json);

            Database myDb = Database.GetInstance();
            Datamaps.SqlUser ich = myDb.GetUserByName("rieglmax");

            bool correct = new PasswordValidator(ich, "test").Validate();
            */

            Server srv = new Server(12345);
            srv.Start();

            Console.Read();
        }
    }
}
