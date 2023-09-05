using System.Collections.Generic;
using Newtonsoft.Json;
using DikuSharp.Server.Enums;
using System;
using DikuSharp.Server.Characters;

namespace DikuSharp.Server.Commands
{
    public class CommandMetaData
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("fileName")]
        public string FileName { get; set; }
        [JsonProperty("priority")]
        public int Priority { get; set; }
        [JsonProperty("level")]
        public int Level { get; set; }
        [JsonProperty("aliases")]
        public IEnumerable<string> Aliases { get; set; }
        [JsonProperty("commandType")]
        public CommandType CommandType { get; set; }
        [JsonProperty("preserveQuotes")]
        public bool PreserveQuotes { get; set; }

        [JsonIgnore]
        public IMudCommand Command { get; set; }
    }
}
