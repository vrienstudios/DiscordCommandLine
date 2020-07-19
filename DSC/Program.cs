using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TDSBF;
using TDSBF.Data;
using TDSBF.Data.Discord.MessageClasses;

namespace DSC2
{
    class Program
    {
        static String Token = null;

        private static void RewriteTitle(string textToAppend)
        {
            Console.Title = "Totality Sample CS | " + textToAppend;
        }
        static void Main(string[] args)
        {
            RewriteTitle(string.Empty);
            Console.Write("Input your token: ");
            
            Totality totality = new Totality();

            string tkBuffer = string.Empty;
            while (Token == null)
            {
                ConsoleKeyInfo keyinfo = Console.ReadKey(true);
                switch (keyinfo.Key)
                {
                    case ConsoleKey.Enter:
                        Token = tkBuffer;
                        break;
                    default:
                        tkBuffer += keyinfo.KeyChar;
                        break;
                }
            }
            tkBuffer = null;

            totality.Setup(Token); // Setup should always be the first item called.
            Events.OnMessageRecieve += Events_OnMessageRecieve;
            Storage.SysAlert += Storage_SysAlert;

            while (!totality.GetReadyState)
            {
                Thread.Sleep(100);
            }

            Console.WriteLine("Loggined in as: {0}:{1}", totality.GetCurrentUser.username, totality.GetCurrentUser.discriminator);
        serverRelChoice:;
            Console.WriteLine("y: List Servers | n: List Relationships | d: input command");
            string cChoice = Console.ReadLine().ToLower();
            switch(cChoice)
            {
                case "y":
                    listServers();
                    goto serverRelChoice;
                case "n":
                    listRelationships();
                    goto serverRelChoice;
                case "d":
                    inShell();
                    goto serverRelChoice;
            }
        }

        private static void inShell()
        {
            RewriteTitle("Shell");
            Console.WriteLine("Input command: \nShell not finished");
        }

        private static void listServers()
        {
        retryGuildSelection:;
            Console.Clear();
            int i = 0;
            foreach(Guild guild in Storage.ReadyEvent.d.guilds)
            {
                Console.WriteLine("{3}|{0}:{1} Members: {2}", guild.id, guild.name, guild.member_count, i);
                i++;
            }
            int gp = 0;
            Console.Write("\nSelect a guild via number: ");
            string guildSelect = Console.ReadLine();
            if (guildSelect.ToUpper() == "BCK")
                return;
            int.TryParse(guildSelect, out gp);
            if (gp < 0)
                goto retryGuildSelection;

            Guild selectedGuild = Storage.ReadyEvent.d.guilds[gp];
            RewriteTitle(selectedGuild.name);

        retryChannelSelection:;
            int ix = 0;
            foreach (Channel channel in selectedGuild.channels)
            {
                Console.WriteLine("{3}|{0}:{1}| nsfw:{2}", channel.id, channel.name, channel.nsfw, ix);
                ix++;
            }
            int cp = 0;
            Console.Write("\nSelect a channel via number: ");
            string channelSelect = Console.ReadLine();
            if (channelSelect.ToUpper() == "BCK")
                goto retryGuildSelection;
            int.TryParse(channelSelect, out cp);
            if (gp < 0)
                goto retryChannelSelection;

            Channel selectedChannel = selectedGuild.channels[cp];
            RewriteTitle(selectedGuild.name);

        retryCommandInput:;
            Console.WriteLine("Input a Command: ");
            string commandlist = Console.ReadLine();
            string command = commandlist.Substring(0, 3).ToUpper();
            switch (command)
            {
                default:
                    Console.WriteLine("Error, command not recognized, try the hlp command.");
                    goto retryCommandInput;
                case "MSG":
                    selectedChannel.PostMessage(commandlist.Substring(4, commandlist.Length - 4), false);
                    goto retryCommandInput;
                case "HLP":
                    printHelp();
                    goto retryCommandInput;
                case "BCK":
                    goto retryChannelSelection;
            }
        }

        private static void printHelp()
        {
            Console.WriteLine("Hlp - displays this page");
            Console.WriteLine("Msg (your message) - shoots a message to the current channel without parentheses");
            Console.WriteLine("Bck - takes you back one step");
            Console.WriteLine("{N/A}Load - change the amount of messages loaded when entering a channel");
            Console.WriteLine("{N/A}frq - view friend requests {not available}");
            Console.WriteLine("{N/a}SnpOn - Enables a Nitro sniper as long as your in the specific channel");
        }

        private static void listRelationships()
        {

        }

        private static void Events_OnMessageRecieve(TDSBF.Data.Discord.Events.Protected.MessageAlert e)
        {
            //Console.WriteLine("MS: {0}", e.Message.content);
        }

        private static void Storage_SysAlert(TDSBF.Data.Discord.Events.Protected.SystemAlert e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
