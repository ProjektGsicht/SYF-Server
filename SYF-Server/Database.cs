using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using SYF_Server.Misc;
using MySql.Data.MySqlClient;
using SYF_Server.Datamaps;
using SYF_Server.Messages;

namespace SYF_Server
{
    class Database
    {
        private string ConnectionString;
        private MySqlConnection Connection;

        private static object syncInstance = new Object();

        private static Database myInstance = null;
        public static Database GetInstance()
        {
            lock (syncInstance)
            {
                if (myInstance == null)
                {
                    myInstance = new Database();
                }
            }

            return myInstance;
        }

        public Database()
        {
            if (String.IsNullOrEmpty(Settings.GetInstance().MySqlHost))
            {
                throw new Exception("No database specified in settings.");
            }

            ConnectionString = String.Format("SERVER={0};DATABASE={1};UID={2};PASSWORD={3};",
                Settings.GetInstance().MySqlHost,
                Settings.GetInstance().MySqlDatabase,
                Settings.GetInstance().MySqlUser,
                Settings.GetInstance().MySqlPassword);

            Connection = new MySqlConnection(ConnectionString);

            Connection.StateChange += OnMySqlStateChanged;

            Connection.Open();
        }

        void OnMySqlStateChanged(object sender, System.Data.StateChangeEventArgs e)
        {
            if (e.CurrentState == System.Data.ConnectionState.Closed ||
                e.CurrentState == System.Data.ConnectionState.Broken)
            {
                Connection.Dispose();
                Connection = new MySqlConnection(ConnectionString);
                Connection.Open();
            }
        }

        public Datamaps.SqlUser GetUserByName(string Username)
        {
            SqlUser RequestedUser = null;

            MySqlCommand command = Connection.CreateCommand();
            command.CommandText = String.Format("SELECT * FROM `{0}` WHERE {1} = ?username;",
                Constants.DB_USERS,
                Constants.DB_USERS_FIELD_USERNAME);

            if (Username.Contains("\\"))
            {
                command.CommandText = String.Format("SELECT * FROM `{0}` WHERE {1} = ?username;",
                    Constants.DB_USERS,
                    Constants.DB_USERS_FIELD_WINDOWSUSER);
            }

            MySqlParameter ParUsername = new MySqlParameter("?username", MySqlDbType.VarChar);
            ParUsername.Value = Username;

            command.Parameters.Add(ParUsername);

            MySqlDataReader Reader;
            Reader = command.ExecuteReader();

            if (Reader.HasRows)
            {
                Reader.Read();

                RequestedUser = new Datamaps.SqlUser(Reader.GetInt32(Constants.DB_USERS_FIELD_ID));
                RequestedUser.Name = Reader.GetString(Constants.DB_USERS_FIELD_NAME);
                RequestedUser.Username = Reader.GetString(Constants.DB_USERS_FIELD_USERNAME);
                RequestedUser.WindowsUser = Reader.GetString(Constants.DB_USERS_FIELD_WINDOWSUSER);
                RequestedUser.Password = Reader.GetString(Constants.DB_USERS_FIELD_PASSWORD);

                command.Dispose();
                command = Connection.CreateCommand();
                command.CommandText = String.Format("SELECT * FROM `{0}` WHERE {1}='{2}';",
                    Constants.DB_FACEIMAGES,
                    Constants.DB_FACEIMAGES_FIELD_USERID,
                    RequestedUser.Id);

                Reader.Close();
                Reader.Dispose();
                Reader = command.ExecuteReader();

                if (Reader.HasRows)
                {
                    while (Reader.Read())
                    {
                        byte[] image;
                        int BlobSize = (int)Reader.GetBytes(2, 0, null, 0, 0);

                        image = new byte[BlobSize];

                        int index = 0;
                        while (index < BlobSize)
                        {
                            int BytesRead = (int)Reader.GetBytes(2, index, image, index, BlobSize-index);
                            index += BlobSize;
                        }

                        if (RequestedUser.FaceImages == null)
                        {
                            RequestedUser.FaceImages = new List<Bitmap>();
                        }

                        MemoryStream MemStream = new MemoryStream(image);
                        RequestedUser.FaceImages.Add(new Bitmap(MemStream));
                    }
                }
            }

            command.Dispose();
            Reader.Close();
            Reader.Dispose();

            return RequestedUser;
        }

