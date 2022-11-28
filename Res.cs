using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MeowTimerWPF
{
    class Res
    {
        public string img { get; set; }
        public string son { get; set; }
        public string Timer { get; set; }
        public bool color { get; set; }

    }

    class Build
    {

        public static string ConfigPath { get; set; } = "Config.json";
        public static Res Config { get; set; }

        public async Task JsonUpdate(string img, string son, bool color, string Timer)
        {
            var json = string.Empty;
            json = JsonConvert.SerializeObject(UpdateConfig(img, son, color, Timer), Formatting.Indented);
            File.WriteAllText(ConfigPath, json);
        }

        private static Res UpdateConfig(string img, string son, bool color, string Timer) => new Res
        {
            img = img,
            son = son,
            color = color,
            Timer = Timer
        };
        public async Task InitializeAsync()
        {
            var json = string.Empty;

            if (!File.Exists(ConfigPath))
            {
                json = JsonConvert.SerializeObject(GenerateNewConfig(), Formatting.Indented);
                File.WriteAllText(ConfigPath, json, new UTF8Encoding(false));
                await Task.Delay(-1);
            }

            json = File.ReadAllText(ConfigPath, new UTF8Encoding(false));
            Config = JsonConvert.DeserializeObject<Res>(json);
        }

        private static Res GenerateNewConfig() => new Res
        {
            img = @"Resources/big_sleep.png",
            son = @"Resources/Cat_Purring.wav",
            color = false,
            Timer = "00:05:00"
        };
    }
}
