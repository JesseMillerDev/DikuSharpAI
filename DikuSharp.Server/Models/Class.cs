using Newtonsoft.Json;

namespace DikuSharp.Server.Models
{
    public class Class
    {
        #region Serializable
        
        [JsonProperty( "name" )]
        public string Name { get; set; }

        #endregion
    }
}
