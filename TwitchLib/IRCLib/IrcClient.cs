using ChatSharp.Handlers;
using IRCLib.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Timers;

namespace IRCLib
{
    public partial class IrcClient
    {
        #region Properties

        protected const int ReadBufferLength = 1024;

        protected bool IsWriting { get; set; }

        protected Socket Socket { get; set; }

        protected int ServerPort { get; set; }

        protected Timer PingTimer { get; set; }

        protected byte[] ReadBuffer { get; set; }

        protected int ReadBufferIndex { get; set; }

        protected string ServerHostname { get; set; }

        public string ServerNameFromPing { get; set; }

        protected ConcurrentQueue<string> WriteQueue { get; set; }

        protected Dictionary<string, MessageHandler> Handlers { get; set; }

        public IrcUser User { get; set; }

        public Encoding Encoding { get; set; }

        public bool UseSSL { get; private set; }

        public Stream NetworkStream { get; set; }

        public string PrivmsgPrefix { get; set; }

        public ServerInfo ServerInfo { get; set; }

        public bool IgnoreInvalidSSL { get; set; }

        public ClientSettings Settings { get; set; }

        public RequestManager RequestManager { get; set; }

        public ChannelCollection Channels { get; private set; }

        public string ServerAddress
        {
            get
            {
                return ServerHostname + ":" + ServerPort;
            }
            internal set
            {
                string[] parts = value.Split(':');
                if(parts.Length > 2 || parts.Length == 0)
                    throw new FormatException("Server address is not in correct format ('hostname:port')");
                ServerHostname = parts[0];
                if(parts.Length > 1)
                    ServerPort = int.Parse(parts[1]);
                else
                    ServerPort = 6667;
            }
        }

        internal static DateTime DateTimeFromIrcTime(int time)
        {
            return new DateTime(1970, 1, 1).AddSeconds(time);
        }

        #endregion Properties

        public IrcClient(string serverAddress, IrcUser user)
        {
            if(serverAddress == null) throw new ArgumentNullException("serverAddress");
            if(user == null) throw new ArgumentNullException("user");

            User = user;
            ServerAddress = serverAddress;
            Encoding = Encoding.UTF8;
            Channels = new ChannelCollection(this);
            Settings = new ClientSettings();
            Handlers = new Dictionary<string, MessageHandler>();
            MessageHandlers.RegisterDefaultHandlers(this);
            RequestManager = new RequestManager();
            WriteQueue = new ConcurrentQueue<string>();
            PrivmsgPrefix = "";
        }

        public IrcClient(string serverAddress, string username, string password, bool useSSL = false, bool usePingTimer = false)
            : this(serverAddress, new IrcUser(username, username, password))
        {
            UseSSL = useSSL;
        }

        public virtual void ConnectAsync()
        {
            if(Socket != null && Socket.Connected) throw new InvalidOperationException("Socket is already connected to server.");
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ReadBuffer = new byte[ReadBufferLength];
            ReadBufferIndex = 0;
            PingTimer = new Timer(30000);
            PingTimer.Elapsed += (sender, e) =>
            {
                if(!string.IsNullOrEmpty(ServerNameFromPing))
                    SendRawMessage("PING :{0}", ServerNameFromPing);
            };

            var checkQueue = new Timer(1000);
            checkQueue.Elapsed += (sender, e) =>
            {
                if(WriteQueue.Count > 0) {
                    string nextMessage;
                    while(!WriteQueue.TryDequeue(out nextMessage)) { }
                    SendRawMessage(nextMessage);
                }
            };
            checkQueue.Start();
            Socket.BeginConnect(ServerHostname, ServerPort, ConnectComplete, null);
        }

        public void Quit(string reason = null)
        {
            if(reason == null)
                SendRawMessage("QUIT");
            else
                SendRawMessage("QUIT :{0}", reason);
            Socket.BeginDisconnect(false, ar =>
            {
                Socket.EndDisconnect(ar);
                NetworkStream.Dispose();
                NetworkStream = null;
            }, null);
            if(PingTimer != null) {
                PingTimer.Dispose();
            }
        }

