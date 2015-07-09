using Newtonsoft.Json;
using System.Collections.Generic;

namespace TwitchLib.Collections
{
    internal class VideoCollection
    {
        [JsonProperty("_total")]
        public int Total { get; set; }

        [JsonProperty("videos")]
        public List<Video> Videos { get; set; }

        [JsonProperty("_links")]
        public Links Links { get; set; }
    }
}