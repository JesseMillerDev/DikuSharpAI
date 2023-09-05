using System.Linq;
using DikuSharp.Server.Models;
using Newtonsoft.Json;

namespace DikuSharp.Server.Characters
{
    public class Character : IThing
    {
        #region Serializable

        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("shortDescription")]
        public string ShortDescription { get; set; }
        public int Level { get; set; }
        [JsonProperty("race")]
        public Race Race { get; set; }
        [JsonProperty("class")]
        public Class Class { get; set; }
        [JsonProperty("hitPoints")]
        public int HitPoints { get; set; }
        [JsonProperty( "maxHitPoints" )]
        public int MaxHitPoints { get; set; }
        [JsonProperty( "manaPoints" )]
        public int ManaPoints { get; set; }
        [JsonProperty( "maxManaPoints" )]
        public int MaxManaPoints { get; set; }
        [JsonProperty( "movePoints" )]
        public int MovePoints { get; set; }
        [JsonProperty( "maxMovePoints" )]
        public int MaxMovePoints { get; set; }

        //Stats!
        [JsonProperty("strength")]
        public int Strength { get; set; }
        [JsonProperty("constitution")]
        public int Constitution { get; set; }
        [JsonProperty("dexterity")]
        public int Dexterity { get; set; }
        [JsonProperty("intelligence")]
        public int Intelligence { get; set; }
        [JsonProperty("wisom")]
        public int Wisdom { get; set; }
        [JsonProperty("charisma")]
        public int Charisma { get; set; }
        

        [JsonProperty("currentRoomVnum")]
        public long CurrentRoomVnum
        {
            get { return _currentRoomVnum; }
            set { _currentRoomVnum = value; _currentRoom = Mud.I.AllRooms.First( r => r.Vnum == value ); }
        }
        private long _currentRoomVnum;
        #endregion

        [JsonIgnore]
        public Room CurrentRoom { get { return _currentRoom; } set { _currentRoom = value; _currentRoomVnum = value.Vnum; } }
        private Room _currentRoom;


        #region Aliases
        //These are shortcut properties to access the real ones
        [JsonIgnore]
        public int Hp { get { return HitPoints; } set { HitPoints = value; } }
        [JsonIgnore]
        public int Mp { get { return ManaPoints; } set { ManaPoints = value; } }
        [JsonIgnore]
        public int Mv { get { return MovePoints;} set { MovePoints = value; } }
        [JsonIgnore]
        public int Str { get { return Strength; } set { Strength = value; } }
        [JsonIgnore]
        public int Con { get { return Constitution; } set { Constitution = value; } }
        [JsonIgnore]
        public int Dex { get { return Dexterity; } set { Dexterity = value; } }
        [JsonIgnore]
        public int Int { get { return Intelligence; } set { Intelligence = value; } }
        [JsonIgnore]
        public int Wis { get { return Wisdom; } set { Wisdom = value; } }
        [JsonIgnore]
        public int Cha { get { return Charisma; } set { Charisma = value; } }      

        #endregion



        public void Load(Connection conn)
        {
            //find the room they quit in
            var area = Mud.I.Areas.FirstOrDefault(a => a.Rooms.Exists(r => r.Vnum == conn.CurrentCharacter.CurrentRoomVnum));
            var room = Mud.I.StartingRoom;

            if (area != null)
            {
                room = area.Rooms.FirstOrDefault(r => r.Vnum == conn.CurrentCharacter.CurrentRoomVnum) ?? Mud.I.StartingRoom;
            }

            //assign the room to the player...
            conn.CurrentCharacter.CurrentRoom = room;
            //and the player to the room... (this will be removed when the player quits or moves)
            room.AddCharacter(conn.CurrentCharacter);
        }
    }
}
