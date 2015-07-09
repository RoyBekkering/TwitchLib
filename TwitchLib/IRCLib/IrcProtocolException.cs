using System;

namespace IRCLib
{
    public class IrcProtocolException : Exception
    {
        public IrcProtocolException()
        {
        }

        public IrcProtocolException(string message)
            : base(message)
        {
        }
    }
}