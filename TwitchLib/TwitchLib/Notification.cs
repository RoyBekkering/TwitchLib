using Newtonsoft.Json;

namespace TwitchLib
{
    internal class Notification
    {
        [JsonProperty("email")]
        public bool Email { get; set; }

        [JsonProperty("push")]
        public bool Push { get; set; }
    }
}