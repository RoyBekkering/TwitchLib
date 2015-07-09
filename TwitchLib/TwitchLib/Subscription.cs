using Newtonsoft.Json;
using System;

namespace TwitchLib
{
    internal class Subscription
    {
        [JsonProperty("_id")]
        public int Id { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("_links")]
        public Links Links { get; set; }
    }
}