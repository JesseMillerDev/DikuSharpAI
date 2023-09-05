using System.Threading.Tasks;

namespace DikuSharp.Server.ConnectionStates
{
    public abstract class SessionState
    {
        public SessionState(Connection conn)
        {
            Connection = conn;
        }

        protected Connection Connection { get; set; }
        public abstract Task ProcessInputAsync(string command);
        public abstract string BuildPrompt();
        public virtual void Begin()
        {
            Connection.SendLine(BuildPrompt());
        }
    }
}
