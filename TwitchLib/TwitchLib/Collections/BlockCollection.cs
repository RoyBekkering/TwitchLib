using Newtonsoft.Json;
using System.Collections.Generic;

namespace TwitchLib.Collections
{
    internal class BlockCollection
    {
        [JsonProperty("_links")]
        public Links Links { get; set; }

        [JsonProperty("blocks")]
        public List<Block> Blocks { get; set; }
    }
}