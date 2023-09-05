using DikuSharp.Server.Characters;
using DikuSharp.Server.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DikuSharp.Server.ConnectionStates
{
    public class SelectCharacterState : SessionState
    {
        private static readonly string ConnectedPrompt = Mud.I.Config.SessionStates[typeof(SelectCharacterState).Name];

        public SelectCharacterState(Connection conn) : base(conn)
        {
        }

        public override string BuildPrompt()
        {
            var sb = new StringBuilder();
            sb.AppendLine(ConnectedPrompt);
            sb.AppendLine(SendCharacterChoices());
            return sb.ToString();
        }

        public override async Task ProcessInputAsync(string command)
        {
            Dictionary<int, PlayerCharacter> choices = Connection.Account.Characters.GetChoices();
            choices.Add(choices.Count, null);

            var isInt = int.TryParse(command, out int choice);

            if (command == "new")
            {
                Connection.CurrentCharacter = new PlayerCharacter();
                Connection.SetState(new CreateCharacterState(Connection));
                return;
            }
            else if (!isInt || !choices.ContainsKey(choice))
            {
                Connection.SendLine("Invalid choice, try again.", false);
                Connection.SendLine(SendCharacterChoices());
                return;
            }

            Connection.CurrentCharacter = choices[choice];
            Connection.CurrentCharacter.Load(Connection);

            Connection.SetState(new PlayingState(Connection));
            await Connection.State.ProcessInputAsync("look");
        }

        private string SendCharacterChoices()
        {
            var sb = new StringBuilder();
            
            Dictionary<int, PlayerCharacter> choices = Connection.Account.Characters.GetChoices();
            foreach (var choice in choices)
            {
                sb.Append($"[{choice.Key}]: {choice.Value.Name}" + Environment.NewLine);
            }
            sb.Append(Environment.NewLine);
            sb.Append("Choice: ");
            return sb.ToString();
        }
    }
}