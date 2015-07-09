using Newtonsoft.Json;

namespace TwitchLib
{
    internal class Game
    {
        [JsonProperty("game")]
        public GameInfo GameInfo { get; set; }

        [JsonProperty("viewers")]
        public int Viewers { get; set; }

        [JsonProperty("channels")]
        public int Channels { get; set; }
    }
}