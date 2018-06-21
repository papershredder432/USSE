using Newtonsoft.Json;
using SDG.Unturned;
using System.IO;
using System.Windows.Forms;

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
            internal set { ConfigData = value; }
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
            {
                try
                {
                    ConfigData = JsonConvert.DeserializeObject<ConfigData>(File.ReadAllText(Location));
                }
                catch
                {
                    MessageBox.Show("ERROR: File is not a valid .json file or it is not valid in terms of it being unturned's Config.json", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
                ConfigData = null;
        }

        public override string ToString() => JsonConvert.SerializeObject(Config, Formatting.Indented);
    }
}
