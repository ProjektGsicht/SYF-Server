using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Drawing;

namespace SYF_Server.Messages
{
    [DataContractAttribute]
    public class FingerprintMessage : Message
    {
        [DataMemberAttribute]
        public string Username;

        [DataMemberAttribute]
        public Image Fingerprint;
    }
}
