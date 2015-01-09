using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;

namespace SYF_Server.Misc
{
    [DataContractAttribute]
    public class Settings
    {
        private static object syncInstance = new Object();
        private static object syncLoad = new Object();
        private static object syncSave = new Object();

        private static Settings myInstance = null;
        public static Settings GetInstance()
        {
            lock (syncInstance)
            {
                if (myInstance == null)
                {
                    Load();
                }
            }

            return myInstance;
        }

        public static void Load(string Path = "./settings.json")
        {
            lock (syncLoad)
            {
                if (File.Exists(Path))
                {
                    string JsonSettings = File.ReadAllText(Path);
                    myInstance = JsonHelper.Deserialize<Settings>(JsonSettings);
                }
                else
                {
                    myInstance = new Settings();
                    Settings.Save();
                }
            }
        }

        public static void Save(string Path = "./settings.json")
        {
            lock (syncSave)
            {
                string JsonSettings = JsonHelper.Serialize<Settings>(myInstance);
                File.WriteAllText(Path, JsonSettings);
            }
        }

        [DataMemberAttribute]
        public bool DebugEnabled;

        [DataMemberAttribute]
        public bool UseEncryption;
    }
}
