using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DikuSharp.Server.ConnectionStates
{
    public class LoginState : SessionState
    {
        private static readonly string ConnectedPrompt = Mud.I.Config.SessionStates[typeof(LoginState).Name];
        private readonly string _userName;
        private bool isLoggingIn = false;

        public LoginState(Connection conn, string userName)
            : base(conn)
        {
            _userName = userName;
        }

        public override string BuildPrompt()
        {
            return ConnectedPrompt;
        }

        public override async Task ProcessInputAsync(string command)
        {
            if (command != string.Empty)
            {
                var authenticatedUser = Authenticate(command);
                
                if (!authenticatedUser)
                {
                    Connection.SendLine("Incorrect user name or password.", false);
                    Connection.SetState(new ConnectedState(Connection));
                    await Connection.Write();
                    return;
                }

                isLoggingIn = true;
              
                Connection.SetState(new SelectCharacterState(Connection));
              
                isLoggingIn = false;
            }
        }

        private bool Authenticate(string name)
        {
            //compare passwords
            Encoding encoding = Encoding.ASCII;
            byte[] result = SHA256.HashData(encoding.GetBytes(name));
            //now convert the hash to a string
            string password = string.Join("", result.Select(b => b.ToString("x2")));
            return password.Equals(Connection.Account.Password, StringComparison.Ordinal);
        }
    }
}