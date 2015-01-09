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
        private static Settings mySettings = null;
        public static Settings GetInstance()
        {
            if (mySettings == null)
            {
                Load();
            }

            return mySettings;
        }

        public static void Load(string Path = "./settings.json")
        {
            if (File.Exists(Path))
            {
                string JsonSettings = File.ReadAllText(Path);
                mySettings = JsonHelper.Deserialize<Settings>(JsonSettings);
            }
            else
            {
                mySettings = new Settings();
                Settings.Save();
            }
        }

        public static void Save(string Path = "./settings.json")
        {
            string JsonSettings = JsonHelper.Serialize<Settings>(mySettings);
            File.WriteAllText(Path, JsonSettings);
        }

        [DataMemberAttribute]
        public bool DebugEnabled;

        [DataMemberAttribute]
        public bool UseEncryption;
    }
}
