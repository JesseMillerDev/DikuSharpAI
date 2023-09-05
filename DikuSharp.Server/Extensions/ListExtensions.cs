using System.Collections.Generic;
using System.Linq;
using DikuSharp.Server.Characters;

namespace DikuSharp.Server.Extensions
{
    public static class ListExtensions
    {
        public static Dictionary<int, PlayerCharacter> GetChoices(this List<PlayerCharacter> characters )
        {
            return characters?.OrderBy( c => c.Level )
                    .Select( ( c, i ) => new KeyValuePair<int, PlayerCharacter>( i, c ) )
                    .ToDictionary( kvp => kvp.Key, kvp => kvp.Value ) ?? new Dictionary<int, PlayerCharacter>();
        }
    }
}
