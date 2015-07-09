﻿using Newtonsoft.Json;
using System;

namespace TwitchLib
{
    internal class Team
    {
        //TODO: Team
        [JsonProperty("_links")]
        public Links Links { get; set; }

        [JsonProperty("_id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("info")]
        public string Info { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("logo")]
        public string Logo { get; set; }

        [JsonProperty("banner")]
        public string Banner { get; set; }

        [JsonProperty("background")]
        public string Background { get; set; }
    }
}