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
        static WebSocket ws = new WebSocket("wss://gateway.discord.gg/?v=6&encoding=json");
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
            Environment.Exit(0);
        }
        public static bool SetUp()
        {
            try
            {
                Console.WriteLine("Setting Events!");
                ws.OnOpen += Ws_OnOpen;
                ws.OnMessage += Ws_OnMessage;
                ws.OnClose += Ws_OnClose;

                Console.WriteLine("Setting Arguments!");
                dtop = Console.CursorTop; // May not work on some computers or in integrated/emuated consoles.
                dleft = Console.CursorLeft;
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
            if (!SetUp())
                Exit();
            Console.WriteLine("Connecting!");
            int timeout = 0;
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

            //var data = new { content = "Hello world!" };
            //Console.WriteLine(data.AsJson().ReadAsStringAsync().Result);//654860494414544905 539280524041125889
            //var response = client.PostAsync("https://discordapp.com/api/v6/channels/539280524041125889/messages", data.AsJson());
            //Console.WriteLine(response.Result);
            #if(debug)
            Console.WriteLine("D for Debug or N for Normal?\n");
            if ((Console.ReadLine().ToLower() == "d") == true)
                Debug();
            #endif
            Run();
            Console.ReadLine();
            ws.Close();
        }
        static void Run()
        {
            while (true)
            {

                while (!inServer)
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
                while (!inChannel)
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
                //int inloop = 1;
                while(inServer && inChannel)
                {
                    //if(inloop < 1)
                        //Console.SetCursorPosition(0, 0);
                    Console.Clear();
                    //Console.WriteLine("\n\n");
                    foreach (Data.EventTypes.MESSAGE_CREATE.Event_message_create mc in StaticData.Messages)
                    {
                        Console.WriteLine(string.Format("{0}#{1}:{2}\n{3}\n", mc.d.author.username, mc.d.author.discriminator, mc.d.timestamp, mc.d.content));
                    }
                    Console.Write("Command: ");
                    String content = Console.ReadLine();
                    switch (content.Split(' ')[0].ToUpper())
                    {
                        case "POST":
                            Send(content.Substring(4, content.Length - 4));
                            Thread.Sleep(20); // wait for response from server.
                            // TODO: Create an event and wait wait on that event to be triggered. (Or wait on a flag to change.)
                            break;
                        case "BACK":
                            inChannel = false;
                            Console.Clear();
                            Console.SetCursorPosition(dleft, dtop);
                            break;
                        case "CLEAR":
                            Console.Clear();
                            StaticData.Messages.Clear();
                            break;
                    }
                }
            }
        }
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
                //Console.WriteLine("Error posting: " + ex.Message);
                return false;
            }
            finally
            {
                data.Dispose();
            }
        }
        class Message // temporary
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
            Console.WriteLine(string.Format("The connection to the Discord API has unexpectedly closed!\nMsg: {0}\nCode: {1}", e.Reason, e.Code));
            Console.ReadLine();
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
        private static void Ws_OnMessage(object sender, MessageEventArgs e)
        {
            ReadyEvent RO = null;
            while(RO == null)
            {
                RO = Parse(e.Data);
            }
            if(flg != 2)
            {
                if (readycount >= 20)
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
                        readycount++;
                }
            }
            else
            {
                switch (RO.t)
                {
                    default:
                        if(debugFlag) // debug flag is set to false yet this still happens.
                            Console.WriteLine("UNKNOWN EVENT: " + string.Format("{0}\n{1}", RO.t, JsonConvert.DeserializeObject(e.Data)));
                        break;
                    case "MESSAGE_CREATE":
                        Data.EventTypes.MESSAGE_CREATE.Event_message_create MC = JsonConvert.DeserializeObject<Data.EventTypes.MESSAGE_CREATE.Event_message_create>(e.Data);
                        StaticData.Messages.Add(MC);
                        Console.Title = "added";
                        break;
                    case "PRESENCE_UPDATE":
                        Data.EventTypes.PRESENCE_UPDATE _UPDATE = JsonConvert.DeserializeObject<Data.EventTypes.PRESENCE_UPDATE>(e.Data);
                        break;
                    case "MESSAGE_ACK": // ACK
                        break;
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
        static void Push(string token, string msg, string channel)
        {

        }
    }
}