        protected virtual void ConnectComplete(IAsyncResult result)
        {
            Socket.EndConnect(result);

            NetworkStream = new NetworkStream(Socket);
            if(UseSSL) {
                NetworkStream = IgnoreInvalidSSL ? new SslStream(NetworkStream, false, (sender, certificate, chain, policyErrors) => true) : new SslStream(NetworkStream);
                ((SslStream)NetworkStream).AuthenticateAsClient(ServerHostname);
            }

            NetworkStream.BeginRead(ReadBuffer, ReadBufferIndex, ReadBuffer.Length, DataReceived, null);
            if(!string.IsNullOrEmpty(User.Password))
                SendRawMessage("PASS {0}", User.Password);
            SendRawMessage("NICK {0}", User.Nick);
            SendRawMessage("USER {0} hostname servername :{1}", User.User, User.RealName);
            PingTimer.Start();
        }

        protected virtual void DataReceived(IAsyncResult result)
        {
            if(NetworkStream == null) {
                OnNetworkError(new SocketErrorEventArgs(SocketError.NotConnected));
                return;
            }

            int length;
            try {
                length = NetworkStream.EndRead(result) + ReadBufferIndex;
            }
            catch(IOException e) {
                var socketException = e.InnerException as SocketException;
                if(socketException != null)
                    OnNetworkError(new SocketErrorEventArgs(socketException.SocketErrorCode));
                else
                    throw;
                return;
            }

            ReadBufferIndex = 0;
            while(length > 0) {
                int messageLength = Array.IndexOf(ReadBuffer, (byte)'\n', 0, length);
                if(messageLength == -1) {
                    ReadBufferIndex = length;
                    break;
                }
                messageLength++;
                var message = Encoding.GetString(ReadBuffer, 0, messageLength - 2); // -2 to remove \r\n
                HandleMessage(message);
                Array.Copy(ReadBuffer, messageLength, ReadBuffer, 0, length - messageLength);
                length -= messageLength;
            }

            NetworkStream.BeginRead(ReadBuffer, ReadBufferIndex, ReadBuffer.Length - ReadBufferIndex, DataReceived, null);
        }

        protected virtual void HandleMessage(string rawMessage)
        {
            OnRawMessageReceived(new RawMessageEventArgs(rawMessage, false));
            var message = new IrcMessage(rawMessage);
            if(Handlers.ContainsKey(message.Command.ToUpper()))
                Handlers[message.Command.ToUpper()](this, message);
        }

        public void SendRawMessage(string message, params object[] format)
        {
            if(NetworkStream == null) {
                OnNetworkError(new SocketErrorEventArgs(SocketError.NotConnected));
                return;
            }

            message = string.Format(message, format);
            var data = Encoding.GetBytes(message + "\r\n");

            if(!IsWriting) {
                IsWriting = true;
                NetworkStream.BeginWrite(data, 0, data.Length, MessageSent, message);
            }
            else {
                WriteQueue.Enqueue(message);
            }
        }

        public void SendIrcMessage(IrcMessage message)
        {
            SendRawMessage(message.RawMessage);
        }

        public void SetHandler(string message, MessageHandler handler)
        {
#if DEBUG
            // This is the default behavior if 3rd parties want to handle certain messages
            // themselves However, if it happens from our own code, we probably did something wrong
            if(Handlers.ContainsKey(message.ToUpper()))
                Console.WriteLine("Warning: {0} handler has been overwritten", message);
#endif
            message = message.ToUpper();
            Handlers[message] = handler;
        }

        #region EventHandlers

        private void MessageSent(IAsyncResult result)
        {
            if(NetworkStream == null) {
                OnNetworkError(new SocketErrorEventArgs(SocketError.NotConnected));
                IsWriting = false;
                return;
            }

            try {
                NetworkStream.EndWrite(result);
            }
            catch(IOException e) {
                var socketException = e.InnerException as SocketException;
                if(socketException != null)
                    OnNetworkError(new SocketErrorEventArgs(socketException.SocketErrorCode));
                else
                    throw;
                return;
            }
            finally {
                IsWriting = false;
            }

            OnRawMessageSent(new RawMessageEventArgs((string)result.AsyncState, true));

            string nextMessage;
            if(WriteQueue.Count > 0) {
                while(!WriteQueue.TryDequeue(out nextMessage)) { }
                SendRawMessage(nextMessage);
            }
        }

        public delegate void MessageHandler(IrcClient client, IrcMessage message);

        public event EventHandler<SocketErrorEventArgs> NetworkError;

        protected virtual void OnNetworkError(SocketErrorEventArgs e)
        {
            if(NetworkError != null) NetworkError(this, e);
        }

        public event EventHandler<RawMessageEventArgs> RawMessageSent;

        protected virtual void OnRawMessageSent(RawMessageEventArgs e)
        {
            if(RawMessageSent != null) RawMessageSent(this, e);
        }

