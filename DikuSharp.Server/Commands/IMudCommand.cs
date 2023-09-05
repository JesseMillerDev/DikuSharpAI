using DikuSharp.Server.Characters;

namespace DikuSharp.Server.Commands
{
    //Main entry point for the server
    public interface IMudCommand
    {
        void Do(PlayerCharacter ch, string[] args);
    }
}
