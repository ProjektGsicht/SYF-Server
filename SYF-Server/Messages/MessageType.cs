using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SYF_Server.Messages
{
    public enum MessageType
    {
        Unknown = -1,
        Text = 0,
        Error = 1,
        FaceImage = 2,
        Fingerprint = 3,
        NewInfo = 4,
        ValidationResponse = 5
    }
}
