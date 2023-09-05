using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using DikuSharp.Server.Characters;
using DikuSharp.Server.Commands;
using DikuSharp.Server.Config;
using DikuSharp.Server.Helps;
using DikuSharp.Server.Models;
using DikuSharp.Server.Repositories;
using Newtonsoft.Json;

namespace DikuSharp.Server
{
    //Main entry point for the server
    public sealed class Mud
    {
        #region Singleton stuff

        /// <summary>
        /// The lazy-loaded Mud singleton class
        /// </summary>
        private static readonly Lazy<Mud> Lazy = new Lazy<Mud>( ( ) => new Mud( ) );

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static Mud Instance => Lazy.Value;
        /// <summary>
        /// Gets the instance. Synonym for Instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static Mud I => Instance;

        #endregion

        #region Constructors & Deconstructors
        /// <summary>
        /// Prevents a default instance of the <see cref="Mud"/> class from being created.
        /// </summary>
        private Mud( )
        {
            Connections = new ConcurrentDictionary<Guid,Connection>();
            Repo = new MudRepository();
        }
        #endregion

        #region Properties
        public MudRepository Repo { get; set; }
        public MudConfig Config { get; set; }
        public ConcurrentDictionary<Guid,Connection> Connections { get; private set; }
        public List<CommandMetaData> Commands { get; private set; }
        public List<Area> Areas { get; private set; }
        public List<Room> AllRooms
        {
            get
            {
                if ( _allRooms == null && Areas != null && Areas.All(a=>a.Rooms != null))
                {
                    _allRooms = Areas.SelectMany( a => a.Rooms ).ToList( );
                }
                return _allRooms;
            }
        }
        private List<Room> _allRooms = null;
        public List<PlayerAccount> Accounts { get; private set; }
        public List<Help> Helps { get; private set; }
        public Room StartingRoom { get; private set; }
        public PlayerCharacter[] AllPlayers {  get { return Connections.Select(c => c.Value.CurrentCharacter).ToArray(); } }

        private Task<TcpClient> holderClientTask;
        private Random tickRandom;
        #endregion

        const int PULSE_PER_SECOND = 4;
        const int PULSE_TICK = 30 * PULSE_PER_SECOND;
        const int PULSE_TRACK = 40 * PULSE_PER_SECOND;

        public void StartServer()
        {
            //load the config first...
            Config = _getMudConfigFromFile();


            //get things started
            //load up the areas...
            Console.WriteLine("Loading areas...");
            Areas = Repo.LoadAreas( Config );

            Console.WriteLine("Loading accounts...");
            Accounts = Repo.LoadAccounts( Config );

            Console.WriteLine("Loading help files...");
            Helps = Repo.LoadHelps( Config );

            Console.WriteLine("Loading command files...");
            Commands = Repo.LoadCommands( Config );

            //Calculate this just once...
            StartingRoom = Areas.First( a => a.Rooms.Exists( r => r.Vnum == Config.RoomVnumForNewPlayers ) )
                    .Rooms.First( r => r.Vnum == Config.RoomVnumForNewPlayers );

            Console.WriteLine("Server is loaded and ready.");
        }
    
        
        /// <summary>
        /// Adds a connection to the mud
        /// </summary>
        /// <param name="connection">The connection.</param>
        public void AddConnection( Connection connection )
        {
            Connections.TryAdd( connection.ConnectionId, connection );
        }

        public void RemoveConnection( Connection connection )
        {
            Connections.TryRemove(connection.ConnectionId, out Connection value);
        }

        #region Game Loop
        /// <summary>
        /// Official starting point of the game.
        /// </summary>
        /// <param name="listener"></param>
        public async Task StartGame(TcpListener listener)
        {
            await GameLoop(listener);
        }

        private async Task GameLoop(TcpListener listener)
        {
            bool mudRunning = true;

            DateTime lastTime = DateTime.Now;

            tickRandom = new Random();
            
            try
            {
                while (mudRunning)
                {
                    try
                    {
                        //New connections
                        HandleNewClient(listener);

                        //Clean up
                        foreach (var conn in I.Connections.Values)
                        {
                            conn.CleanUp();
                        }

                        //Input
                        foreach (var conn in I.Connections.Values)
                        {
                            await conn.Read();
                            InputParser.Parse(conn);
                        }

                        //Autonomous game stuff
                        Update();

                        //Output
                        foreach (var conn in I.Connections.Values)
                        {
                            OutputParser.Parse(conn);
                            await conn.Write();
                        }

                    }
                    catch ( IOException io )
                    {
                    }
                }

                return;
            }
            catch( Exception ex)
            {
                throw ex;
            }
        }

        private void HandleNewClient(TcpListener listener)
        {
            if ( holderClientTask == null )
            {
                holderClientTask = listener.AcceptTcpClientAsync();
            }
            
            if ( holderClientTask.IsCompleted )
            {
                var connection = new Connection(holderClientTask.Result);
                I.AddConnection(connection);
                //connection.SendWelcomeMessage();

                //listen for a new one
                holderClientTask = listener.AcceptTcpClientAsync();
            }
        }

        /// <summary>
        /// Main method to update mobs, rooms, objs, fighting, etc.
        /// </summary>
        private void Update()
        {
            //foreach( var area in Areas )
            //{
            //    foreach( var room in area.Rooms )
            //    {
            //        foreach( var mob in room.)
            //    }
            //}
        }
        #endregion

        #region Reading from files

        
        public MudConfig _getMudConfigFromFile()
        {
            var rawJson = File.ReadAllText( "mud.json" );
            var mudConfig = JsonConvert.DeserializeObject<MudConfig>( rawJson );
            return mudConfig;
        }

        #endregion
    }
}
