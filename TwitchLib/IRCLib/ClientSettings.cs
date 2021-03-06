namespace IRCLib
{
    public class ClientSettings
    {
        public ClientSettings(bool whoIsOnConnect = false, bool whoIsOnJoin = false, bool modeOnJoin = false,
                bool generateRandomNicknameIfRefused = false, int joinWhoIsDelay = 1)
        {
            WhoIsOnConnect = whoIsOnConnect;
            WhoIsOnJoin = whoIsOnJoin;
            ModeOnJoin = modeOnJoin;
            GenerateRandomNickIfRefused = generateRandomNicknameIfRefused;
            JoinWhoIsDelay = joinWhoIsDelay;
        }

        /// <summary>
        /// If true, the client will WHOIS itself upon joining, which will populate the hostname in
        /// IrcClient.User. This will allow you, for example, to use IrcUser.Match(...) on yourself
        /// to see if you match any masks.
        /// </summary>
        public bool WhoIsOnConnect { get; set; }

        /// <summary>
        /// If true, the client will MODE any channel it joins, populating IrcChannel.Mode. If
        /// false, IrcChannel.Mode will be null until the mode is explicitly requested.
        /// </summary>
        public bool ModeOnJoin { get; set; }

        /// <summary> If true, the library will generate a random nick with alphanumerical
        /// characters if it encounters a NICK error.
        public bool GenerateRandomNickIfRefused { get; set; }

        /// <summary>
        /// If true, the library will WHOIS everyone in a channel upon joining. This procedure can
        /// take several minutes on larger channels.
        /// </summary>
        public bool WhoIsOnJoin { get; set; }

        /// <summary>
        /// The delay, in seconds, between each WHOIS when WhoIsOnJoin is true.
        /// </summary>
        public int JoinWhoIsDelay { get; set; }
    }
}