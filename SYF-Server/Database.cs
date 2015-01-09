using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SYF_Server.Misc;
using MySql.Data.MySqlClient;

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
            Datamaps.SqlUser RequestedUser = null;

            MySqlCommand command = Connection.CreateCommand();
            command.CommandText = String.Format("SELECT * FROM {0} WHERE {1}='{2}';",
                Constants.DB_USERS,
                Constants.DB_USERS_FIELD_USERNAME,
                Username);

            MySqlDataReader Reader;
            Reader = command.ExecuteReader();

            if (Reader.HasRows)
            {
                Reader.Read();

                RequestedUser = new Datamaps.SqlUser(Reader.GetInt32(Constants.DB_USERS_FIELD_ID));
                RequestedUser.Name = Reader.GetString(Constants.DB_USERS_FIELD_NAME);
                RequestedUser.Username = Reader.GetString(Constants.DB_USERS_FIELD_USERNAME);
                RequestedUser.Password = Reader.GetString(Constants.DB_USERS_FIELD_PASSWORD);
            }

            return RequestedUser;
        }
    }
}
