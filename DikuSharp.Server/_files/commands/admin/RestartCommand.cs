using DikuSharp.Server.Characters;
using DikuSharp.Server.Commands;
using DikuSharp.Server;

public class RestartCommand : IMudCommand
{
    public void Do(PlayerCharacter ch, string[] args)
    {
        if (args.Length > 0)
        {
            ch.SendLine("Sorry can't do that.");
        }
        else
        {
            ch.SendLine("Re-loading configuration files...");
            Mud.I.StartServer();
            ch.SendLine("Done!");
        }
    }
}
