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
    class NewInfoMessage : Message
    {
        public NewInfoMessage()
        {
            this.Type = MessageType.NewInfo;
        }

        [DataMemberAttribute]
        public MessageType RegisterType;

        [DataMemberAttribute]
        public string Username;

        [DataMemberAttribute]
        public string Password;

        [DataMemberAttribute]
        public string Name;

        public Bitmap FaceImage
        {
            get
            {
                if (InternalDataFaceImage == null) return null;
                using (MemoryStream ms = new MemoryStream(InternalDataFaceImage))
                {
                    return new Bitmap(ms);
                }
            }
            set
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    value.Save(ms, ImageFormat.Bmp);
                    InternalDataFaceImage = ms.ToArray();
                }
            }
        }

        [DataMemberAttribute]
        private byte[] InternalDataFaceImage;

        public Bitmap Fingerprint
        {
            get
            {
                if (InternalDataFingerprint == null) return null;
                using (MemoryStream ms = new MemoryStream(InternalDataFingerprint))
                {
                    return new Bitmap(ms);
                }
            }
            set
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    value.Save(ms, ImageFormat.Bmp);
                    InternalDataFingerprint = ms.ToArray();
                }
            }
        }

        [DataMemberAttribute]
        private byte[] InternalDataFingerprint;
    }
}
