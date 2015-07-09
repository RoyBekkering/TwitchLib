using Newtonsoft.Json;
using System.Collections.Generic;

namespace TwitchLib.Collections
{
    internal class ChannelCollection
    {
        [JsonProperty("channels")]
        public List<Channel> Channels { get; set; }

        [JsonProperty("_total")]
        public int Total { get; set; }

        [JsonProperty("_links")]
        public Links Links { get; set; }
    }
}