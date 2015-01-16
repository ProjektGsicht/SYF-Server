using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace SYF_Server.Messages
{
    [DataContractAttribute]
    class ValidationResponseMessage : Message
    {
        public ValidationResponseMessage()
        {
            Type = MessageType.ValidationResponse;
        }

        [DataMemberAttribute]
        public bool Success;

        [DataMemberAttribute]
        public string ErrorMessage;
    }
}
