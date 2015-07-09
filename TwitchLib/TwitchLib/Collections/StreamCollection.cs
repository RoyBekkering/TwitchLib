using Newtonsoft.Json;
using System.Collections.Generic;

namespace TwitchLib.Collections
{
    internal class StreamCollection
    {
        [JsonProperty("_total")]
        public int Total { get; set; }

        [JsonProperty("streams")]
        public List<Stream> Streams { get; set; }

        [JsonProperty("_links")]
        public Links Links { get; set; }
    }
}