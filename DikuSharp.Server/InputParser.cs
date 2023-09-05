using System.Linq;

namespace DikuSharp.Server
{
    //Main entry point for the server
    public static class InputParser
    {
        public static void Parse(Connection connection)
        {
            if (!connection.InputBuffer.Any()) { return; }

            var line = connection.InputBuffer.First();
            connection.State.ProcessInputAsync(line);
        }
    }
}
