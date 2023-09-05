using System.Collections.Generic;
using Newtonsoft.Json;

namespace DikuSharp.Server.Characters
{
    public class PlayerAccount
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
        [JsonProperty("characters")]
        public List<PlayerCharacter> Characters { get; set; }
    }
}
