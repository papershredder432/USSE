using Newtonsoft.Json;
using SDG.Unturned;
using System.IO;

namespace papershredder.Programs.USSE.Memory
{
    public class ConfigJson
    {
        public ConfigData Config
        {
            get
            {
                if (ConfigData == null)
                    LoadConfig();

                return ConfigData;
            }
        }
        public string Location { get; set; }

        public ConfigJson(string loc)
        {
            Location = loc;
        }

        public void Save() =>
            File.WriteAllText(Location, ToString());

        private ConfigData ConfigData { get; set; }
        private void LoadConfig()
        {
            if (File.Exists(Location) && Directory.Exists(Path.GetDirectoryName(Location)))
                ConfigData = JsonConvert.DeserializeObject<ConfigData>(File.ReadAllText(Location));
            else
                ConfigData = null;
        }

        public override string ToString() => JsonConvert.SerializeObject(Config, Formatting.Indented);
    }
}
