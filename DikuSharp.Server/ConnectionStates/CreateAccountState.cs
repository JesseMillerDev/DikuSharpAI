using DikuSharp.Server.Characters;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DikuSharp.Server.ConnectionStates
{
    public class CreateAccountState : SessionState
    {
        private readonly string ConnectedPrompt = Mud.I.Config.SessionStates[typeof(CreateAccountState).Name];

        public CreateAccountState(Connection conn)
            : base(conn)
        {
        }

        public override string BuildPrompt()
        {
            return ConnectedPrompt;
        }

        public override async Task ProcessInputAsync(string command)
        {
            //if _account is null create new account;
            var _account = Connection.Account ?? new PlayerAccount();

            if (string.IsNullOrEmpty(_account.Name))
                ParseNewUsername(command);
            else if (string.IsNullOrEmpty(_account.Password))
                ChooseNewPassword(command);
            else
                ConfirmNewPassword(command);
        }

        private void ParseNewUsername(string line)
        {
            //First line is their user name
            var account = Mud.I.Accounts.FirstOrDefault(a => a.Name.Equals(line, StringComparison.OrdinalIgnoreCase));
            if (account == null)
            {
                //Check account name against banned list
                //Allow Account Creation
                account = new PlayerAccount
                {
                    Name = line
                };

                Connection.Account = account;
                Connection.SendLine("Choose a password: ", false);
            }
            else
            {               
                Connection.SendLine("Username unavailable, try again: ", false);
            }
        }

        private void ChooseNewPassword(string line)
        {
            //TODO: Password Validation Here
            //compare passwords
            Encoding encoding = Encoding.ASCII;
            byte[] result = SHA256.HashData(encoding.GetBytes(line));
            //now convert the hash to a string
            string password = string.Join("", result.Select(b => b.ToString("x2")));
            Connection.Account.Password = password;

            Connection.SendLine("Confirm your password: ", false);
        }

        private void ConfirmNewPassword(string line)
        {
            //compare passwords
            Encoding encoding = Encoding.ASCII;
            byte[] result = SHA256.HashData(encoding.GetBytes(line));
            //now convert the hash to a string
            string password = string.Join("", result.Select(b => b.ToString("x2")));

            if (!password.Equals(Connection.Account.Password, StringComparison.Ordinal))
            {
                Connection.Account.Password = null;
                Connection.SendLine("Passwords do not match! Choose a new password: ", false);
            }
            else
            {
                Mud.I.Repo.SaveAccount(Mud.I.Config, Connection.Account);
                Connection.SendLine($"Welcome {Connection.Account.Name}.");
                Connection.SetState(new SelectCharacterState(Connection));
            }
        }
    }
}