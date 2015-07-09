using Newtonsoft.Json;
using System.Collections.Generic;

namespace TwitchLib.Collections
{
    internal class FollowerCollection
    {
        [JsonProperty("_total")]
        public int Total { get; set; }

        [JsonProperty("follows")]
        public List<Follow> Followers { get; set; }

        [JsonProperty("_links")]
        public Links Links { get; set; }
    }
}