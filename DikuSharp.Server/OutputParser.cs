using DikuSharp.Server.Characters;
using DikuSharp.Server.ConnectionStates;
using System.Linq;

namespace DikuSharp.Server
{
    public static class OutputParser
    {
        public static void Parse( Connection connection )
        {
            if (!connection.InputBuffer.Any()) { return; }
            if (connection.State.GetType() == typeof(PlayingState))
            {
                //here we'll add the prompt
                connection.SendLine(Prompt.ParsePrompt(connection.CurrentCharacter), sendNewLine: false);
            }
        }
    }
}
