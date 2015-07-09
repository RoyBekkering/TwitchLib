using Newtonsoft.Json;

namespace TwitchLib
{
    internal class Ingest
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("default")]
        public bool Default { get; set; }

        [JsonProperty("_id")]
        public int Id { get; set; }

        [JsonProperty("url_template")]
        public string UrlTemplate { get; set; }

        [JsonProperty("availability")]
        public double Availability { get; set; }
    }
}