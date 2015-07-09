using System;

namespace IRCLib.Events
{
    public class ServerMOTDEventArgs : EventArgs
    {
        public string MOTD { get; set; }

        public ServerMOTDEventArgs(string motd)
        {
            MOTD = motd;
        }
    }
}