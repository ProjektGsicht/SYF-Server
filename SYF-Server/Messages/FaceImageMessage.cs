using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace SYF_Server.Messages
{
    [DataContractAttribute]
    public class FaceImageMessage : Message
    {
        public FaceImageMessage()
        {
            this.Type = MessageType.FaceImage;
        }

        [DataMemberAttribute]
        public string Username;

        [DataMemberAttribute]
        public string WindowsUser;

        public Bitmap FaceImage
        {
            get
            {
                if (InternalData == null) return null;
                using (MemoryStream ms = new MemoryStream(InternalData))
                {
                    return new Bitmap(ms);
                }
            }
            set
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    value.Save(ms, ImageFormat.Bmp);
                    InternalData = ms.ToArray();
                }
            }
        }

        [DataMemberAttribute]
        public byte[] InternalData;
    }
}
