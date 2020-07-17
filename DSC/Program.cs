using System;
using System.IO;
using WebSocketSharp;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Threading;
using DSC.Data;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace DSC
{
    public enum Commands
    {
        Post = 0,
        Delete = 1,
        Pin = 2,
        Select = 3,
        Back = 4,
        Exit = 5,
    }
    public static class Program
    {
        public static string Token = string.Empty;
        static WebSocket ws;
        static string op1 = @"{""op"": 1, ""d"": 251}"; // ""presence"": { ""game"": { ""name"": ""test"", ""type"": 0 } }
        static string op2 = @"{""op"": 2, ""d"": { ""token"": ""&1"", ""properties"": { ""$os"": ""linux"", ""$browser"": ""etzyy - wrapper"", ""$device"": ""etzyy - wrapper"" }, ""compress"": false, ""large_threshold"": 250, ""status"": ""online"", ""since"": &2, ""afk"": false} }";

        static int flg = 0;
        static int heartbeat = 0;
        static int readycount = 0;
        static int dtop { get; set; }
        static int dleft { get; set; }

        public static string selectedGuildID = string.Empty; // Used for the current guild which is selected.
        public static Channel selectedChannel;
        public static Guild selectedGuild;
        static ReadyEvent ReadyEvent { get; set; }

        static HttpClient client = new HttpClient();
        static HttpRequestMessage req = new HttpRequestMessage();

        static bool debugFlag = false;
        static bool inServer = false;
        static bool inChannel = false;
        public static bool WSConnect()
        {
            bool fn = false;
            try
            {
                ws.Connect();
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine("FATAL! Can not connect! | " + ex.Message);
                return false;
            }
        }
        static bool yN() => Console.ReadLine().ToLower()  == "y";
        static void Exit([Optional] string message)
        {
            if (message != null)
                Console.WriteLine(message);
            Thread.Sleep(20); // provide time to read terminal
            Console.ReadLine();
            Environment.Exit(0);
        }
        public static bool SetUp()
        {
            try
            {
                ws = new WebSocket("wss://gateway.discord.gg/?v=6&encoding=json");
                Console.WriteLine("Setting Events!");
                ws.OnOpen += Ws_OnOpen;
                ws.OnMessage += Ws_OnMessage;
                ws.OnClose += Ws_OnClose;

                Console.WriteLine("Setting Arguments!");
                //dtop = Console.CursorTop;
                //dleft = Console.CursorLeft;

                Token = File.ReadAllLines(Environment.CurrentDirectory + @"\tk.txt")[0];
                DateTime dt70 = new DateTime(1970, 1, 1);
                TimeSpan ts70 = DateTime.Now - dt70;
                op2 = op2.Replace("&1", Token).Replace("&2", "1577777392396");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Token);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return false;
            }
        }
        static void Main(string[] args)
        {
            Thread Baro = new Thread(() => SetUp());
            Baro.Start();
            //if (!SetUp())
            //    Exit();
            Console.WriteLine("Connecting!");
            int timeout = 0;
            Thread.Sleep(1000);
            while (!WSConnect() && timeout < 5)
            {
                timeout++;
            }
            if (timeout == 5)
                Exit("Failed to connect after 5 tries...");
            while (flg != 2)
                Thread.Sleep(30);
            Console.WriteLine(string.Format("Logged in as: {0} \nContinue? y/N", StaticData.CurrentUser.username));
            if (!yN())
                Exit("Replied 'no' to continuation message.");

            Console.WriteLine("D for Debug or N for Normal?\n");
            if ((Console.ReadLine().ToLower() == "d") == true)
                Debug();
            else
            {
                //Thread thr = new Thread(Run);
                //thr.Start();
                Run();
            }
            Console.ReadLine();
            ws.Close();
        }

        static char sel = 'y';
        static bool onFriend = true;
        static void Run()
        {
            while (true)
            {
            A:
                Console.WriteLine("y: List Servers; n: List Relationships");
                sel = Console.ReadLine().ToLower()[0];
                while(sel == 'n')
                {

                        List<User2> userRelationships = new List<User2>();
                        for (int i = 0; i < ReadyEvent.d.relationships.Count; i++)
                        {
                            Console.WriteLine(String.Format("{0}-{1}:{2}", i, ReadyEvent.d.relationships[i].user.id + " " + ReadyEvent.d.relationships[i].user.username, ReadyEvent.d.relationships[i].user.discriminator));
                            userRelationships.Add(ReadyEvent.d.relationships[i].user);
                        }
                        Console.WriteLine("Select a person to DM");
                        string content = Console.ReadLine();
                    if (content.ToUpper() == "BACK")
                        goto A;
                        Recipient rec = new Recipient();
                        rec.integrateUser(userRelationships[int.Parse(content)]);
                        Channel channel = new Channel();
                    channel.id = ReadyEvent.d.private_channels.First(o => (o.recipients.Count() > 0 ? o.recipients[0].id : "0") == rec.id).id;
                        selectedChannel = channel;
                    GetMessages(channel.id, StaticData.NumberMessagesToRetrieve);
                    onFriend = true;
                        while (onFriend)
                        {
                            Console.Clear();
                            foreach (Data.EventTypes.MESSAGE_CREATE.Event_message_create mc in StaticData.Messages)
                            {
                                Console.WriteLine(string.Format("{0}#{1}:{2}\n{3}\n", mc.d.author.username, mc.d.author.discriminator, mc.d.timestamp, mc.d.content));
                            }

                            Console.Write("Command: ");
                            Command(Console.ReadLine());
                        }
                    
                }
                while (!inServer && sel == 'y')
                {
                    try
                    {
                        for (int i = 0; i < ReadyEvent.d.guilds.Count; i++)
                        {
                            Console.WriteLine(string.Format("{3}| Name: {0} | Members: {1} | ID: {2}", ReadyEvent.d.guilds[i].name, ReadyEvent.d.guilds[i].member_count, ReadyEvent.d.guilds[i].id, i));
                        }
                        Console.Write("Select a guild: \n");
                        string content = Console.ReadLine();
                        if (content.ToUpper() == "BACK")
                            Exit("END");
                        selectedGuild = ReadyEvent.d.guilds[int.Parse(content)];
                        inServer = true;
                    }
                    catch
                    {
                        Console.WriteLine("Try again!");
                    }
                }
                while (!inChannel && sel == 'y')
                {
                    List<Channel> textChannels = selectedGuild.channels.Where(root => root.type == 0).ToList();
                    try
                    {
                        Console.WriteLine("Channels: \n");
                        for(int i = 0; i < textChannels.Count(); i++)
                        {
                            Console.WriteLine(string.Format("{0}|{1} | nsfw: {2}", i, textChannels[i].name, textChannels[i].nsfw));
                        }
                        string content = Console.ReadLine();
                        if (content.ToUpper() == "BACK")
                        {
                            inServer = false;
                            break;
                        }
                        selectedChannel = textChannels[int.Parse(content)];
                        inChannel = true;
                    }
                    catch
                    {
                        Console.WriteLine("Try again!");
                    }
                    finally
                    {
                        textChannels.Clear();
                    }
                }
                while(inServer && inChannel)
                {
                    Console.Clear();
                    foreach (Data.EventTypes.MESSAGE_CREATE.Event_message_create mc in StaticData.Messages)
                    {
                        Console.WriteLine(string.Format("{0}#{1}:{2}\n{3}\n", mc.d.author.username, mc.d.author.discriminator, mc.d.timestamp, mc.d.content));
                    }
                    Console.Write("Command: ");
                    Command(Console.ReadLine());
                }
            }
        }

        static void Command(string userinput)
        {
            switch (userinput.Split(' ')[0].ToUpper())
            {
                case "POST":
                    Send(userinput.Substring(4, userinput.Length - 4));
                    Thread.Sleep(10);
                    break;
                case "BACK":
                    if (inChannel)
                        inChannel = !inChannel;
                    else if (onFriend)
                        onFriend = !onFriend;

                    selectedChannel = null;
                    Console.Clear();
                    StaticData.Messages.Clear();
                    Console.SetCursorPosition(dleft, dtop);
                    break;
                case "CLEAR":
                    Console.Clear();
                    StaticData.Messages.Clear();
                    break;
                case "LOAD":
                    StaticData.NumberMessagesToRetrieve = int.Parse(userinput.Substring(4, userinput.Length - 4));
                    Console.Title = "Number of messages to load has changed; reload the channel manually for it to take effect.";
                    break;
                case "EXIT":
                    Exit("User requested termination");
                    break;
            }
        }

        static void GetMessages(string channel_id, int amount)
        {
            //https://discordapp.com/api/v6/channels/358835562389438464/messages?limit=50
            System.Threading.Tasks.Task<HttpResponseMessage> response = client.GetAsync(string.Format("https://discordapp.com/api/v6/channels/{0}/messages?limit={1}", channel_id, amount.ToString()));
            if (!response.Result.IsSuccessStatusCode)
                throw new Exception();
            System.Threading.Tasks.Task<string> str = response.Result.Content.ReadAsStringAsync();
            List<Data.MESSAGE> MC = JsonConvert.DeserializeObject<List<Data.MESSAGE>>(str.Result);
            Console.WriteLine(str.Result);
            foreach(Data.MESSAGE eventMessage in MC)
                StaticData.Messages.Add(eventMessage.toEventMessage());

            StaticData.Messages.Reverse();

            foreach (Data.EventTypes.MESSAGE_CREATE.Event_message_create mc in StaticData.Messages)
                Console.WriteLine(string.Format("{0}#{1}:{2}\n{3}\n", mc.d.author.username, mc.d.author.discriminator, mc.d.timestamp, mc.d.content));
        }

        /// <summary>
        /// Send a string to the selected server.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        static bool Send([Optional] string content)
        {
            if (content == null)
                content = Console.ReadLine();
            StringContent data = new StringContent(JsonConvert.SerializeObject(new Message(content)), UnicodeEncoding.UTF8, "application/json");
            try
            {
                System.Threading.Tasks.Task<HttpResponseMessage> response = client.PostAsync(string.Format("https://discordapp.com/api/v6/channels/{0}/messages", selectedChannel.id), data);
                if (!response.Result.IsSuccessStatusCode)
                    throw new Exception();
                Console.Title = "Message Sent";
                return true;
            }
            catch (Exception ex)
            {
                Console.Title = ex.Message;
                Console.WriteLine("Error posting: " + ex.Message);
                return false;
            }
            finally
            {
                data.Dispose();
            }
        }
        class Message // semi-permanent. Removed/moved in next update.
        {
            public string content { get; set; }
            public Message(string content)
            {
                this.content = content;
            }
        }
        static void Debug()
        {
            debugFlag = true;
        }

        private static void Ws_OnOpen(object sender, EventArgs e)
        {
            Console.WriteLine("Connected! Waiting for handshake...");
        }

        private static void Ws_OnClose(object sender, CloseEventArgs e)
        {
            Console.WriteLine(e.Reason);
            Console.WriteLine(e.Code);
        }

        //Console.WriteLine("DATA FLAG");
        //Console.WriteLine(e.Data);
        //Console.WriteLine("END DATA");
        //int x = 0;
        //int.TryParse(e.Data.Split(',')[2].Split(':')[1], out x);

        static ReadyEvent Parse(string data)
        {
            try
            {
                ReadyEvent RO = JsonConvert.DeserializeObject<ReadyEvent>(data);
                return RO;
            }
            catch
            {
                return null;
            }
        }
        static ReadyEvent RO = null;
        private static void Ws_OnMessage(object sender, MessageEventArgs e)
        {
            RO = Parse(e.Data);
            if(RO != null)
            {
                if (flg != 2)
                {
                    if (readycount >= 60)
                        Exit("FAILED TO RECIEVE CRITICAL OP FROM SERVER");
                    if (flg < 1)
                    {
                        readycount++;
                        if (RO.op == 10)
                        {
                            Console.WriteLine("Op10 has been recieved!, parsing...");
                            HeartBeat heart = JsonConvert.DeserializeObject<HeartBeat>(e.Data);
                            heartbeat = heart.d.heartbeat_interval;
                            Console.WriteLine("Heartbeat: " + heartbeat);
                            Thread thr = new Thread(() => PostOp1(heartbeat));
                            thr.Start();
                            flg = 1;
                            RO = null;
                            ws.Send(op2);
                            Console.WriteLine("Replying with OP2... waiting."); // Seems to be some kind of loop here.
                            readycount = 0;
                        }
                    }
                    else
                    {
                        if (RO.t == "READY")
                        {
                            Console.WriteLine("Recieved 'READY' event! Handshake complete!");
                            StaticData.CurrentUser = RO.d.user;
                            StaticData.Guilds = RO.d.guilds;
                            ReadyEvent = RO;
                            flg = 2;
                        }
                        else
                        {
                            readycount++;
                            RO = null;
                        }
                    }
                }
                else
                {
                    if(RO.t != null)
                    {
                        Console.Title = (RO.t);
                        switch (RO.t)
                        {
                            case "MESSAGE_CREATE":
                                Data.EventTypes.MESSAGE_CREATE.Event_message_create MC = JsonConvert.DeserializeObject<Data.EventTypes.MESSAGE_CREATE.Event_message_create>(e.Data);
                                if (selectedChannel != null)
                                {
                                    if (MC.d.channel_id == selectedChannel.id)
                                        StaticData.Messages.Add(MC);
                                    Console.Clear();
                                    foreach (Data.EventTypes.MESSAGE_CREATE.Event_message_create mc in StaticData.Messages)
                                    {
                                        Console.WriteLine(string.Format("{0}#{1}:{2}\n{3}\n", mc.d.author.username, mc.d.author.discriminator, mc.d.timestamp, mc.d.content));
                                    }

                                    Console.Write("Command: ");
                                }
                                //Console.Title = "added";
                                break;
                            case "PRESENCE_UPDATE":
                                Data.EventTypes.PRESENCE_UPDATE _UPDATE = JsonConvert.DeserializeObject<Data.EventTypes.PRESENCE_UPDATE>(e.Data);
                                break;
                            case "MESSAGE_ACK": // ACK (Heartbeat interval)
                                break;
                            default:
                                if (debugFlag)
                                    Console.WriteLine("UNKNOWN EVENT: " + string.Format("{0}\n{1}", RO.t, JsonConvert.DeserializeObject(e.Data)));
                                break;
                        }
                    }
                    else
                    {
                        Console.Title = "   ";
                    }
                }
            }
        }   

        static void PostOp1(int heartbeat_interval)
        {
            while (true)
            {
                ws.Send(op1);
                Thread.Sleep(heartbeat_interval);
            }
        }
    }
}
