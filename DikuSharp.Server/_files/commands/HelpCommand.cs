using DikuSharp.Server.Characters;
using DikuSharp.Server.Commands;
using DikuSharp.Server;

public class HelpCommand : IMudCommand
{
    public void Do(PlayerCharacter ch, string[] args)
    {
        if (args.Length > 0)
        {
            var arg = args[0].ToLower();
            foreach(var help in Mud.I.Helps)
            { 
                if (help.Keywords.IndexOf(arg) != -1)
                {
                    ch.SendLine(help.Contents);
                    return;
                }
            }
            ch.SendLine("NOT A VALID HELP FILE...");
        }
        else
        {
            ch.SendLine("NOT A VALID HELP FILE...");
        }
    }
}
