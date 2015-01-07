using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace SYF_Server.Messages
{
    public enum ErrorCode
    {

    }

    [DataContractAttribute]
    public class ErrorMessage : TextMessage
    {
        public ErrorMessage()
        {
            this.Type = MessageType.Error;
        }

        [DataMemberAttribute]
        public ErrorCode Code;
    }
}
