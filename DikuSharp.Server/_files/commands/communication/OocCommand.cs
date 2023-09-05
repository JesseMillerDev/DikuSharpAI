using DikuSharp.Server.Characters;
using DikuSharp.Server.Commands;
using DikuSharp.Server;
using System.Linq;

public class RestartCommand : IMudCommand
{
    public void Do(PlayerCharacter ch, string[] args)
    {
        if (args.Length < 1)
        {
            ch.SendLine("Ooc what?");
        }
        else
        {
            foreach (var player in Mud.I.AllPlayers)
            {
                if (player.Name == ch.Name) 
                { 
                    player.SendLine("{WYou ooc, '{C" + string.Join(' ', args) + "{W'{x"); 
                }
                else 
                { 
                    player.SendLine("{W" + ch.Name + " oocs '{C" + string.Join(' ', args) + "{W'{x"); 
                }
            }
        }
    }
}
