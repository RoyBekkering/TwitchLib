using Newtonsoft.Json;
using System.Collections.Generic;

namespace TwitchLib.Collections
{
    internal class EmoticonCollection
    {
        [JsonProperty("_links")]
        public Links Links { get; set; }

        [JsonProperty("emoticons")]
        public List<Emoticon> Emoticons { get; set; }
    }
}