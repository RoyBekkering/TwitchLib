using Newtonsoft.Json;

namespace TwitchLib
{
    internal class Token
    {
        [JsonProperty("authorization")]
        public Authorization Authorization { get; set; }

        [JsonProperty("user_name")]
        public string Username { get; set; }

        [JsonProperty("valid")]
        public bool Valid { get; set; }
    }
}