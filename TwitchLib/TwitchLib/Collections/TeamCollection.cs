using Newtonsoft.Json;
using System.Collections.Generic;

namespace TwitchLib.Collections
{
    internal class TeamCollection
    {
        [JsonProperty("teams")]
        public List<Team> Teams { get; set; }

        [JsonProperty("_links")]
        public Links Links { get; set; }
    }
}