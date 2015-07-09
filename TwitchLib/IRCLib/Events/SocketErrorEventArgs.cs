using System;
using System.Net.Sockets;

namespace IRCLib.Events
{
    public class SocketErrorEventArgs : EventArgs
    {
        public SocketError SocketError { get; set; }

        public SocketErrorEventArgs(SocketError socketError)
        {
            SocketError = socketError;
        }
    }
}