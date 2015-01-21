using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SYF_Server.Misc
{
    class Constants
    {
        public const string DB_USERS                        = "tb1_userdaten";
        public const string DB_USERS_FIELD_ID               = "userid";
        public const string DB_USERS_FIELD_NAME             = "name";
        public const string DB_USERS_FIELD_USERNAME         = "username";
        public const string DB_USERS_FIELD_WINDOWSUSER      = "windowsuser";
        public const string DB_USERS_FIELD_PASSWORD         = "password";

        public const string DB_FACEIMAGES                   = "tb2_verifikation1";
        public const string DB_FACEIMAGES_FIELD_ID          = "ID";
        public const string DB_FACEIMAGES_FIELD_USERID      = "Userid";
        public const string DB_FACEIMAGES_FIELD_FACEIMAGE   = "Upload";
    }
}
