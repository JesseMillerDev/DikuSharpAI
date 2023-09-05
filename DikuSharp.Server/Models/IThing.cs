using Newtonsoft.Json;

namespace DikuSharp.Server.Models
{
    public interface IThing
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
    }
}