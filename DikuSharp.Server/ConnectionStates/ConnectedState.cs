using System;
using System.Linq;
using System.Threading.Tasks;

namespace DikuSharp.Server.ConnectionStates
{
    public class ConnectedState : SessionState
    {
        private static readonly string ConnectedPrompt = Mud.I.Config.SessionStates[typeof(ConnectedState).Name];

        public ConnectedState(Connection conn)
            : base(conn)
        {
            
        }

        public override string BuildPrompt()
        {
            return ConnectedPrompt;
        }

        public override async Task ProcessInputAsync(string command)
        {
            switch (command.ToLower())
            {
                case "":
                    break;
                case "new":
                    Connection.SetState(new CreateAccountState(Connection));
                    break;
                case "quit":
                case "close":
                case "exit":
                    Connection.Disconnect();
                    break;
                default:
                    ParseUsername(command);
                    break;
            }
        }

        private void ParseUsername(string command)
        {
            //First line is their user name
            var account = Mud.I.Accounts.FirstOrDefault(a => a.Name.Equals(command, StringComparison.OrdinalIgnoreCase));

            if (account == null)
            {
                Connection.SendLine("Account not found, put in a valid account or type new: >", false);
                return;
            }

            Connection.Account = account;
            Connection.SetState(new LoginState(Connection, account.Name));

        }
    }
}
