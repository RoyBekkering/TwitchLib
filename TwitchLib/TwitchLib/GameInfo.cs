using Newtonsoft.Json;

namespace TwitchLib
{
    internal class GameInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("box")]
        public Thumbnail Box { get; set; }

        [JsonProperty("logo")]
        public Thumbnail Logo { get; set; }

        [JsonProperty("popularity")]
        public int Popularity { get; set; }

        [JsonProperty("_links")]
        public Links Links { get; set; }

        [JsonProperty("giantbomb_id")]
        public int GiantBombId { get; set; }
    }
}