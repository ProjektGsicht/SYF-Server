using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SYF_Server.Messages;

namespace SYF_Server.Validation
{
    interface IValidator
    {
        bool Validate();
        ErrorMessage LastErrorMessage();
    }
}
