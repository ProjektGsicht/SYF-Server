using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;

namespace SYF_Server.Messages
{
    [DataContractAttribute]
    public class Message
    {
        [DataMemberAttribute]
        public MessageType Type;

        public static MessageType GetTypeFromJson(string Json)
        {
            if (string.IsNullOrEmpty(Json))
            {
                return MessageType.Unknown;
            }

            if (!Json.StartsWith("{"))
            {
                return MessageType.Unknown;
            }

            Regex Reg = new Regex(".Type..(.),");
            if (Reg.IsMatch(Json))
            {
                MatchCollection Matches = Reg.Matches(Json);
                int size = Matches.Count;
                return (MessageType)int.Parse(Matches[0].Groups[1].Value);
            }
            else
            {
                return MessageType.Unknown;
            }
        }
    }
}
