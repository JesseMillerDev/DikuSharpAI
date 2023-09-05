using DikuSharp.Server.Characters;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DikuSharp.Server.ConnectionStates
{
    public class CreateCharacterState : SessionState
    {
        private readonly string ConnectedPrompt = Mud.I.Config.SessionStates[typeof(CreateCharacterState).Name];

        public CreateCharacterState(Connection conn)
            : base(conn)
        {
        }

        public override string BuildPrompt()
        {
            return ConnectedPrompt;
        }

        public override async Task ProcessInputAsync(string command)
        {
            if (Connection.Account.Characters != null)
            {
                if (Connection.Account.Characters.Select(x => x.Name == command).Any())
                {
                    Connection.SendLine("Character name already in use.", false);
                    return;
                }
            }
            else
            {
                Connection.Account.Characters = new List<PlayerCharacter>();
            }  

            //TODO: Implement an internal state engine for Character creation
            //Choose Name
            //Choose Race
            //Choose Class
            //Etc.
            
            var character = Connection.CurrentCharacter;
            if (string.IsNullOrEmpty(character.Name))
            {
                Connection.CurrentCharacter.Name = command;
                Connection.SendLine("Short Description.", false);
                return;
            }
                
            Connection.CurrentCharacter.ShortDescription = command;
            character.Load(Connection);

            Connection.Account.Characters.Add(Connection.CurrentCharacter);
            Mud.I.Repo.SaveAccount(Mud.I.Config, Connection.Account);
            
            Connection.SetState(new PlayingState(Connection));
        }
    }
}