        public void AddInfo(NewInfoMessage UserInfo)
        {
            SqlUser User = GetUserByName(UserInfo.Username);

            using (MySqlCommand command = new MySqlCommand())
            {
                command.Connection = Connection;

                if (User == null)
                {
                    command.CommandText = String.Format("INSERT INTO `{0}` ({1}, {2}, {3}, {4}) VALUES (?name, ?username, ?windowsuser, ?password);",
                        Constants.DB_USERS,
                        Constants.DB_USERS_FIELD_NAME,
                        Constants.DB_USERS_FIELD_USERNAME,
                        Constants.DB_USERS_FIELD_WINDOWSUSER,
                        Constants.DB_USERS_FIELD_PASSWORD);

                    MySqlParameter ParUsername = new MySqlParameter("?username", MySqlDbType.VarChar, 30);
                    MySqlParameter ParPassword = new MySqlParameter("?password", MySqlDbType.VarChar, 32);

                    ParUsername.Value = UserInfo.Username;
                    ParPassword.Value = UserInfo.Password;

                    command.Parameters.Add(ParUsername);
                    command.Parameters.Add(ParPassword);
                }
                else
                {
                    command.CommandText = String.Format("UPDATE `{0}` SET {1} = ?name, {2} = ?windowsuser WHERE {3} = ?userid;",
                        Constants.DB_USERS,
                        Constants.DB_USERS_FIELD_NAME,
                        Constants.DB_USERS_FIELD_WINDOWSUSER,
                        Constants.DB_USERS_FIELD_ID);

                    MySqlParameter ParUserId = new MySqlParameter("?userid", MySqlDbType.Int32, 11);

                    ParUserId.Value = User.Id;

                    command.Parameters.Add(ParUserId);
                }


                MySqlParameter ParName = new MySqlParameter("?name", MySqlDbType.VarChar, 30);
                MySqlParameter ParWindowsUser = new MySqlParameter("?windowsuser", MySqlDbType.VarChar, 60);

                ParName.Value = String.IsNullOrEmpty(UserInfo.Name) ? "" : UserInfo.Name;
                ParWindowsUser.Value = String.IsNullOrEmpty(UserInfo.WindowsUser) ? UserInfo.Username : UserInfo.WindowsUser;

                command.Parameters.Add(ParName);
                command.Parameters.Add(ParWindowsUser);

                command.ExecuteNonQuery();
            }

            if (User == null)
            {
                User = GetUserByName(UserInfo.WindowsUser);
            }

            using (MySqlCommand command = new MySqlCommand())
            {
                command.Connection = Connection;

                command.CommandText = String.Format("INSERT INTO `{0}` ({1}, {2}) VALUES (?userid, ?faceimage);",
                    Constants.DB_FACEIMAGES,
                    Constants.DB_FACEIMAGES_FIELD_USERID,
                    Constants.DB_FACEIMAGES_FIELD_FACEIMAGE);

                MySqlParameter ParUserId = new MySqlParameter("?userid", MySqlDbType.Int32, 11);
                MySqlParameter ParFaceimage = new MySqlParameter("?faceimage", MySqlDbType.LongBlob, UserInfo.InternalDataFaceImage.Length);

                ParUserId.Value = User.Id;
                ParFaceimage.Value = UserInfo.InternalDataFaceImage;

                command.Parameters.Add(ParUserId);
                command.Parameters.Add(ParFaceimage);

                command.ExecuteNonQuery();
            }
        }
    }
}
