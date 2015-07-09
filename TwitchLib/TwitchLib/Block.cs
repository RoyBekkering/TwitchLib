using Newtonsoft.Json;
using System;

namespace TwitchLib
{
    internal class Block
    {
        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("_links")]
        public Links Links { get; set; }

        [JsonProperty("notifications")]
        public bool Notifications { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }

        [JsonProperty("_id")]
        public int Id { get; set; }
    }
}