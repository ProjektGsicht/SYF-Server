using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SYF_Server.Datamaps;

namespace SYF_Server.Validation
{
    class PasswordValidator : IValidator
    {
        private SqlUser _User;
        public SqlUser User
        {
            get
            {
                return _User;
            }
        }

        private string _Password;
        public string Password
        {
            get
            {
                return _Password;
            }
        }

        public PasswordValidator(string User, string Password)
        {
            _User = Database.GetInstance().GetUserByName(User);
            _Password = Password;
        }

        public PasswordValidator(SqlUser User, string Password)
        {
            _User = User;
            _Password = Password;
        }

        public bool Validate()
        {
            if (User.Password.Equals(Password))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Messages.ErrorMessage LastErrorMessage()
        {
            throw new NotImplementedException();
        }
    }
}
