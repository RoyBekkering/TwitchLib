using System;

namespace IRCLib.Events
{
    public class SupportsEventArgs : EventArgs
    {
        public ServerInfo ServerInfo { get; set; }

        public SupportsEventArgs(ServerInfo serverInfo)
        {
            ServerInfo = serverInfo;
        }
    }
}