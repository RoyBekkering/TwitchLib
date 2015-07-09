using Newtonsoft.Json;
using System.Collections.Generic;

namespace TwitchLib.Collections
{
    internal class GameCollection
    {
        [JsonProperty("_links")]
        public Links Links { get; set; }

        [JsonProperty("_total")]
        public int Total { get; set; }

        [JsonProperty("top")]
        public List<Game> TopGames { get; set; }
    }
}