using Newtonsoft.Json;

namespace TwitchLib
{
    public class Thumbnail
    {
        [JsonProperty("small")]
        public string Small { get; set; }

        [JsonProperty("medium")]
        public string Medium { get; set; }

        [JsonProperty("large")]
        public string Large { get; set; }

        [JsonProperty("template")]
        public string Template { get; set; }   //TODO '{' and '}' breaks syntax
    }
}