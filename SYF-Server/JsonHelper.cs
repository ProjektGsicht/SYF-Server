﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace SYF_Server
{
    public class JsonHelper
    {
        public static T Deserialize<T>(string JsonText)
        {
            try
            {
                DataContractJsonSerializer Serializer = new DataContractJsonSerializer(typeof(T));

                MemoryStream MemStream = new MemoryStream(Encoding.UTF8.GetBytes(JsonText));
                T myObject = (T)Serializer.ReadObject(MemStream);

                return myObject;
            }
            catch
            {
                return default(T);
            }
        }

        public static string Serialize<T>(T myObject)
        {
            try
            {
                DataContractJsonSerializer Serializer = new DataContractJsonSerializer(typeof(T));

                MemoryStream MemStream = new MemoryStream();
                Serializer.WriteObject(MemStream, myObject);
            
                MemStream.Seek(0, SeekOrigin.Begin);

                StreamReader MemReader = new StreamReader(MemStream);
                string JsonText = MemReader.ReadToEnd();

                return JsonText;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
