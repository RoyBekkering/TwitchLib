using Newtonsoft.Json;
using System.Collections.Generic;

namespace TwitchLib.Collections
{
    internal class IngestCollection
    {
        [JsonProperty("_links")]
        public Links Links { get; set; }

        [JsonProperty("ingests")]
        public List<Ingest> Ingests { get; set; }
    }
}