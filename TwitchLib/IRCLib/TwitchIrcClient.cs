using System;
using System.Net.Sockets;
using System.Timers;

namespace IRCLib
{
    public class TwitchIrcClient : IrcClient
    {
        public TwitchIrcClient(string serverAddress, string username, string password)
            : base(serverAddress, username, password)
        { }

        public override void ConnectAsync()
        {
            if(Socket != null && Socket.Connected)
                throw new InvalidOperationException("Socket is already connected to server.");

            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ReadBuffer = new byte[ReadBufferLength];
            ReadBufferIndex = 0;

            Timer checkQueue = new Timer(1000);
            checkQueue.Elapsed += (sender, e) =>
            {
                if(WriteQueue.Count > 0) {
                    string nextMessage;
                    while(!WriteQueue.TryDequeue(out nextMessage)) { };
                    SendRawMessage(nextMessage);
                }
            };
            checkQueue.Start();
            Socket.BeginConnect(ServerHostname, ServerPort, ConnectComplete, null);
        }

        protected override void ConnectComplete(IAsyncResult result)
        {
            Socket.EndConnect(result);

            NetworkStream = new NetworkStream(Socket);
            NetworkStream.BeginRead(ReadBuffer, ReadBufferIndex, ReadBuffer.Length, DataReceived, null);

            SendRawMessage("PASS oauth:{0}", User.Password);
            SendRawMessage("NICK {0}", User.Nick);
        }
    }
}