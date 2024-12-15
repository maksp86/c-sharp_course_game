using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace JABEUP_Game.Game.Model
{
    public partial class Save
    {
        [JsonProperty("saveTime", Required = Required.Always)]
        public DateTimeOffset SaveTime { get; set; }

        [JsonProperty("options", Required = Required.Always)]
        public Options Options { get; set; }

        [JsonProperty("highScore", Required = Required.Always)]
        public long HighScore { get; set; }
    }

    public partial class Options
    {
        [JsonProperty("musicVolume", Required = Required.Always)]
        public float MusicVolume { get; set; }

        [JsonProperty("soundVolume", Required = Required.Always)]
        public float SoundVolume { get; set; }

		[JsonProperty("keyBindings", Required = Required.Always)]
		public Dictionary<string, Keys> KeyBindings { get; set; }
    }
}
