using Newtonsoft.Json;
using System;

namespace TwitchLib
{
    public class Stream
    {
        [JsonProperty("game")]
        public string Game { get; set; }

        [JsonProperty("viewers")]
        public int Viewers { get; set; }

        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("channel")]
        public Channel Channel { get; set; }

        [JsonProperty("preview")]
        public Thumbnail Thumbnail { get; set; }

        [JsonProperty("_links")]
        public Links Links { get; set; }
    }
}