        public event EventHandler<RawMessageEventArgs> RawMessageReceived;

        protected virtual void OnRawMessageReceived(RawMessageEventArgs e)
        {
            if(RawMessageReceived != null) RawMessageReceived(this, e);
        }

        public event EventHandler<IrcNoticeEventArgs> NoticeReceived;

        protected internal virtual void OnNoticeReceived(IrcNoticeEventArgs e)
        {
            if(NoticeReceived != null) NoticeReceived(this, e);
        }

        public event EventHandler<ServerMOTDEventArgs> MOTDPartReceived;

        protected internal virtual void OnMOTDPartReceived(ServerMOTDEventArgs e)
        {
            if(MOTDPartReceived != null) MOTDPartReceived(this, e);
        }

        public event EventHandler<ServerMOTDEventArgs> MOTDReceived;

        protected internal virtual void OnMOTDReceived(ServerMOTDEventArgs e)
        {
            if(MOTDReceived != null) MOTDReceived(this, e);
        }

        public event EventHandler<PrivateMessageEventArgs> PrivateMessageReceived;

        protected internal virtual void OnPrivateMessageReceived(PrivateMessageEventArgs e)
        {
            if(PrivateMessageReceived != null) PrivateMessageReceived(this, e);
        }

        public event EventHandler<PrivateMessageEventArgs> ChannelMessageReceived;

        protected internal virtual void OnChannelMessageReceived(PrivateMessageEventArgs e)
        {
            if(ChannelMessageReceived != null) ChannelMessageReceived(this, e);
        }

        public event EventHandler<PrivateMessageEventArgs> UserMessageReceived;

        protected internal virtual void OnUserMessageReceived(PrivateMessageEventArgs e)
        {
            if(UserMessageReceived != null) UserMessageReceived(this, e);
        }

        public event EventHandler<ErronousNickEventArgs> NickInUse;

        protected internal virtual void OnNickInUse(ErronousNickEventArgs e)
        {
            if(NickInUse != null) NickInUse(this, e);
        }

        public event EventHandler<ModeChangeEventArgs> ModeChanged;

        protected internal virtual void OnModeChanged(ModeChangeEventArgs e)
        {
            if(ModeChanged != null) ModeChanged(this, e);
        }

        public event EventHandler<ChannelUserEventArgs> UserJoinedChannel;

        protected internal virtual void OnUserJoinedChannel(ChannelUserEventArgs e)
        {
            if(UserJoinedChannel != null) UserJoinedChannel(this, e);
        }

        public event EventHandler<ChannelUserEventArgs> UserPartedChannel;

        protected internal virtual void OnUserPartedChannel(ChannelUserEventArgs e)
        {
            if(UserPartedChannel != null) UserPartedChannel(this, e);
        }

        public event EventHandler<ChannelEventArgs> ChannelListReceived;

        protected internal virtual void OnChannelListReceived(ChannelEventArgs e)
        {
            if(ChannelListReceived != null) ChannelListReceived(this, e);
        }

        public event EventHandler<ChannelTopicEventArgs> ChannelTopicReceived;

        protected internal virtual void OnChannelTopicReceived(ChannelTopicEventArgs e)
        {
            if(ChannelTopicReceived != null) ChannelTopicReceived(this, e);
        }

        public event EventHandler<EventArgs> ConnectionComplete;

        protected internal virtual void OnConnectionComplete(EventArgs e)
        {
            if(ConnectionComplete != null) ConnectionComplete(this, e);
        }

        public event EventHandler<SupportsEventArgs> ServerInfoReceived;

        protected internal virtual void OnServerInfoReceived(SupportsEventArgs e)
        {
            if(ServerInfoReceived != null) ServerInfoReceived(this, e);
        }

        public event EventHandler<KickEventArgs> UserKicked;

        protected internal virtual void OnUserKicked(KickEventArgs e)
        {
            if(UserKicked != null) UserKicked(this, e);
        }

        public event EventHandler<WhoIsReceivedEventArgs> WhoIsReceived;

        protected internal virtual void OnWhoIsReceived(WhoIsReceivedEventArgs e)
        {
            if(WhoIsReceived != null) WhoIsReceived(this, e);
        }

        public event EventHandler<NickChangedEventArgs> NickChanged;

        protected internal virtual void OnNickChanged(NickChangedEventArgs e)
        {
            if(NickChanged != null) NickChanged(this, e);
        }

        #endregion EventHandlers
    }
}