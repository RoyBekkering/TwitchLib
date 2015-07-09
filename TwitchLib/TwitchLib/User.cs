using Newtonsoft.Json;
using System;

namespace TwitchLib
{
    internal class User
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("_links")]
        public Links Links { get; set; }

        [JsonProperty("logo")]
        public string Logo { get; set; }

        [JsonProperty("_id")]
        public int Id { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("bio")]
        public string Bio { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("partnered")]
        public bool Partnered { get; set; }

        [JsonProperty("notifications")]
        public Notification Notifications { get; set; }
    }
}