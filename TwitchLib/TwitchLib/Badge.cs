using Newtonsoft.Json;

namespace TwitchLib
{
    internal class Badge
    {
        [JsonProperty("alpha")]
        public string Alpha { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonProperty("svg")]
        public string SVG { get; set; }
    }
}