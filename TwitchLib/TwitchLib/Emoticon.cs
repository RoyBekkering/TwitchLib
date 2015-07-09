using Newtonsoft.Json;
using System.Collections.Generic;

namespace TwitchLib
{
    internal class Emoticon
    {
        [JsonProperty("regex")]
        public string Regex { get; set; }

        [JsonProperty("images")]
        public List<EmoticonImage> Images { get; set; }
    }
}