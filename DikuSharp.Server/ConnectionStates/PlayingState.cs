using DikuSharp.Server.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DikuSharp.Server.ConnectionStates
{
    public class PlayingState : SessionState
    {
        private static readonly string ConnectedPrompt = Mud.I.Config.SessionStates[typeof(PlayingState).Name];
        public PlayingState(Connection conn)
            : base(conn)
        {
        }

        public override string BuildPrompt()
        {
            return ConnectedPrompt;
        }

        public override async Task ProcessInputAsync(string command)
        {
            ParsePlaying(command);
        }
        
        private void ParsePlaying(string line)
        {
            var character = Connection.CurrentCharacter;

            if (string.IsNullOrWhiteSpace(line))
            {
                return;
            }

            //split the line to find a command and all arguments
            var cmdAndArgs = _getCommandAndArgsFromInput(line);
            var cmdName = cmdAndArgs.Item1.ToLower();

            //attempt to find the command
            var command = Mud.I.Commands.FirstOrDefault(c => c.Name.ToLower().StartsWith(cmdName))
                    ?? Mud.I.Commands.FirstOrDefault(c => c.Aliases?.FirstOrDefault(a => a.ToLower().StartsWith(cmdName)) != null);

            if (command == null)
            {
                character.SendLine("Huh?");
            }
            else if (command.Level > character.Level)
            {
                character.SendLine("Huh?");
            }
            else
            {
                try
                {
                    //now parse the args, honoring quotes if necessary
                    var args = _parseArgs(cmdAndArgs.Item2, command.PreserveQuotes, ' ').ToArray();

                    //do it!
                    command.Command.Do(character, args);

                    //TODO
                    //Do something better here - maybe something returned from the command to signal if another will need to be executed?
                    //or another way to force another command to happen
                    //if (command.CommandType == CommandType.Exit)
                    //{
                    //    ParsePlaying(connection, "look");
                    //}
                }
                catch (Exception ex)
                {
                    throw ex;
                }               
            }
        }
        private static Tuple<string, string> _getCommandAndArgsFromInput(string line)
        {
            string cmd;
            string args;

            //We need to handle some special aliases (e.g. 'hello = "say hello")
            if (line[0] == '\'' || line[0] == ']')
            {
                cmd = line[0].ToString();
                args = new string(line.Skip(1).ToArray());
            }
            else
            {
                //now handle space
                var parts = line.Split(' ');
                cmd = parts[0];
                args = string.Join(" ", parts.Skip(1)); //this preserves the args for later processing
            }

            return new Tuple<string, string>(cmd, args);
        }

        private static List<string> _parseArgs(string args, bool preserveQuotes, params char[] delimiters)
        {
            /*
             * Parse '' quotes (i.e. cast 'magic missile' <person>) - used for multi-word arguments
             *
             * This was adapted from Richard Shepherd's version found here:
             * http://stackoverflow.com/questions/554013/regular-expression-to-split-on-spaces-unless-in-quotes
             */
            List<string> results = new();

            //check to see if there's an even number of quotes.
            bool inQuote = false;
            StringBuilder currentToken = new();
            for (int index = 0; index < args.Length; ++index)
            {
                char currentCharacter = args[index];
                if (currentCharacter == '\'' && !preserveQuotes)
                {
                    // When we see a ", we need to decide whether we are
                    // at the start or send of a quoted section...
                    inQuote = !inQuote;
                }
                else if (delimiters.Contains(currentCharacter) && inQuote == false)
                {
                    // We've come to the end of a token, so we find the token,
                    // trim it and add it to the collection of results...
                    string result = currentToken.ToString().Trim();
                    if (result != "")
                        results.Add(result);

                    // We start a new token...
                    currentToken = new StringBuilder();
                }
                else
                {
                    // We've got a 'normal' character, so we add it to
                    // the curent token...
                    currentToken.Append(currentCharacter);
                }
            }

            // We've come to the end of the string, so we add the last token...
            string lastResult = currentToken.ToString().Trim();
            if (lastResult != "")
                results.Add(lastResult);

            return results;
        }

    }
}
