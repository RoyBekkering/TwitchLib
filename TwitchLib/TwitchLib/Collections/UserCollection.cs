using Newtonsoft.Json;
using System.Collections.Generic;

namespace TwitchLib.Collections
{
    internal class UserCollection
    {
        [JsonProperty("videos")]
        public List<User> Users { get; set; }

        [JsonProperty("_links")]
        public Links Links { get; set; }
    }
}