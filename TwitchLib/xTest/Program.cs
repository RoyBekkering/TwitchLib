using IRCLib;
using System;
using System.IO;
using System.Linq;

namespace xTest
{
    internal class Program
    {
        private static bool joined;
        private static TwitchIrcClient client;

        private static void Main(string[] args)
        {
            InitClient();

            while(true) {
                string result = Console.ReadLine();

                if(result != null && result.ToLower().Equals("exit")) {
                    break;
                }
                if(result.Length > 0 && client != null) {
                    if(!joined) {
                        client.SendRawMessage("JOIN #roytazz");
                        joined = true;
                    }
                    if(client != null) {
                        client.SendRawMessage(result);
                    }
                }
            }
        }

        private static void InitClient()
        {
            string[] info = GetInfo();
            client = new TwitchIrcClient("irc.twitch.tv", info[0], info[1]);

            client.NetworkError += (s, e) => Console.WriteLine("Error: " + e.SocketError);
            client.RawMessageReceived += (s, e) => {
                Console.WriteLine("<< {0}", e.Message);
            };
            client.RawMessageSent += (s, e) => {
                Console.WriteLine(">> {0}", e.Message);
            };
            client.UserMessageReceived += (s, e) =>
            {
                //Do stuff
            };
            client.ChannelMessageReceived += (s, e) =>
            {
                Console.WriteLine("<{0}> {1}", e.PrivateMessage.User.Nick, e.PrivateMessage.Message);
            };
            client.ChannelTopicReceived += (s, e) =>
            {
                Console.WriteLine("Received topic for channel {0}: {1}", e.Channel.Name, e.Topic);
            };
            client.ConnectAsync();
        }

        private static string[] GetInfo()
        {
            using(StreamReader sr = new StreamReader(Environment.CurrentDirectory + "/Info.txt")) {
                return sr.ReadToEnd().Split(',');
            }
        }
    }
}