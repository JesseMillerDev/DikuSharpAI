using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using CSScripting;
using DikuSharp.Server.Characters;
using DikuSharp.Server.Colors;
using DikuSharp.Server.ConnectionStates;

namespace DikuSharp.Server
{
    /// <summary>
    /// A client connection to the MUD. Handles all the reading/writing of messages
    /// </summary>
    public class Connection
    {
        private readonly NetworkStream _stream;
        private readonly StreamReader _reader;
        private readonly StreamWriter _writer;
        
        //these act like 'buffers'
        private readonly List<string> inputBuffer;
        private readonly List<string> outputBuffer;

        private Task<string> inputTask;
        private Task outputTask;

        public Guid ConnectionId { get; set; }
        public SessionState State { get; set; }
        public PlayerAccount Account { get; set; }
        public bool UseColors { get; set; }

        /// <summary>
        /// Gets or sets the current character. Represents the character the account has chosen
        /// </summary>
        /// <value>
        /// The current character.
        /// </value>
        public PlayerCharacter CurrentCharacter {
            get { return _currentCharacter; }
            set { _currentCharacter = value; _currentCharacter.CurrentConnection = this; }
        }

        public List<string> InputBuffer { get => inputBuffer; }
        public List<string> OutputBuffer { get => outputBuffer; }

        private PlayerCharacter _currentCharacter;

        /// <summary>
        /// Initializes a new instance of the <see cref="Connection"/> class.
        /// </summary>
        /// <param name="client">The client.</param>
        public Connection( TcpClient client )
        {
            _stream = client.GetStream( );
            _reader = new StreamReader( _stream );
            _writer = new StreamWriter( _stream );

            inputBuffer = new List<string>();
            outputBuffer = new List<string>();

            ConnectionId = Guid.NewGuid( );
            
            SetState(new ConnectedState(this));
            
            UseColors = true;//start false

            //starts the input task - will complete when the user has typed something in.
            inputTask = _reader.ReadLineAsync();
        }
        
        /// <summary>
        /// Saves a line to the internal output buffer
        /// </summary>
        /// <param name="message"></param>
        public void SendLine( string message, bool sendNewLine = true )
        {
            var colorMessage = Colorizer.Colorize(message).Trim('\0');
            outputBuffer.Add(colorMessage);
            if (sendNewLine)
            {
                outputBuffer.Add(Environment.NewLine);
            }
        }

        /// <summary>
        /// Saves a formated line to the internal output buffer.
        /// </summary>
        /// <param name="formatMessage"></param>
        /// <param name="arg"></param>
        public void SendLine( string formatMessage, bool sendNewLine, params object[] arg )
        {
            SendLine( string.Format( formatMessage, arg ), sendNewLine);
        }

        /// <summary>
        /// Used in game loop to assign input to buffer
        /// </summary>
        public async Task Read()
        {
            try {
                if (inputTask.IsCompleted)
                {
                    //they've typed something in, so grab it and listen again
                    inputBuffer.Add(inputTask.Result);
                    inputTask = _reader.ReadLineAsync();
                }
                else
                {
                    //null this out so we make sure we never repeat a command
                    inputBuffer.Clear();
                }
            }
            catch(IOException io)
            {
                //if this failed we'll just assume the client needs to be removed
                Console.WriteLine(io.Message);
                Mud.I.RemoveConnection(this);
            }
            catch (Exception) { throw; } //throw everything else
        }

        /// <summary>
        /// Used in game loop to write output buffer to client
        /// </summary>
        public async Task Write()
        {
            var msg = outputBuffer.JoinBy(Environment.NewLine);

            if (string.IsNullOrEmpty(msg))
                return;

            outputTask ??= _writer.WriteLineAsync(msg);

            if (outputTask.IsCompleted)
            {
                await _writer.FlushAsync();

                outputBuffer.Clear();
                outputTask = null;
            }
        }

        public void CleanUp()
        {
            if(!_stream.Socket.Connected)
            {
                _stream.Dispose();
                Mud.I.RemoveConnection(this);
            }
        }

        internal void SetState(SessionState newState)
        {
            State = newState;
            newState?.Begin();
        }

        internal void Disconnect()
        {
            _stream.Socket.Disconnect(true);
        }
    }
}
