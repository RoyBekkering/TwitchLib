using Newtonsoft.Json;

namespace TwitchLib
{
    internal class Root
    {
        [JsonProperty("token")]
        public Token Token { get; set; }

        [JsonProperty("_links")]
        public Links Links { get; set; }
    }
}