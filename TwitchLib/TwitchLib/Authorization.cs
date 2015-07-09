using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TwitchLib
{
    internal class Authorization
    {
        [JsonProperty("scopes")]
        public List<string> Scopes { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}