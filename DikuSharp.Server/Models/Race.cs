﻿using Newtonsoft.Json;

namespace DikuSharp.Server.Models
{
    public class Race
    {
        #region Serializable
        
        [JsonProperty( "name" )]
        public string Name { get; set; }
        
        #endregion
    }
}
