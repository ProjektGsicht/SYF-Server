using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SYF_Server.Datamaps
{
    class SqlUser
    {
        public SqlUser(int Id)
        {
            this._Id = Id;
        }

        private int _Id;
        public int Id
        {
            get
            {
                return _Id;
            }
        }

        public string Username;
        public string Name;
        public string Password;
    }
}
